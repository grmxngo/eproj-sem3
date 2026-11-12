import glob, re
count = 0
for f in glob.glob(r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Views\Admin\*.cshtml"):
    if f.endswith("_AdminNav.cshtml") or f.endswith("_ViewStart.cshtml"):
        continue
    s = open(f, encoding="utf-8").read()
    s2 = re.sub(r'^@await Html\.PartialAsync\("_AdminNav"\)\s*\n', "", s, flags=re.M)
    if s2 != s:
        open(f, "w", encoding="utf-8").write(s2)
        count += 1
print("removed admin nav partial from", count, "views")
