import json
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
data = json.load(open(p, encoding="utf-8"))
data["homePalettes"] = {
  "note": "Three complete-home palettes. Same structure (real data); only token colours change via ?palette=hN.",
  "palettes": [
    {"id": "H1", "name": "Night museum", "desc": "charcoal + cream + vermilion", "color": {
      "bg": "#111111", "surface": "#1b1b1b", "surfaceRaised": "#262626", "ink": "#f2ece1", "inkMuted": "#a89f90",
      "border": "#3a3a3a", "accent": "#d6452e", "onAccent": "#ffffff", "accentAlt": "#d9a441",
      "success": "#6fae6f", "warning": "#d9a441", "danger": "#b5311f"}},
    {"id": "H2", "name": "Deep forest", "desc": "deep green + cream + antique gold", "color": {
      "bg": "#0f2a22", "surface": "#15362c", "surfaceRaised": "#1c4437", "ink": "#eef0e2", "inkMuted": "#9fb3a6",
      "border": "#2e5347", "accent": "#e0a53c", "onAccent": "#1a1408", "accentAlt": "#d97a3f",
      "success": "#7fae7f", "warning": "#e0a53c", "danger": "#c0452f"}},
    {"id": "H3", "name": "Cool archive", "desc": "cool grey-green paper + rust + ink", "color": {
      "bg": "#eef0ea", "surface": "#e2e6dd", "surfaceRaised": "#d5dacf", "ink": "#141a17", "inkMuted": "#5d6a62",
      "border": "#b9c2b6", "accent": "#b5311f", "onAccent": "#ffffff", "accentAlt": "#2f6b5e",
      "success": "#3f6e3a", "warning": "#b07a26", "danger": "#7e1a10"}}
  ]
}
json.dump(data, open(p, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("homePalettes added")
