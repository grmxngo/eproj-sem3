using HeritageTrail.Web.Models;
using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HeritageTrail.Web.Controllers;

public sealed class AuthController : Controller
{
    private readonly AuthService _auth;
    private readonly EnvironmentInfo _env;

    public AuthController(AuthService auth, EnvironmentInfo env)
    {
        _auth = auth;
        _env = env;
    }

    /// <summary>Staging-only quick sign-in for previews (never enabled in production).</summary>
    [HttpGet("/auth/dev-login")]
    public async Task<IActionResult> DevLogin(string? asUser = null, string? as_ = null, string? next = null)
    {
        if (!_env.IsStaging) return NotFound();
        var email = (asUser ?? as_ ?? "admin").Equals("admin", StringComparison.OrdinalIgnoreCase)
            ? "admin@heritagetrail.local"
            : "demo@heritagetrail.local";
        var user = await _auth.FindByEmailAsync(email);
        if (user is null) return NotFound();
        await SignInAsync(user);
        if (!string.IsNullOrWhiteSpace(next) && next.StartsWith('/')) return Redirect(next);
        return user.Role == "admin" ? Redirect("/admin") : Redirect("/account");
    }

    [HttpGet("/auth/register")]
    public IActionResult Register(string? form, string? auth)
    {
        var key = (form ?? "f1").ToLowerInvariant();
        ViewData["FormStyle"] = key is "f2" or "f3" ? key : "f1";
        var layout = (auth ?? "a1").ToLowerInvariant();
        ViewData["AuthLayout"] = layout is "a2" or "a3" ? layout : "a1";
        return View();
    }

    [HttpPost("/auth/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string name, string email, string password,
        string? phone, string? address, string? occupation, string? organization)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewData["Error"] = "Name, email and password are required.";
            return View();
        }
        if (password.Length < 8)
        {
            ViewData["Error"] = "Password must be at least 8 characters.";
            return View();
        }

        var (user, error) = await _auth.RegisterAsync(name, email, password, phone, address, occupation, organization);
        if (user is null)
        {
            ViewData["Error"] = error;
            return View();
        }

        await SignInAsync(user);
        return Redirect("/account");
    }

    [HttpGet("/auth/login")]
    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("/auth/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl)
    {
        var user = await _auth.ValidateAsync(email, password);
        if (user is null)
        {
            ViewData["Error"] = "Invalid email or password.";
            return View();
        }

        await SignInAsync(user);
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return user.Role == "admin" ? Redirect("/admin") : Redirect("/account");
        }
        return Redirect(returnUrl);
    }

    [HttpPost("/auth/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    private async Task SignInAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
    }
}
