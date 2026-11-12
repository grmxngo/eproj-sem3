import json
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
data = json.load(open(p, encoding="utf-8"))
base = {"bg": "#F5EFE3", "surface": "#EAE4D5", "surfaceRaised": "#E0D8C7", "ink": "#241C14", "inkMuted": "#6B6152", "border": "#CFC7B6", "success": "#4F7A4F", "warning": "#C79A3F", "danger": "#B3261E"}
mock = [
    {"id": "P1", "name": "Lacquer & Paper — sơn mài dẫn", "desc": "nút/link chính = sơn mài đỏ; vàng lá chỉ cho nhấn (event/số liệu)", "color": dict(base, accent="#A63A24", onAccent="#FFF8EC", accentAlt="#C79A3F")},
    {"id": "P2", "name": "Gold & Ink — vàng lá dẫn", "desc": "nút chính = vàng lá (chữ đậm); đỏ dành cho event/khẩn cấp", "color": dict(base, accent="#B8912F", onAccent="#241C14", accentAlt="#A63A24")},
    {"id": "P3", "name": "Moss & Paper — rêu dẫn", "desc": "nút chính = rêu (di sản ngoài trời); đỏ/vàng cho event", "color": dict(base, surface="#E7E6D8", surfaceRaised="#DCDBCB", ink="#1F241B", inkMuted="#5F6653", border="#C8C7B4", accent="#4F6243", onAccent="#FFF8EC", accentAlt="#A63A24")},
    {"id": "P4", "name": "Brick & Paper — gạch dẫn", "desc": "nút chính = gạch nung (tháp Chăm); vàng lá nhấn", "color": dict(base, bg="#F5EDE0", surface="#EADDCB", surfaceRaised="#DFD1BC", ink="#2A1D15", inkMuted="#75604E", border="#D2C2AC", accent="#B4552D", onAccent="#FFF8EC", accentAlt="#C79A3F")},
]
data["mockPalettes"] = {"note": "4 palette derived from the word-count objects, applied to the same mockup for in-context comparison.", "palettes": mock}
json.dump(data, open(p, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("added mockPalettes")
