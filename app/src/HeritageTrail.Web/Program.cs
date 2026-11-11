using HeritageTrail.Web.Data;
using HeritageTrail.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// SSOT: one environment matrix read by app + scripts.
builder.Configuration.AddJsonFile("Content/config/environments.json", optional: false, reloadOnChange: true);

var envName = builder.Environment.EnvironmentName;
var envConfig = builder.Configuration.GetSection($"environments:{envName}").Get<EnvironmentConfig>()
    ?? throw new InvalidOperationException(
        $"Missing 'environments:{envName}' in Content/config/environments.json. Set ASPNETCORE_ENVIRONMENT to Staging or Production.");

builder.Services.AddSingleton(envConfig);
builder.Services.AddSingleton<EnvironmentInfo>();
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddSingleton<CandidatesProvider>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

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
