import sys
from docx import Document

def dump(path, out):
    doc = Document(path)
    lines = []
    for p in doc.paragraphs:
        t = p.text.strip()
        if t:
            lines.append(t)
    for i, tbl in enumerate(doc.tables):
        lines.append(f"=== TABLE {i+1} ===")
        for row in tbl.rows:
            cells = [c.text.strip().replace("\n", " / ") for c in row.cells]
            lines.append(" | ".join(cells))
    with open(out, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print(f"wrote {out}: {len(lines)} lines")

dump(r"C:\Users\me\Desktop\xem3\src\HeritageTrail\Heritagetrail\Heritage-Project Specification.docx",
     r"C:\Users\me\Desktop\xem3\spec.txt")
dump(r"C:\Users\me\Desktop\xem3\PhanTichNghiepVu-HeritageTrail.docx",
     r"C:\Users\me\Desktop\xem3\phantich.txt")
