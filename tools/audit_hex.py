import re, glob
hard = []
for f in glob.glob(r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Views\**\*.cshtml", recursive=True):
    s = open(f, encoding="utf-8").read()
    for m in re.finditer(r'#[0-9a-fA-F]{3,8}\b', s):
        hard.append((f.split("HeritageTrail.Web\\")[-1], m.group(0)))
for f in glob.glob(r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\*.css"):
    s = open(f, encoding="utf-8").read()
    for m in re.finditer(r'#[0-9a-fA-F]{3,8}\b', s):
        hard.append((f.split("wwwroot\\")[-1], m.group(0)))
print("HARDCODED HEX (views + css):", len(hard))
from collections import Counter
for f, h in hard[:60]:
    print(f, h)
