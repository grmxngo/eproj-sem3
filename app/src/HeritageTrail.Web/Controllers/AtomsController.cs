using HeritageTrail.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace HeritageTrail.Web.Controllers;

/// <summary>Atom gallery — staging only. Each URL is a screenshot target for chat verification.</summary>
public sealed class AtomsController : Controller
{
    private readonly EnvironmentInfo _env;
    private readonly CandidatesProvider _candidates;

    public AtomsController(EnvironmentInfo env, CandidatesProvider candidates)
    {
        _env = env;
        _candidates = candidates;
    }

    [HttpGet("/atoms")]
    public IActionResult Index()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["Palettes"] = _candidates.Palettes;
        ViewData["Typography"] = _candidates.Typography;
        return View();
    }

    [HttpGet("/atoms/batch1/palette/{id}")]
    public IActionResult Palette(string id)
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        var palette = _candidates.Palettes.FirstOrDefault(p => p!["id"]!.GetValue<string>() == id);
        if (palette is null) return NotFound();
        ViewData["Palette"] = palette;
        return View("Batch1/Palette");
    }

    [HttpGet("/atoms/batch1/type")]
    public IActionResult Type()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["Typography"] = _candidates.Typography;
        return View("Batch1/Type");
    }

    [HttpGet("/atoms/batch1/tokens")]
    public IActionResult Tokens()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["Radii"] = _candidates.Radii;
        return View("Batch1/Tokens");
    }

    [HttpGet("/atoms/batch1/clamp")]
    public IActionResult Clamp()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        return View("Batch1/Clamp");
    }
}
