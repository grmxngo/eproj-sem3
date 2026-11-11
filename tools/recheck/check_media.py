import zipfile, io
out = io.StringIO()
z = zipfile.ZipFile(r"C:\Users\me\Desktop\xem3\PhanTichNghiepVu-HeritageTrail.docx")
for n in z.namelist():
    if n.startswith('word/media/'):
        out.write("ANALYSIS media: %s\n" % n)
out.write("ANALYSIS media total: %d\n" % len([n for n in z.namelist() if n.startswith('word/media/')]))
with open(r"C:\Users\me\Desktop\xem3\media_check.txt", "w", encoding="utf-8") as f:
    f.write(out.getvalue())
print("done")
