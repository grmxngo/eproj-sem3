import zipfile, io
from docx import Document

out = io.StringIO()
doc = Document(r"C:\Users\me\Desktop\xem3\PhanTichNghiepVu-HeritageTrail.docx")
body = doc.element.body
n_para = n_tbl = 0
seq = []
for child in body.iterchildren():
    tag = child.tag.split('}')[-1]
    if tag == 'p':
        n_para += 1
        t = ''.join(child.itertext()).strip()
        if t:
            seq.append(('P', t[:90]))
    elif tag == 'tbl':
        n_tbl += 1
        txts = [''.join(r.itertext()).strip()[:40] for r in child.iter('.//w:tr')][:2]
        seq.append(('T', ' || '.join(txts)))
out.write(f"analysis: {n_para} paragraphs, {n_tbl} tables\n")
for i, (k, t) in enumerate(seq):
    out.write(f"{i:3d} {k} {t}\n")

zp = zipfile.ZipFile(r"C:\Users\me\Desktop\xem3\src\HeritageTrail\Heritagetrail\Heritage-Project Specification.docx")
names = zp.namelist()
media = [n for n in names if n.startswith('word/media/')]
out.write("\nspec media: %r\n" % media)
for n in names:
    if n.endswith('.xml'):
        data = zp.read(n).decode('utf-8', 'ignore')
        if 'txbxContent' in data:
            out.write("TEXTBOX in %s\n" % n)
with open(r"C:\Users\me\Desktop\xem3\structure_check.txt", "w", encoding="utf-8") as f:
    f.write(out.getvalue())
print("done")
