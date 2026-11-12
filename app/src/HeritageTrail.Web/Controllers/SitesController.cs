using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace HeritageTrail.Web.Controllers;

public sealed class SitesController : Controller
{
    private readonly MongoContext _db;

    public SitesController(MongoContext db) => _db = db;

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

        ViewData["Sites"] = sites;
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
        return View();
    }
}
