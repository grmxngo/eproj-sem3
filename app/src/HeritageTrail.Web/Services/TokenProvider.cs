using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace HeritageTrail.Web.Services;

/// <summary>
/// Reads Content/design/tokens.json — the single source of truth for every design value.
/// Emits CSS custom properties and exposes the raw JSON for /design/tokens.json.
/// </summary>
public sealed partial class TokenProvider
{
    private readonly JsonObject _root;

    public TokenProvider(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Content", "design", "tokens.json");
        Raw = File.ReadAllText(path);
        _root = JsonNode.Parse(Raw)!.AsObject();
    }

    public string Raw { get; }

    public JsonNode? Node(string name) => _root[name];

    public string ToCss()
    {
        var sb = new StringBuilder(":root{");
        AddGroup(sb, "color-", _root["color"]?.AsObject());
        AddGroup(sb, "font-", _root["fonts"]?.AsObject());
        AddGroup(sb, "fs-", _root["fluid"]?["fs"]?.AsObject());
        AddGroup(sb, "space-", _root["fluid"]?["space"]?.AsObject());
        AddGroup(sb, "radius-", _root["radius"]?.AsObject());
        AddGroup(sb, "shadow-", _root["shadow"]?.AsObject());
        AddGroup(sb, "motion-", _root["motion"]?.AsObject());
        AddGroup(sb, "ui-", _root["ui"]?.AsObject());

        if (_root["layout"] is JsonObject layout)
        {
            AppendVar(sb, "--gutter", layout["gutter"]);
            AppendVar(sb, "--maxw", layout["maxWidth"]);
            AppendVar(sb, "--grid-min", layout["gridMin"]);
        }

        sb.Append('}');
        return sb.ToString();
    }

    private static void AddGroup(StringBuilder sb, string prefix, JsonObject? group)
    {
        if (group is null)
        {
            return;
        }

        foreach (var (key, value) in group)
        {
            AppendVar(sb, "--" + prefix + CssNaming.Kebab(key), value);
        }
    }

    private static void AppendVar(StringBuilder sb, string name, JsonNode? value)
    {
        if (value is JsonValue jsonValue)
        {
            sb.Append(name).Append(':').Append(RawValue(jsonValue)).Append(';');
        }
    }

    private static string RawValue(JsonValue value) =>
        value.GetValueKind() == System.Text.Json.JsonValueKind.String
            ? value.GetValue<string>()
            : value.ToJsonString();
}

public static partial class CssNaming
{
    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex UpperRegex();

    /// <summary>surfaceRaised -> surface-raised (used for CSS custom property names).</summary>
    public static string Kebab(string name) => UpperRegex().Replace(name, "-$1").ToLowerInvariant();
}
