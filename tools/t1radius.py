import json
# tokens -> T1 fonts
tp = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\tokens.json"
t = json.load(open(tp, encoding="utf-8"))
t["fonts"] = {
    "display": "'Palatino Linotype','Book Antiqua',Georgia,serif",
    "body": "'Segoe UI','Segoe UI Variable Text',system-ui,sans-serif",
    "mono": "Consolas,'Cascadia Mono',monospace"
}
t["meta"]["version"] = "0.4.0-h3-t1"
t["meta"]["note"] = "Palette H3 + typography T1 approved by user. Radius pending."
json.dump(t, open(tp, "w", encoding="utf-8"), ensure_ascii=False, indent=2)

# radius sets
cp = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
c = json.load(open(cp, encoding="utf-8"))
c["radiusSets"] = {
  "note": "Radius systems applied to the complete home page.",
  "sets": [
    {"id": "R1", "name": "Sharp heritage", "desc": "near-square corners", "sm": "2px", "md": "4px", "lg": "6px", "pill": "999px"},
    {"id": "R2", "name": "Soft modern", "desc": "rounded but restrained", "sm": "6px", "md": "12px", "lg": "20px", "pill": "999px"},
    {"id": "R3", "name": "Organic", "desc": "large soft corners", "sm": "10px", "md": "18px", "lg": "28px", "pill": "999px"}
  ]
}
json.dump(c, open(cp, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("T1 fonts + radiusSets")
