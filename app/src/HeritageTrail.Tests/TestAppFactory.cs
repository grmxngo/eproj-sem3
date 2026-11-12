using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace HeritageTrail.Tests;

/// <summary>
/// Hosts the app against an isolated MongoDB on port 27020 (temp dbpath), seeded with SeedMode=Reset.
/// </summary>
public sealed class TestAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const int MongoPort = 27020;

    private Process? _mongo;
    private readonly string _tmp = Path.Combine(Root(), "artifacts", "test-tmp", Guid.NewGuid().ToString("N"));

    private static string Root()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "app", "src", "HeritageTrail.Web")))
        {
            dir = dir.Parent;
        }
        if (dir is null)
        {
            throw new InvalidOperationException("Workspace root (with app/src/HeritageTrail.Web) not found");
        }
        return dir.FullName;
    }

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(_tmp);
        var dbPath = Path.Combine(_tmp, "mongo");
        Directory.CreateDirectory(dbPath);

        var mongod = Directory.GetFiles(@"C:\Program Files\MongoDB\Server", "mongod.exe", SearchOption.AllDirectories)
            .OrderByDescending(x => x)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("mongod.exe not found under C:\\Program Files\\MongoDB\\Server");

        _mongo = Process.Start(new ProcessStartInfo
        {
            FileName = mongod,
            Arguments = $"--dbpath \"{dbPath}\" --port {MongoPort} --bind_ip 127.0.0.1 --quiet",
            CreateNoWindow = true,
            UseShellExecute = false
        })!;

        var deadline = DateTime.UtcNow.AddSeconds(30);
        var up = false;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var tcp = new TcpClient();
                await tcp.ConnectAsync("127.0.0.1", MongoPort);
                up = true;
                break;
            }
            catch
            {
                await Task.Delay(300);
            }
        }

        if (!up)
        {
            throw new InvalidOperationException("Test mongod did not start on port " + MongoPort);
        }

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
        Environment.SetEnvironmentVariable("HT_MONGO_PORT", MongoPort.ToString());
        Environment.SetEnvironmentVariable("HT_DB_NAME", "heritagetrail_test");
        Environment.SetEnvironmentVariable("HT_SEED", "Reset");
        Environment.SetEnvironmentVariable("HT_UPLOADS", Path.Combine(_tmp, "uploads"));
        Environment.SetEnvironmentVariable("HT_MAIL", Path.Combine(_tmp, "mail"));
        Environment.SetEnvironmentVariable("HT_ATOMS", "false");

        Client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    public HttpClient NewClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        HandleCookies = true
    });

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Pin the content root to the web project regardless of where the host is launched from.
        var webRoot = Path.Combine(Root(), "app", "src", "HeritageTrail.Web");
        builder.ConfigureWebHost(web => web.UseContentRoot(webRoot));
        return base.CreateHost(builder);
    }

    public new async Task DisposeAsync()
    {
        Client.Dispose();
        try
        {
            _mongo?.Kill(entireProcessTree: true);
            _mongo?.WaitForExit(5000);
        }
        catch
        {
            // process already gone
        }

        try
        {
            Directory.Delete(_tmp, true);
        }
        catch
        {
            // temp cleanup is best-effort
        }
    }
}
