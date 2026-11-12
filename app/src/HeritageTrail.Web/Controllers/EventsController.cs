using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace HeritageTrail.Web.Controllers;

public sealed class EventsController : Controller
{
    private readonly MongoContext _db;

    public EventsController(MongoContext db) => _db = db;

    [HttpGet("/events")]
    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var upcoming = await _db.Events.Find(e => e.StartDate >= now).SortBy(e => e.StartDate).ToListAsync();
        var past = await _db.Events.Find(e => e.StartDate < now).SortByDescending(e => e.StartDate).ToListAsync();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var interested = userId is null
            ? new List<string>()
            : (await _db.Interests.Find(i => i.UserId == userId).ToListAsync()).Select(i => i.EventId).ToList();

        ViewData["Upcoming"] = upcoming;
        ViewData["Past"] = past;
        ViewData["Interested"] = interested;
        return View();
    }

    [HttpGet("/events/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var ev = await _db.Events.Find(e => e.Slug == slug).FirstOrDefaultAsync();
        if (ev is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isInterested = userId is not null &&
            await _db.Interests.CountDocumentsAsync(i => i.EventId == ev.Id && i.UserId == userId) > 0;
        var site = ev.SiteId is null ? null : await _db.Sites.Find(s => s.Id == ev.SiteId).FirstOrDefaultAsync();

        ViewData["Event"] = ev;
        ViewData["Site"] = site;
        ViewData["Interested"] = isInterested;
        return View();
    }

    [Authorize]
    [HttpPost("/events/{id}/interest")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleInterest(string id)
    {
        var ev = await _db.Events.Find(e => e.Id == id).FirstOrDefaultAsync();
        if (ev is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (ev.StartDate < DateTime.UtcNow)
        {
            TempData["Notice"] = "This programme has already taken place.";
            return RedirectToAction(nameof(Detail), new { slug = ev.Slug });
        }

        var existing = await _db.Interests.Find(i => i.EventId == id && i.UserId == userId).FirstOrDefaultAsync();
        if (existing is null)
        {
            await _db.Interests.InsertOneAsync(new EventInterest { EventId = id, UserId = userId });
            TempData["Notice"] = "Interest recorded.";
        }
        else
        {
            await _db.Interests.DeleteOneAsync(i => i.Id == existing.Id);
            TempData["Notice"] = "Interest removed.";
        }

        return RedirectToAction(nameof(Detail), new { slug = ev.Slug });
    }
}
