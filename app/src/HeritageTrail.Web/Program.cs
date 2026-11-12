using HeritageTrail.Web.Data;
using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// SSOT: one environment matrix read by app + scripts.
builder.Configuration.AddJsonFile("Content/config/environments.json", optional: false, reloadOnChange: true);

var envName = builder.Environment.EnvironmentName;
var envConfig = builder.Configuration.GetSection($"environments:{envName}").Get<EnvironmentConfig>()
    ?? throw new InvalidOperationException(
        $"Missing 'environments:{envName}' in Content/config/environments.json. Set ASPNETCORE_ENVIRONMENT to Staging or Production.");

// Test/CI overrides (used by the integration test suite to point at an isolated Mongo).
if (int.TryParse(Environment.GetEnvironmentVariable("HT_MONGO_PORT"), out var htMongoPort))
    envConfig = envConfig with { MongoPort = htMongoPort };
if (Environment.GetEnvironmentVariable("HT_DB_NAME") is { Length: > 0 } htDb)
    envConfig = envConfig with { Database = htDb };
if (Environment.GetEnvironmentVariable("HT_SEED") is { Length: > 0 } htSeed)
    envConfig = envConfig with { SeedMode = htSeed };
if (Environment.GetEnvironmentVariable("HT_UPLOADS") is { Length: > 0 } htUploads)
    envConfig = envConfig with { Uploads = htUploads };
if (Environment.GetEnvironmentVariable("HT_MAIL") is { Length: > 0 } htMail)
    envConfig = envConfig with { MailOutbox = htMail };
if (Environment.GetEnvironmentVariable("HT_ATOMS") is { Length: > 0 } htAtoms)
    envConfig = envConfig with { AtomsGallery = bool.TryParse(htAtoms, out var a) && a };

builder.Services.AddSingleton(envConfig);
builder.Services.AddSingleton<EnvironmentInfo>();
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddSingleton<CandidatesProvider>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MailOutbox>();
builder.Services.AddSingleton<UploadsStore>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ht.auth";
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/login";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MongoContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await SeedService.RunAsync(db, envConfig, logger);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Seed skipped — MongoDB unreachable? Health endpoint will report degraded.");
    }
}

app.UseStaticFiles();

var uploadsRoot = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "..", ".."));
var uploadsPath = Path.Combine(uploadsRoot, envConfig.Uploads);
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/healthz", (EnvironmentInfo env, MongoContext db) => Results.Json(new
{
    status = db.Ping() ? "ok" : "degraded",
    environment = env.Name,
    badge = env.Badge,
    database = db.DatabaseName,
    atoms = env.IsAtomsEnabled
}));

app.MapGet("/design/tokens.json", (TokenProvider tokens) =>
    Results.Text(tokens.Raw, "application/json"));

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;
