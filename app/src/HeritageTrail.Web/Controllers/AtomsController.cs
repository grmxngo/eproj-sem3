using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeritageTrail.Web.Controllers;

/// <summary>Atom gallery — staging only. All preview atoms are rendered from tokens.json (SSOT).</summary>
public sealed class AtomsController : Controller
{
    private readonly EnvironmentInfo _env;

    public AtomsController(EnvironmentInfo env) => _env = env;

    [HttpGet("/atoms")]
    public IActionResult Index()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        return View();
    }

    [HttpGet("/atoms/complete")]
    public IActionResult Complete()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        return View();
    }
}
