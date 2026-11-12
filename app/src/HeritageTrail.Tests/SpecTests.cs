using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.Text;
using Xunit;

namespace HeritageTrail.Tests;

public sealed class SpecTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;
    private readonly HttpClient _client;
    private readonly MongoContext _db;

    public SpecTests(TestAppFactory factory)
    {
        _factory = factory;
        _client = factory.NewClient();
        _db = factory.Services.GetRequiredService<MongoContext>();
    }

    private static string UniqueEmail(string prefix = "user") =>
        $"{prefix}-{Guid.NewGuid():N}@test.local";

    private async Task<HttpResponseMessage> RegisterAsync(string email, string password = "Password123")
    {
        var token = await Http.TokenAsync(_client, "/auth/register");
        return await _client.PostAsync("/auth/register", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["name"] = "Test User",
            ["email"] = email,
            ["password"] = password
        }));
    }

    [Fact]
    public async Task Register_ValidUser_RedirectsToAccount()
    {
        var resp = await RegisterAsync(UniqueEmail());
        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        Assert.Equal("/account", resp.Headers.Location?.ToString());

        var account = await _client.GetAsync("/account");
        Assert.Equal(HttpStatusCode.OK, account.StatusCode);
        Assert.Contains("My account", await account.BodyAsync());
    }

    [Fact]
    public async Task Register_DuplicateEmail_IsRejected()
    {
        var email = UniqueEmail();
        await RegisterAsync(email);

        var second = await RegisterAsync(email);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Contains("Email already registered", await second.BodyAsync());
    }

    [Fact]
    public async Task Guest_AdminPage_RedirectsToLogin()
    {
        var resp = await _client.GetAsync("/admin");
        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        Assert.Contains("/auth/login", resp.Headers.Location?.ToString());
    }

    [Fact]
    public async Task NonAdminUser_AdminPage_IsDenied()
    {
        await Http.LoginAsync(_client, "demo@heritagetrail.local", "Demo123!");
        var resp = await _client.GetAsync("/admin");
        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
        Assert.Contains("/auth/login", resp.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Search_NoResults_ShowsEmptyMessage()
    {
        var html = await _client.GetStringAsync("/heritage-sites?q=qqqqzzzz");
        Assert.Contains("No heritage site found", html);
    }

    [Fact]
    public async Task Search_Category_FiltersSites()
    {
        var html = await _client.GetStringAsync("/heritage-sites?cat=Temples");
        Assert.Contains("Angkor Wat", html);
        Assert.DoesNotContain(">Petra<", html);
    }

    [Fact]
    public async Task Admin_AddsSite_AppearsOnPublicList()
    {
        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var token = await Http.TokenAsync(_client, "/admin/sites/new");
        var resp = await _client.PostAsync("/admin/sites", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["name"] = "Test Tower",
            ["slug"] = "test-tower",
            ["category"] = "Temples",
            ["location"] = "Test City",
            ["historicalBackground"] = "Built for the test suite.",
            ["architectureSignificance"] = "Its arches are load-bearing.",
            ["culturalBackground"] = "Holds one ceremony a year.",
            ["imagesCsv"] = "",
            ["factsText"] = "Fact one\nFact two",
            ["isAssociated"] = "false"
        }));
        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);

        var list = await _client.GetStringAsync("/heritage-sites");
        Assert.Contains("Test Tower", list);
    }

    [Fact]
    public async Task Admin_DeletesCategoryInUse_IsBlocked()
    {
        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var categories = await _db.Categories.Find(c => c.Name == "Temples").FirstOrDefaultAsync();
        Assert.NotNull(categories);

        var token = await Http.TokenAsync(_client, "/admin/categories");
        await _client.PostAsync($"/admin/categories/{categories.Id}/delete",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = token
            }));

        var page = await _client.GetStringAsync("/admin/categories");
        Assert.Contains("Cannot delete", page);
        Assert.Contains("Temples", page);
    }

    [Fact]
    public async Task Interest_Toggle_NeverDuplicates()
    {
        await Http.LoginAsync(_client, "demo@heritagetrail.local", "Demo123!");
        var ev = await _db.Events.Find(e => e.Slug == "temple-walk-angkor").FirstOrDefaultAsync();
        Assert.NotNull(ev);

        var token = await Http.TokenAsync(_client, $"/events/{ev.Slug}");
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        });

        // on
        await _client.PostAsync($"/events/{ev.Id}/interest", form);
        Assert.Equal(1, await _db.Interests.CountDocumentsAsync(i => i.EventId == ev.Id));

        // off (toggle removes)
        var token2 = await Http.TokenAsync(_client, $"/events/{ev.Slug}");
        await _client.PostAsync($"/events/{ev.Id}/interest",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token2 }));
        Assert.Equal(0, await _db.Interests.CountDocumentsAsync(i => i.EventId == ev.Id));

        // on again — never more than one record
        var token3 = await Http.TokenAsync(_client, $"/events/{ev.Slug}");
        await _client.PostAsync($"/events/{ev.Id}/interest",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token3 }));
        Assert.Equal(1, await _db.Interests.CountDocumentsAsync(i => i.EventId == ev.Id));
    }

    [Fact]
    public async Task Interest_PastEvent_IsRejected()
    {
        await Http.LoginAsync(_client, "demo@heritagetrail.local", "Demo123!");
        var ev = await _db.Events.Find(e => e.Slug == "ming-wall-walk").FirstOrDefaultAsync();
        Assert.NotNull(ev);

        var token = await Http.TokenAsync(_client, $"/events/{ev.Slug}");
        await _client.PostAsync($"/events/{ev.Id}/interest",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token }));

        var count = await _db.Interests.CountDocumentsAsync(i => i.EventId == ev.Id);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Invite_InvalidEmail_IsNotRecorded()
    {
        await Http.LoginAsync(_client, "demo@heritagetrail.local", "Demo123!");
        var token = await Http.TokenAsync(_client, "/heritage-sites/angkor-wat");
        var before = await _db.Invitations.CountDocumentsAsync(FilterDefinition<Invitation>.Empty);

        await _client.PostAsync("/heritage-sites/angkor-wat/invite", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["email"] = "not-an-email"
        }));

        var after = await _db.Invitations.CountDocumentsAsync(FilterDefinition<Invitation>.Empty);
        Assert.Equal(before, after);
    }

    [Fact]
    public async Task Query_AdminReply_IsVisibleToUser()
    {
        await Http.LoginAsync(_client, "demo@heritagetrail.local", "Demo123!");
        var token = await Http.TokenAsync(_client, "/account");
        await _client.PostAsync("/account/queries", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["subject"] = "Opening hours",
            ["message"] = "When does Angkor open on Mondays?"
        }));

        var query = await _db.Queries.Find(q => q.Subject == "Opening hours").FirstOrDefaultAsync();
        Assert.NotNull(query);

        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var adminToken = await Http.TokenAsync(_client, "/admin/queries");
        await _client.PostAsync($"/admin/queries/{query.Id}/reply", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = adminToken,
            ["reply"] = "Angkor opens daily at 07:30."
        }));

        var updated = await _db.Queries.Find(q => q.Id == query.Id).FirstOrDefaultAsync();
        Assert.Equal("Answered", updated?.Status);
        Assert.Contains("Angkor opens daily", updated?.Reply);

        await Http.LoginAsync(_client, "demo@heritagetrail.local", "Demo123!");
        var account = await _client.GetStringAsync("/account");
        Assert.Contains("Angkor opens daily", account);
    }

    [Fact]
    public async Task Event_EndBeforeStart_IsRejected()
    {
        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var before = await _db.Events.CountDocumentsAsync(FilterDefinition<Event>.Empty);
        var token = await Http.TokenAsync(_client, "/admin/events/new");

        var resp = await _client.PostAsync("/admin/events", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["title"] = "Broken Schedule",
            ["slug"] = "broken-schedule",
            ["type"] = "walk",
            ["description"] = "Ends before it starts.",
            ["startDate"] = "2026-01-03T10:00",
            ["endDate"] = "2026-01-02T10:00",
            ["venue"] = "Nowhere"
        }));
        Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);

        var after = await _db.Events.CountDocumentsAsync(FilterDefinition<Event>.Empty);
        Assert.Equal(before, after);

        var page = await _client.GetStringAsync("/admin/events");
        Assert.Contains("end date must be after the start date", page);
    }

    [Fact]
    public async Task Upload_Exe_IsRejected()
    {
        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var before = await _db.Gallery.CountDocumentsAsync(FilterDefinition<GalleryItem>.Empty);
        var token = await Http.TokenAsync(_client, "/admin/gallery");

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(token), "__RequestVerificationToken");
        content.Add(new StringContent("Evil upload"), "caption");
        content.Add(new StringContent("site"), "refType");
        content.Add(new ByteArrayContent(Encoding.ASCII.GetBytes("MZ fake exe")), "photo", "evil.exe");

        await _client.PostAsync("/admin/gallery", content);

        var after = await _db.Gallery.CountDocumentsAsync(FilterDefinition<GalleryItem>.Empty);
        Assert.Equal(before, after);

        var page = await _client.GetStringAsync("/admin/gallery");
        Assert.Contains("Only jpg, png and webp", page);
    }

    [Fact]
    public async Task Upload_Png_IsAcceptedAndServed()
    {
        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var token = await Http.TokenAsync(_client, "/admin/gallery");

        // 1x1 transparent PNG
        var png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(token), "__RequestVerificationToken");
        content.Add(new StringContent("Uploaded pixel"), "caption");
        content.Add(new StringContent("site"), "refType");
        content.Add(new ByteArrayContent(png), "photo", "pixel.png");

        await _client.PostAsync("/admin/gallery", content);

        var item = await _db.Gallery.Find(g => g.Caption == "Uploaded pixel").FirstOrDefaultAsync();
        Assert.NotNull(item);
        Assert.StartsWith("/uploads/", item.ImageUrl);

        var served = await _client.GetAsync(item.ImageUrl);
        Assert.Equal(HttpStatusCode.OK, served.StatusCode);
    }

    [Fact]
    public async Task MembersPage_DoesNotExposePasswordHashes()
    {
        await Http.LoginAsync(_client, "admin@heritagetrail.local", "Admin123!");
        var html = await _client.GetStringAsync("/admin/members");
        Assert.DoesNotContain("PasswordHash", html);
        Assert.DoesNotContain("AQAAAA", html);
    }
}
