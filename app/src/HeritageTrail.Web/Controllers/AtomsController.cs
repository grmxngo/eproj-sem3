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

    [HttpGet("/atoms/batch1/typefaces")]
    public IActionResult Typefaces()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        return View("Batch1/Typefaces");
    }

    [HttpGet("/atoms/batch1/glyphs")]
    public IActionResult Glyphs()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["Typography"] = _candidates.Typography;
        return View("Batch1/Glyphs");
    }

    [HttpGet("/atoms/batch1/section/{id}")]
    public IActionResult Section(string id)
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        var palette = _candidates.MockPalettes.FirstOrDefault(p => p!["id"]!.GetValue<string>() == id);
        if (palette is null) return NotFound();
        ViewData["Mock"] = palette;
        return View("Batch1/Section");
    }

    [HttpGet("/atoms/batch1/palette-atoms")]
    public IActionResult PaletteAtoms()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["MockPalettes"] = _candidates.MockPalettes;
        return View("Batch1/PaletteAtoms");
    }

    [HttpGet("/atoms/batch1/page/{id}")]
    public IActionResult Page(string id)
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        var palette = _candidates.MockPalettes.FirstOrDefault(p => p!["id"]!.GetValue<string>() == id);
        if (palette is null) return NotFound();
        ViewData["Mock"] = palette;
        return View("Batch1/Page");
    }

    [HttpGet("/atoms/batch1/palettes")]
    public IActionResult Palettes()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["MockPalettes"] = _candidates.MockPalettes;
        return View("Batch1/Palettes");
    }

    [HttpGet("/atoms/site-card")]
    public IActionResult SiteCard()
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

    [HttpGet("/atoms/design/{id}")]
    public IActionResult Design(string id)
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        return id switch
        {
            "d1" => View("Design/D1"),
            "d2" => View("Design/D2"),
            "d3" => View("Design/D3"),
            "e1" => View("Design/E1"),
            "e2" => View("Design/E2"),
            "e3" => View("Design/E3"),
            "e4" => View("Design/E4"),
            "e5" => View("Design/E5"),
            "e6" => View("Design/E6"),
            _ => NotFound()
        };
    }

    [HttpGet("/atoms/batch1/mock/{id}")]
    public IActionResult Mock(string id)
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        var palette = _candidates.MockPalettes.FirstOrDefault(p => p!["id"]!.GetValue<string>() == id);
        if (palette is null) return NotFound();
        ViewData["Mock"] = palette;
        return View("Batch1/Mock");
    }

    [HttpGet("/atoms/batch1/derived")]
    public IActionResult Derived()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["Derived"] = _candidates.Derived;
        return View("Batch1/Derived");
    }

    [HttpGet("/atoms/batch1/base")]
    public IActionResult Base()
    {
        if (!_env.IsAtomsEnabled) return NotFound();
        ViewData["Bases"] = _candidates.Bases;
        return View("Batch1/Base");
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
