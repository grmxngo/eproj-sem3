using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace HeritageTrail.Web.Controllers;

public sealed class SitesController : Controller
{
    private readonly MongoContext _db;
    private readonly IWebHostEnvironment _env;

    public SitesController(MongoContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("/heritage-sites")]
    public async Task<IActionResult> Index(string? q, string? cat)
    {
        var filter = Builders<HeritageSite>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(cat))
        {
            filter &= Builders<HeritageSite>.Filter.Eq(s => s.Category, cat);
        }
        if (!string.IsNullOrWhiteSpace(q))
        {
            var rx = new BsonRegularExpression(Regex.Escape(q.Trim()), "i");
            filter &= Builders<HeritageSite>.Filter.Or(
                Builders<HeritageSite>.Filter.Regex(s => s.Name, rx),
                Builders<HeritageSite>.Filter.Regex(s => s.Location, rx));
        }

        var sites = await _db.Sites.Find(filter).SortBy(s => s.Name).ToListAsync();
        var categories = await _db.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync();

        ViewData["Sites"] = sites.Where(s => !s.IsAssociated).ToList();
        ViewData["Associated"] = sites.Where(s => s.IsAssociated).ToList();
        ViewData["Categories"] = categories;
        ViewData["Q"] = q;
        ViewData["Cat"] = cat;
        return View();
    }

    [HttpGet("/heritage-sites/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var site = await _db.Sites.Find(s => s.Slug == slug).FirstOrDefaultAsync();
        if (site is null) return NotFound();

        var events = await _db.Events.Find(e => e.SiteId == site.Id).SortBy(e => e.StartDate).ToListAsync();
        ViewData["Site"] = site;
        ViewData["Events"] = events;
        ViewData["Description"] = $"{site.Name} — {site.ArchitectureSignificance}";
        ViewData["Credits"] = LoadCredits();
        return View();
    }

    /// <summary>Image attribution from the fetch pipeline (wwwroot/images/sites/credits.json).</summary>
    private Dictionary<string, string> LoadCredits()
    {
        var map = new Dictionary<string, string>();
        try
        {
            var path = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
                "images", "sites", "credits.json");
            if (!System.IO.File.Exists(path)) return map;

            var root = JsonNode.Parse(System.IO.File.ReadAllText(path))?.AsArray();
            if (root is null) return map;

            foreach (var item in root)
            {
                var local = item?["local"]?.GetValue<string>();
                if (local is null) continue;
                var artist = Regex.Replace(item?["artist"]?.GetValue<string>() ?? "", "<[^>]+>", "").Trim();
                var license = item?["license"]?.GetValue<string>() ?? "";
                var text = string.IsNullOrWhiteSpace(artist) ? "Wikimedia Commons" : $"© {artist} · {license}";
                map[local] = text;
            }
        }
        catch
        {
            // attribution is best-effort; the caption falls back to a plain label
        }
        return map;
    }
}
