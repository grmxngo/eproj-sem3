import zipfile, io, re
out = io.StringIO()
for label, path in [("ANALYSIS", r"C:\Users\me\Desktop\xem3\PhanTichNghiepVu-HeritageTrail.docx"),
                    ("SPEC", r"C:\Users\me\Desktop\xem3\src\HeritageTrail\Heritagetrail\Heritage-Project Specification.docx")]:
    z = zipfile.ZipFile(path)
    docxml = z.read('word/document.xml').decode('utf-8', 'ignore')
    for tag in ['w:ins', 'w:del', 'w:delText', 'w:commentReference', 'w:comment']:
        out.write(f"{label}: count of <{tag}> = {docxml.count('<'+tag)} {docxml.count('<'+tag+' ')}\n")
    n = z.namelist()
    out.write(f"{label}: parts = {[x for x in n if 'comment' in x or 'revision' in x.lower()]}\n")
with open(r"C:\Users\me\Desktop\xem3\rev_check.txt", "w", encoding="utf-8") as f:
    f.write(out.getvalue())
print("done")
