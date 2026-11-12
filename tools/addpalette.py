import json, io
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Content\design\candidates.json"
data = json.load(open(p, encoding="utf-8"))
data["derivedPalette"] = {
    "note": "Derived by counting domain words in the 3 original English docs, then mapping each object to the colours that appear with it in reality (the docs contain no colour words).",
    "roles": [
        {"id": "bg", "role": "Base — giấy / đá vôi", "hex": "#F5EFE3", "source": "giấy tài liệu, đá vôi", "evidence": "heritage sites 17 · gallery 3 · image 3"},
        {"id": "surface", "role": "Surface", "hex": "#EAE4D5", "source": "đá vôi sáng", "evidence": "danh sách site, form"},
        {"id": "ink", "role": "Ink — mực than", "hex": "#241C14", "source": "mực in trên giấy", "evidence": "document 5 · history 2 · background 2"},
        {"id": "muted", "role": "Ink muted", "hex": "#6B6152", "source": "mực nhạt", "evidence": "meta, caption"},
        {"id": "border", "role": "Border", "hex": "#CFC7B6", "source": "đường kẻ đá", "evidence": "dividers"},
        {"id": "accent", "role": "Accent — sơn mài", "hex": "#A63A24", "source": "cửa đền sơn mài, đèn lồng", "evidence": "temple 1 · palace 1 · event 5 · programme 8"},
        {"id": "gold", "role": "Accent alt — vàng lá", "hex": "#C79A3F", "source": "vàng lá đền/cung điện", "evidence": "palace 1 · exhibition 1"},
        {"id": "moss", "role": "Support — rêu", "hex": "#5C6B4A", "source": "rêu trên gạch/đá", "evidence": "site 17 · architecture 3"},
        {"id": "onaccent", "role": "On accent", "hex": "#FFF8EC", "source": "giấy sáng", "evidence": "chữ trên nền accent"},
        {"id": "success", "role": "Success", "hex": "#4F7A4F", "source": "rêu", "evidence": "semantic"},
        {"id": "warning", "role": "Warning", "hex": "#C79A3F", "source": "vàng", "evidence": "semantic"},
        {"id": "danger", "role": "Danger", "hex": "#B3261E", "source": "sơn mài đậm", "evidence": "semantic"}
    ]
}
json.dump(data, open(p, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
print("updated candidates.json")
