import json
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
data = json.load(open(p, encoding="utf-8"))
pals = [
    {"id": "P1", "name": "Sơn mài trầm", "desc": "giấy ấm + đỏ sơn mài đậm, event band tối", "color": {"bg": "#F5EDDD", "surface": "#EADCC3", "surfaceRaised": "#DCC9A8", "ink": "#1E140E", "inkMuted": "#6A563F", "border": "#C9B490", "accent": "#9A2B14", "onAccent": "#FFF3E4", "accentAlt": "#C08A2E", "success": "#3F6E3A", "warning": "#B07A26", "danger": "#7E1A10"}},
    {"id": "P2", "name": "Mực & đá", "desc": "nền đá lạnh + navy đậm, đồng cho nhấn", "color": {"bg": "#EAEFF0", "surface": "#DCE4E6", "surfaceRaised": "#CBD7DA", "ink": "#101A21", "inkMuted": "#53646C", "border": "#B2C3C7", "accent": "#1B4D6B", "onAccent": "#F2F8FA", "accentAlt": "#B36A2C", "success": "#2F6B55", "warning": "#B36A2C", "danger": "#8E2A20"}},
    {"id": "P3", "name": "Rêu đậm", "desc": "nền rêu nhạt + rêu đậm, đất nung cho event", "color": {"bg": "#E9F0DF", "surface": "#D9E5C9", "surfaceRaised": "#C7D6B2", "ink": "#121B0F", "inkMuted": "#566449", "border": "#A8BC93", "accent": "#2F4A2A", "onAccent": "#F5FAEE", "accentAlt": "#C25B2E", "success": "#356B33", "warning": "#B07A26", "danger": "#8E2415"}},
    {"id": "P4", "name": "Gạch & nắng", "desc": "nền cát + gạch nung đậm, teal sông cho event", "color": {"bg": "#F4E6CF", "surface": "#E7D3B4", "surfaceRaised": "#D5BE99", "ink": "#1F1209", "inkMuted": "#745C42", "border": "#C0A67E", "accent": "#A63014", "onAccent": "#FFF4E8", "accentAlt": "#175B54", "success": "#3F6E3A", "warning": "#B07A26", "danger": "#7E1A10"}}
]
data["mockPalettes"] = {"note": "Bolder editorial palettes: high contrast paper, saturated deep accent, events band in ink.", "palettes": pals}
json.dump(data, open(p, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("bolder palettes set")
