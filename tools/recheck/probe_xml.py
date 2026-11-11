import zipfile, io, re
z = zipfile.ZipFile(r"C:\Users\me\Desktop\xem3\PhanTichNghiepVu-HeritageTrail.docx")
xml = z.read('word/document.xml').decode('utf-8', 'ignore')
# find the paragraph containing T04
i = xml.find('T04')
seg = xml[i-200:i+2000]
out = io.StringIO()
out.write("segment:\n" + seg + "\n")
# count w:t occurrences containing T04 and other patterns
m = re.findall(r'<w:t[^>]*>([^<]*)</w:t>', xml)
joined = "".join(m)
out.write("\n\n--- first 200 w:t values ---\n")
for t in m[:200]:
    out.write(repr(t) + "\n")
out.write("\nT04 count in all w:t texts: %d\n" % m.count('T04'))
with open(r"C:\Users\me\Desktop\xem3\xml_probe.txt", "w", encoding="utf-8") as f:
    f.write(out.getvalue())
print("done")
