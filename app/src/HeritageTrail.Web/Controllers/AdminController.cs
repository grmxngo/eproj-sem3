using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace HeritageTrail.Web.Controllers;

[Authorize(Roles = "admin")]
public sealed class AdminController : Controller
{
    private readonly MongoContext _db;
    private readonly MailOutbox _outbox;
    private readonly UploadsStore _uploads;

    public AdminController(MongoContext db, MailOutbox outbox, UploadsStore uploads)
    {
        _db = db;
        _outbox = outbox;
        _uploads = uploads;
    }

    private static string Slugify(string value) =>
        Regex.Replace(value.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');

    private static List<string> ParseFacts(string? text) =>
        string.IsNullOrWhiteSpace(text)
            ? new List<string>()
            : text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    [HttpGet("/admin")]
    public async Task<IActionResult> Index()
    {
        ViewData["Sites"] = await _db.Sites.CountDocumentsAsync(FilterDefinition<HeritageSite>.Empty);
        ViewData["Events"] = await _db.Events.CountDocumentsAsync(FilterDefinition<Event>.Empty);
        ViewData["Members"] = await _db.Users.CountDocumentsAsync(u => u.Role == "user");
        ViewData["OpenQueries"] = await _db.Queries.CountDocumentsAsync(q => q.Status == "Open");
        ViewData["RecentQueries"] = await _db.Queries
            .Find(FilterDefinition<Query>.Empty)
            .SortByDescending(q => q.CreatedAt)
            .Limit(5)
            .ToListAsync();
        return View();
    }

    [HttpGet("/admin/sites")]
    public async Task<IActionResult> Sites()
    {
        ViewData["Sites"] = await _db.Sites.Find(FilterDefinition<HeritageSite>.Empty).SortBy(s => s.Name).ToListAsync();
        return View();
    }

    [HttpGet("/admin/sites/new")]
    public async Task<IActionResult> SiteNew()
    {
        ViewData["Categories"] = await _db.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync();
        ViewData["Site"] = new HeritageSite();
        return View("SiteEdit");
    }

    [HttpGet("/admin/sites/{id}/edit")]
    public async Task<IActionResult> SiteEdit(string id)
    {
        var site = await _db.Sites.Find(s => s.Id == id).FirstOrDefaultAsync();
        if (site is null) return NotFound();
        ViewData["Categories"] = await _db.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync();
        ViewData["Site"] = site;
        return View("SiteEdit");
    }

    [HttpPost("/admin/sites")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SiteSave(
        string? id, string name, string? slug, string category, string location,
        string historicalBackground, string architectureSignificance,
        string? culturalBackground, string? imagesCsv, string? factsText, string? video, bool isAssociated)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(location))
        {
            TempData["Notice"] = "Name and location are required.";
            return RedirectToAction(nameof(Sites));
        }

        var images = string.IsNullOrWhiteSpace(imagesCsv)
            ? new List<string>()
            : imagesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        if (string.IsNullOrWhiteSpace(id))
        {
            await _db.Sites.InsertOneAsync(new HeritageSite
            {
                Name = name.Trim(),
                Slug = string.IsNullOrWhiteSpace(slug) ? Slugify(name) : Slugify(slug),
                Category = category,
                Location = location.Trim(),
                HistoricalBackground = historicalBackground,
                ArchitectureSignificance = architectureSignificance,
                CulturalBackground = culturalBackground,
                Images = images,
                Facts = ParseFacts(factsText),
                Video = string.IsNullOrWhiteSpace(video) ? null : video.Trim(),
                IsAssociated = isAssociated
            });
            TempData["Notice"] = "Site created.";
        }
        else
        {
            await _db.Sites.UpdateOneAsync(
                s => s.Id == id,
                Builders<HeritageSite>.Update
                    .Set(s => s.Name, name.Trim())
                    .Set(s => s.Slug, string.IsNullOrWhiteSpace(slug) ? Slugify(name) : Slugify(slug))
                    .Set(s => s.Category, category)
                    .Set(s => s.Location, location.Trim())
                    .Set(s => s.HistoricalBackground, historicalBackground)
                    .Set(s => s.ArchitectureSignificance, architectureSignificance)
                    .Set(s => s.CulturalBackground, culturalBackground)
                    .Set(s => s.Images, images)
                    .Set(s => s.Facts, ParseFacts(factsText))
                    .Set(s => s.Video, string.IsNullOrWhiteSpace(video) ? null : video.Trim())
                    .Set(s => s.IsAssociated, isAssociated));
            TempData["Notice"] = "Site updated.";
        }

        return RedirectToAction(nameof(Sites));
    }

