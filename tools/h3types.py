import json
# 1) tokens.json -> H3 palette
tp = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\tokens.json"
t = json.load(open(tp, encoding="utf-8"))
h3 = {"bg": "#eef0ea", "surface": "#e2e6dd", "surfaceRaised": "#d5dacf", "ink": "#141a17", "inkMuted": "#5d6a62",
      "border": "#b9c2b6", "accent": "#b5311f", "onAccent": "#ffffff", "accentAlt": "#2f6b5e",
      "success": "#3f6e3a", "warning": "#b07a26", "danger": "#7e1a10"}
t["color"] = h3
t["meta"]["version"] = "0.3.0-h3-cool-archive"
t["meta"]["status"] = "approved-palette"
t["meta"]["note"] = "Palette H3 (Cool archive) approved by user. Typography pending approval."
json.dump(t, open(tp, "w", encoding="utf-8"), ensure_ascii=False, indent=2)

# 2) candidates.json -> typeSets
cp = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
c = json.load(open(cp, encoding="utf-8"))
c["typeSets"] = {
  "note": "Typography systems applied to the complete homepage for comparison.",
  "sets": [
    {"id": "T1", "name": "Classical editorial", "desc": "serif display + sans body",
     "display": "'Palatino Linotype','Book Antiqua',Georgia,serif",
     "body": "'Segoe UI','Segoe UI Variable Text',system-ui,sans-serif",
     "mono": "Consolas,'Cascadia Mono',monospace"},
    {"id": "T2", "name": "Museum modern", "desc": "grotesk display + sans body",
     "display": "'Bahnschrift','Segoe UI Variable Display','Segoe UI',sans-serif",
     "body": "'Segoe UI','Segoe UI Variable Text',system-ui,sans-serif",
     "mono": "Consolas,'Cascadia Mono',monospace"},
    {"id": "T3", "name": "Signage condensed", "desc": "condensed display + humanist sans body",
     "display": "'Franklin Gothic Medium','Arial Narrow',sans-serif",
     "body": "'Trebuchet MS','Segoe UI',system-ui,sans-serif",
     "mono": "Consolas,'Cascadia Mono',monospace"}
  ]
}
json.dump(c, open(cp, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("tokens=H3, typeSets added")
