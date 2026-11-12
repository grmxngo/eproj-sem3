import json
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
data = json.load(open(p, encoding="utf-8"))
pals = [
    {
        "id": "P1", "name": "Sơn mài trên giấy — ấm",
        "desc": "nền giấy dó ấm + chữ nâu đen; đỏ sơn mài bão hoà cho event",
        "color": {"bg": "#F3EADA", "surface": "#EADFCB", "surfaceRaised": "#DFD2B6", "ink": "#241A11", "inkMuted": "#6B5B45", "border": "#D2C3A6", "accent": "#A8321C", "onAccent": "#FFF6EA", "accentAlt": "#C08A2E", "success": "#4E7A47", "warning": "#C08A2E", "danger": "#8E1F12"}
    },
    {
        "id": "P2", "name": "Đá xanh — bảo tàng",
        "desc": "nền đá lạnh hơi xanh + chữ xanh đen; navy đậm + đồng cho event",
        "color": {"bg": "#EDF1F1", "surface": "#E0E7E7", "surfaceRaised": "#D3DCDD", "ink": "#16202A", "inkMuted": "#5A6A74", "border": "#BFCCCE", "accent": "#23415F", "onAccent": "#F2F7F7", "accentAlt": "#A8823B", "success": "#3D7A5C", "warning": "#A8823B", "danger": "#98392F"}
    },
    {
        "id": "P3", "name": "Rêu — ngoài trời",
        "desc": "nền xanh rêu nhạt + chữ xanh rêu đậm; rêu đậm + đất nung cho event",
        "color": {"bg": "#E9F0E2", "surface": "#DDE7D2", "surfaceRaised": "#CFDCC2", "ink": "#1A2316", "inkMuted": "#5C684F", "border": "#B9C9AA", "accent": "#3D5A34", "onAccent": "#F7FAF1", "accentAlt": "#B4552D", "success": "#3F7A3F", "warning": "#B8862F", "danger": "#A33022"}
    },
    {
        "id": "P4", "name": "Gạch nung — nắng",
        "desc": "nền cát ấm + gạch nung đậm; teal sông cho event tương phản",
        "color": {"bg": "#F2E5D2", "surface": "#E7D5BC", "surfaceRaised": "#DAC6A6", "ink": "#2A1B10", "inkMuted": "#75604A", "border": "#C9B294", "accent": "#B0391F", "onAccent": "#FFF5EC", "accentAlt": "#1E5B57", "success": "#4A7A4A", "warning": "#B8862F", "danger": "#93291A"}
    },
]
data["mockPalettes"] = {"note": "4 complete palettes: base/surface/ink/accent all change, not just the button colour.", "palettes": pals}
json.dump(data, open(p, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("replaced mockPalettes with 4 complete palettes")
