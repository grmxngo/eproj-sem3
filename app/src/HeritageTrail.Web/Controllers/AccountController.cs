using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace HeritageTrail.Web.Controllers;

[Authorize]
public sealed class AccountController : Controller
{
    private readonly MongoContext _db;

    public AccountController(MongoContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("/account")]
    public async Task<IActionResult> Index()
    {
        var interests = await _db.Interests.Find(i => i.UserId == UserId).ToListAsync();
        var eventIds = interests.Select(i => i.EventId).ToList();
        var events = eventIds.Count == 0
            ? new List<Event>()
            : await _db.Events.Find(e => eventIds.Contains(e.Id)).SortBy(e => e.StartDate).ToListAsync();
        var queries = await _db.Queries.Find(q => q.UserId == UserId).SortByDescending(q => q.CreatedAt).ToListAsync();
        var invitations = await _db.Invitations.Find(i => i.FromUserId == UserId).SortByDescending(i => i.SentAt).ToListAsync();

        ViewData["Events"] = events;
        ViewData["Queries"] = queries;
        ViewData["Invitations"] = invitations;
        return View();
    }

    [HttpPost("/account/queries")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateQuery(string subject, string message)
    {
        if (!string.IsNullOrWhiteSpace(subject) && !string.IsNullOrWhiteSpace(message))
        {
            await _db.Queries.InsertOneAsync(new Query
            {
                UserId = UserId,
                Subject = subject.Trim(),
                Message = message.Trim()
            });
            TempData["Notice"] = "Query sent.";
        }
        return Redirect("/account");
    }
}
