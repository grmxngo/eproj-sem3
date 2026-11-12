using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.Json.Nodes;

namespace HeritageTrail.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly MongoContext _db;
    private readonly CandidatesProvider _candidates;
    private readonly EnvironmentInfo _env;

    public HomeController(MongoContext db, CandidatesProvider candidates, EnvironmentInfo env)
    {
        _db = db;
        _candidates = candidates;
        _env = env;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(string? palette = null, string? type = null, string? radius = null, string? hero = null, string? events = null, string? cards = null, string? gallery = null)
    {
        var now = DateTime.UtcNow;
        ViewData["Upcoming"] = await _db.Events.Find(e => e.StartDate >= now).SortBy(e => e.StartDate).Limit(4).ToListAsync();
        ViewData["Past"] = await _db.Events.Find(e => e.StartDate < now).SortByDescending(e => e.StartDate).Limit(1).ToListAsync();
        ViewData["Sites"] = await _db.Sites.Find(FilterDefinition<HeritageSite>.Empty).SortBy(s => s.Name).Limit(6).ToListAsync();
        ViewData["Categories"] = await _db.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync();
        ViewData["About"] = await _db.AboutPages.Find(FilterDefinition<AboutPage>.Empty).ToListAsync();
        ViewData["Gallery"] = await _db.Gallery.Find(FilterDefinition<GalleryItem>.Empty).Limit(4).ToListAsync();

        var heroKey = (hero ?? "c").ToLowerInvariant();
        ViewData["Hero"] = heroKey is "a" or "b" or "c" ? heroKey : "c";

        var eventsKey = (events ?? "e3").ToLowerInvariant();
        ViewData["Events"] = eventsKey is "e1" or "e2" or "e3" ? eventsKey : "e3";

        var cardsKey = (cards ?? "s1").ToLowerInvariant();
        ViewData["Cards"] = cardsKey is "s2" or "s3" ? cardsKey : "s1";

        var galleryKey = (gallery ?? "g1").ToLowerInvariant();
        ViewData["GalleryStyle"] = galleryKey is "g2" or "g3" ? galleryKey : "g1";

        if (!string.IsNullOrWhiteSpace(palette) && _env.IsStaging)
        {
            var choice = _candidates.HomePalettes.FirstOrDefault(p =>
                p!["id"]!.GetValue<string>().Equals(palette, StringComparison.OrdinalIgnoreCase));
            if (choice is not null)
            {
                ViewData["PaletteVars"] = BuildPaletteVars(choice.AsObject());
            }
        }

        if (!string.IsNullOrWhiteSpace(type) && _env.IsStaging)
        {
            var set = _candidates.TypeSets.FirstOrDefault(p =>
                p!["id"]!.GetValue<string>().Equals(type, StringComparison.OrdinalIgnoreCase));
            if (set is not null)
            {
                var s = set.AsObject();
                ViewData["TypeVars"] =
                    $"--font-display:{s["display"]!.GetValue<string>()};" +
                    $"--font-body:{s["body"]!.GetValue<string>()};" +
                    $"--font-mono:{s["mono"]!.GetValue<string>()};";
            }
        }

        if (!string.IsNullOrWhiteSpace(radius) && _env.IsStaging)
        {
            var set = _candidates.RadiusSets.FirstOrDefault(p =>
                p!["id"]!.GetValue<string>().Equals(radius, StringComparison.OrdinalIgnoreCase));
            if (set is not null)
            {
                var s = set.AsObject();
                ViewData["RadiusVars"] =
                    $"--radius-sm:{s["sm"]!.GetValue<string>()};" +
                    $"--radius-md:{s["md"]!.GetValue<string>()};" +
                    $"--radius-lg:{s["lg"]!.GetValue<string>()};" +
                    $"--radius-pill:{s["pill"]!.GetValue<string>()};";
            }
        }

        return View();
    }

    private static string BuildPaletteVars(JsonObject palette)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var (k, v) in palette["color"]!.AsObject())
        {
            if (v is JsonValue jv && jv.GetValueKind() == System.Text.Json.JsonValueKind.String)
            {
                sb.Append("--color-").Append(CssNaming.Kebab(k)).Append(':').Append(jv.GetValue<string>()).Append(';');
            }
        }
        return sb.ToString();
    }
}
