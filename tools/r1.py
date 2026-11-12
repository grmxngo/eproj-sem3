import json
tp = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\tokens.json"
t = json.load(open(tp, encoding="utf-8"))
t["radius"] = {"sm": "2px", "md": "4px", "lg": "6px", "pill": "999px"}
t["meta"]["version"] = "0.5.0-h3-t1-r1"
t["meta"]["note"] = "Palette H3, typography T1, radius R1 approved. Density pending."
json.dump(t, open(tp, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("radius R1 in tokens")
