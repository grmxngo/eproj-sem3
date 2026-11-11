using System.Text.Json.Nodes;

namespace HeritageTrail.Web.Services;

/// <summary>Preview-only candidates for Batch 1 (colors/type). Promoted into tokens.json after user approval.</summary>
public sealed class CandidatesProvider
{
    private readonly JsonObject _root;

    public CandidatesProvider(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Content", "design", "candidates.json");
        Raw = File.ReadAllText(path);
        _root = JsonNode.Parse(Raw)!.AsObject();
    }

    public string Raw { get; }

    public JsonArray Palettes => _root["palettes"]!.AsArray();

    public JsonArray Typography => _root["typography"]!.AsArray();

    public JsonArray Radii => _root["radiusOptions"]!.AsArray();
}
