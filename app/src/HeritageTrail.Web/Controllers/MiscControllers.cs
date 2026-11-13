using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace HeritageTrail.Web.Controllers;

public sealed class AboutController : Controller
{
    private readonly MongoContext _db;
    public AboutController(MongoContext db) => _db = db;

    [HttpGet("/about/{slug}")]
    public async Task<IActionResult> Show(string slug)
    {
        var page = await _db.AboutPages.Find(p => p.Slug == slug).FirstOrDefaultAsync();
        if (page is null) return NotFound();
        ViewData["Page"] = page;
        ViewData["Pages"] = await _db.AboutPages.Find(FilterDefinition<AboutPage>.Empty).ToListAsync();
        return View();
    }
}

public sealed class GalleryController : Controller
{
    private readonly MongoContext _db;
    public GalleryController(MongoContext db) => _db = db;

    [HttpGet("/gallery")]
    public async Task<IActionResult> Index()
    {
        ViewData["Items"] = await _db.Gallery.Find(FilterDefinition<GalleryItem>.Empty).ToListAsync();
        return View();
    }
}

public sealed class ContactController : Controller
{
    private readonly MongoContext _db;
    public ContactController(MongoContext db) => _db = db;

    [HttpGet("/contact")]
    public async Task<IActionResult> Index()
    {
        ViewData["Info"] = await _db.ContactInfos.Find(FilterDefinition<ContactInfo>.Empty).FirstOrDefaultAsync();
        return View();
    }

    [HttpPost("/contact")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string name, string email, string message)
    {
        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(message))
        {
            await _db.ContactMessages.InsertOneAsync(new ContactMessage { Name = name.Trim(), Email = email.Trim(), Message = message.Trim() });
            TempData["Notice"] = "Message sent.";
        }
        return Redirect("/contact");
    }
}

public sealed class InviteController : Controller
{
    private readonly MongoContext _db;
    private readonly MailOutbox _outbox;

    public InviteController(MongoContext db, MailOutbox outbox)
    {
        _db = db;
        _outbox = outbox;
    }

    [Authorize]
    [HttpPost("/heritage-sites/{slug}/invite")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string slug, string email)
    {
        var site = await _db.Sites.Find(s => s.Slug == slug).FirstOrDefaultAsync();
        if (site is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
        {
            var from = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _db.Invitations.InsertOneAsync(new Invitation
            {
                FromUserId = from,
                ToEmail = email.Trim(),
                SiteId = site.Id
            });
            _outbox.Send(
                email.Trim(),
                $"{User.Identity?.Name} invites you to explore {site.Name}",
                $"Explore {site.Name} — {site.Location}: https://127.0.0.1:5181/heritage-sites/{site.Slug}");
            TempData["Notice"] = "Invitation sent.";
        }
        else
        {
            TempData["Notice"] = "Enter a valid email address.";
        }

        return Redirect($"/heritage-sites/{slug}");
    }
}


/// <summary>/explore — browse sites grouped by category (visual overview; the searchable index is /heritage-sites).</summary>
public sealed class ExploreController : Controller
{
    private readonly MongoContext _db;

    public ExploreController(MongoContext db) => _db = db;

    [HttpGet("/explore")]
    public async Task<IActionResult> Index()
    {
        var sites = await _db.Sites.Find(FilterDefinition<HeritageSite>.Empty).SortBy(s => s.Name).ToListAsync();
        var categories = await _db.Categories.Find(FilterDefinition<Category>.Empty).ToListAsync();
        ViewData["Sites"] = sites;
        ViewData["Categories"] = categories;
        return View();
    }
}


/// <summary>/error/{code} — designed error pages (404, 500, 403).</summary>
public sealed class ErrorController : Controller
{
    [HttpGet("/error/{code:int}")]
    public IActionResult Index(int code)
    {
        Response.StatusCode = code is >= 400 and <= 599 ? code : 500;
        ViewData["Code"] = code;
        ViewData["Title"] = "Error " + code;
        ViewData["Message"] = code switch
        {
            404 => "The page you asked for does not exist on this site.",
            403 => "You do not have permission to open this page.",
            _ => "Something went wrong on our side. Please try again."
        };
        return View();
    }
}
