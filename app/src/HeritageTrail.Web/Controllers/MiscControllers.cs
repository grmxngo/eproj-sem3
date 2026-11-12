using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
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
    public InviteController(MongoContext db) => _db = db;

    [Authorize]
    [HttpPost("/heritage-sites/{slug}/invite")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string slug, string email)
    {
        var site = await _db.Sites.Find(s => s.Slug == slug).FirstOrDefaultAsync();
        if (site is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
        {
            await _db.Invitations.InsertOneAsync(new Invitation
            {
                FromUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                ToEmail = email.Trim(),
                SiteId = site.Id
            });
            TempData["Notice"] = "Invitation recorded.";
        }
        else
        {
            TempData["Notice"] = "Enter a valid email address.";
        }

        return Redirect($"/heritage-sites/{slug}");
    }
}
