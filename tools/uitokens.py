import json
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\tokens.json"
t = json.load(open(p, encoding="utf-8"))
t["ui"] = {
    "chipPadding": "3px 8px",
    "inputPadding": "6px 10px",
    "captionPadding": "4px 8px",
    "tableCellPadding": "10px",
    "panelPadding": "clamp(1rem, 0.9rem + 0.5vw, 1.5rem)",
    "topbarPadding": "10px 24px",
    "sidebarWidth": "220px",
    "sideItemPadding": "8px 10px",
    "overlayStrong": "88%",
    "overlaySoft": "8%"
}
t["meta"]["version"] = "0.6.0-ui-tokens"
json.dump(t, open(p, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("ui tokens added")
