using System.Text.Json.Nodes;

namespace HeritageTrail.Web.Services;

/// <summary>Staging-only preview alternatives (home page: ?palette / ?type / ?radius). Locked values live in tokens.json.</summary>
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

    public JsonArray HomePalettes => _root["homePalettes"]!["palettes"]!.AsArray();

    public JsonArray TypeSets => _root["typeSets"]!["sets"]!.AsArray();

    public JsonArray RadiusSets => _root["radiusSets"]!["sets"]!.AsArray();
}