    [HttpPost("/admin/sites/{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SiteDelete(string id)
    {
        await _db.Sites.DeleteOneAsync(s => s.Id == id);
        TempData["Notice"] = "Site deleted.";
        return RedirectToAction(nameof(Sites));
    }

    [HttpGet("/admin/events")]
    public async Task<IActionResult> Events()
    {
        var events = await _db.Events.Find(FilterDefinition<Event>.Empty).SortByDescending(e => e.StartDate).ToListAsync();
        var interests = await _db.Interests.Find(FilterDefinition<EventInterest>.Empty).ToListAsync();
        ViewData["Events"] = events;
        ViewData["InterestCounts"] = interests.GroupBy(i => i.EventId).ToDictionary(g => g.Key, g => g.Count());
        return View();
    }

    [HttpGet("/admin/events/new")]
    public async Task<IActionResult> EventNew()
    {
        ViewData["Sites"] = await _db.Sites.Find(FilterDefinition<HeritageSite>.Empty).ToListAsync();
        ViewData["Event"] = new Event();
        return View("EventEdit");
    }

    [HttpGet("/admin/events/{id}/edit")]
    public async Task<IActionResult> EventEdit(string id)
    {
        var ev = await _db.Events.Find(e => e.Id == id).FirstOrDefaultAsync();
        if (ev is null) return NotFound();
        ViewData["Sites"] = await _db.Sites.Find(FilterDefinition<HeritageSite>.Empty).ToListAsync();
        ViewData["Event"] = ev;
        return View("EventEdit");
    }

    [HttpPost("/admin/events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventSave(
        string? id, string title, string? slug, string type, string description,
        string? siteId, DateTime startDate, DateTime endDate, string venue)
    {
        if (string.IsNullOrWhiteSpace(title) || endDate < startDate)
        {
            TempData["Notice"] = "Title is required and the end date must be after the start date.";
            return RedirectToAction(nameof(Events));
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            await _db.Events.InsertOneAsync(new Event
            {
                Title = title.Trim(),
                Slug = string.IsNullOrWhiteSpace(slug) ? Slugify(title) : Slugify(slug),
                Type = type,
                Description = description,
                SiteId = string.IsNullOrWhiteSpace(siteId) ? null : siteId,
                StartDate = startDate,
                EndDate = endDate,
                Venue = venue
            });
            TempData["Notice"] = "Event created.";
        }
        else
        {
            await _db.Events.UpdateOneAsync(
                e => e.Id == id,
                Builders<Event>.Update
                    .Set(e => e.Title, title.Trim())
                    .Set(e => e.Slug, string.IsNullOrWhiteSpace(slug) ? Slugify(title) : Slugify(slug))
                    .Set(e => e.Type, type)
                    .Set(e => e.Description, description)
                    .Set(e => e.SiteId, string.IsNullOrWhiteSpace(siteId) ? null : siteId)
                    .Set(e => e.StartDate, startDate)
                    .Set(e => e.EndDate, endDate)
                    .Set(e => e.Venue, venue));
            TempData["Notice"] = "Event updated.";
        }

        return RedirectToAction(nameof(Events));
    }

    [HttpPost("/admin/events/{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EventDelete(string id)
    {
        await _db.Events.DeleteOneAsync(e => e.Id == id);
        TempData["Notice"] = "Event deleted.";
        return RedirectToAction(nameof(Events));
    }

    [HttpGet("/admin/events/{id}/interested")]
    public async Task<IActionResult> Interested(string id)
    {
        var ev = await _db.Events.Find(e => e.Id == id).FirstOrDefaultAsync();
        if (ev is null) return NotFound();
        var interests = await _db.Interests.Find(i => i.EventId == id).ToListAsync();
        var userIds = interests.Select(i => i.UserId).ToList();
        var users = userIds.Count == 0
            ? new List<User>()
            : await _db.Users.Find(u => userIds.Contains(u.Id)).ToListAsync();
        ViewData["Event"] = ev;
        ViewData["Interests"] = interests;
        ViewData["Users"] = users;
        return View();
    }

    [HttpGet("/admin/members")]
    public async Task<IActionResult> Members()
    {
        ViewData["Members"] = await _db.Users.Find(u => u.Role == "user").SortBy(u => u.Name).ToListAsync();
        return View();
    }

    [HttpGet("/admin/queries")]
    public async Task<IActionResult> Queries()
    {
        var queries = await _db.Queries.Find(FilterDefinition<Query>.Empty).SortByDescending(q => q.CreatedAt).ToListAsync();
        var userIds = queries.Select(q => q.UserId).Distinct().ToList();
        var users = userIds.Count == 0
            ? new List<User>()
            : await _db.Users.Find(u => userIds.Contains(u.Id)).ToListAsync();
        ViewData["Queries"] = queries;
        ViewData["Users"] = users;
        return View();
    }

    [HttpPost("/admin/queries/{id}/reply")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QueryReply(string id, string reply)
    {
        var query = await _db.Queries.Find(q => q.Id == id).FirstOrDefaultAsync();
        if (query is not null && !string.IsNullOrWhiteSpace(reply))
        {
            await _db.Queries.UpdateOneAsync(
                q => q.Id == id,
                Builders<Query>.Update
                    .Set(q => q.Reply, reply.Trim())
                    .Set(q => q.Status, "Answered")
                    .Set(q => q.RepliedAt, DateTime.UtcNow));
            var user = await _db.Users.Find(u => u.Id == query.UserId).FirstOrDefaultAsync();
            if (user is not null)
            {
                _outbox.Send(user.Email, $"Reply: {query.Subject}", reply);
            }
            TempData["Notice"] = "Reply saved and sent.";
        }
        return RedirectToAction(nameof(Queries));
    }

    [HttpGet("/admin/about")]
    public async Task<IActionResult> About()
    {
        ViewData["Pages"] = await _db.AboutPages.Find(FilterDefinition<AboutPage>.Empty).ToListAsync();
        return View();
    }

    [HttpGet("/admin/about/{slug}")]
    public async Task<IActionResult> AboutEdit(string slug)
    {
        var page = await _db.AboutPages.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        if (page is null) return NotFound();
        ViewData["Page"] = page;
        return View();
    }

    [HttpPost("/admin/about/{slug}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AboutSave(string slug, string title, string content)
    {
        await _db.AboutPages.UpdateOneAsync(
            p => p.Slug == slug,
            Builders<AboutPage>.Update.Set(p => p.Title, title).Set(p => p.Content, content));
        TempData["Notice"] = "About page updated.";
        return RedirectToAction(nameof(About));
    }

    [HttpGet("/admin/gallery")]
    public async Task<IActionResult> Gallery()
    {
        ViewData["Items"] = await _db.Gallery.Find(FilterDefinition<GalleryItem>.Empty).ToListAsync();
        return View();
    }

    [HttpPost("/admin/gallery")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryAdd(string caption, string? imageUrl, string refType, string? credit, IFormFile? photo)
    {
        var finalUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
        if (photo is { Length: > 0 })
        {
            var (url, error) = await _uploads.SaveAsync(photo);
            if (url is null)
            {
                TempData["Notice"] = error;
                return RedirectToAction(nameof(Gallery));
            }
            finalUrl = url;
        }

        if (!string.IsNullOrWhiteSpace(caption) && !string.IsNullOrWhiteSpace(finalUrl))
        {
            await _db.Gallery.InsertOneAsync(new GalleryItem
            {
                Caption = caption.Trim(),
                ImageUrl = finalUrl,
                RefType = refType,
                Credit = credit
            });
            TempData["Notice"] = "Image added.";
        }
        return RedirectToAction(nameof(Gallery));
    }

    [HttpPost("/admin/gallery/{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GalleryDelete(string id)
    {
        await _db.Gallery.DeleteOneAsync(g => g.Id == id);
        TempData["Notice"] = "Image removed.";
        return RedirectToAction(nameof(Gallery));
    }

    [HttpGet("/admin/contact")]
    public async Task<IActionResult> Contact()
    {
        ViewData["Info"] = await _db.ContactInfos.Find(FilterDefinition<ContactInfo>.Empty).FirstOrDefaultAsync();
        ViewData["Messages"] = await _db.ContactMessages.Find(FilterDefinition<ContactMessage>.Empty)
            .SortByDescending(m => m.CreatedAt).ToListAsync();
        return View();
    }

    [HttpPost("/admin/contact")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactSave(string address, string phone, string email, string note)
    {
        var existing = await _db.ContactInfos.Find(FilterDefinition<ContactInfo>.Empty).FirstOrDefaultAsync();
        if (existing is null)
        {
            await _db.ContactInfos.InsertOneAsync(new ContactInfo { Address = address, Phone = phone, Email = email, Note = note });
        }
        else
        {
            await _db.ContactInfos.UpdateOneAsync(
                c => c.Id == existing.Id,
                Builders<ContactInfo>.Update
                    .Set(c => c.Address, address)
                    .Set(c => c.Phone, phone)
                    .Set(c => c.Email, email)
                    .Set(c => c.Note, note));
        }
        TempData["Notice"] = "Contact info saved.";
        return RedirectToAction(nameof(Contact));
    }

    [HttpGet("/admin/categories")]
    public async Task<IActionResult> Categories()
    {
        var categories = await _db.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync();
        var sites = await _db.Sites.Find(FilterDefinition<HeritageSite>.Empty).ToListAsync();
        ViewData["Categories"] = categories;
        ViewData["Counts"] = sites.GroupBy(s => s.Category).ToDictionary(g => g.Key, g => g.Count());
        return View();
    }

    [HttpPost("/admin/categories")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CategoryAdd(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            await _db.Categories.InsertOneAsync(new Category { Name = name.Trim() });
            TempData["Notice"] = "Category added.";
        }
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost("/admin/categories/{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CategoryDelete(string id)
    {
        var category = await _db.Categories.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (category is null) return RedirectToAction(nameof(Categories));

        var inUse = await _db.Sites.CountDocumentsAsync(s => s.Category == category.Name) > 0;
        if (inUse)
        {
            TempData["Notice"] = $"Cannot delete \"{category.Name}\" — sites still use this category.";
            return RedirectToAction(nameof(Categories));
        }

        await _db.Categories.DeleteOneAsync(c => c.Id == id);
        TempData["Notice"] = "Category deleted.";
        return RedirectToAction(nameof(Categories));
    }
}
