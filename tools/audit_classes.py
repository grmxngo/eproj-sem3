import re, glob, os
web = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web"
css = open(os.path.join(web, "wwwroot", "css", "atoms.css"), encoding="utf-8").read()
classes = sorted(set(re.findall(r'\.([a-zA-Z][\w-]*)', css)))
texts = ""
for f in glob.glob(os.path.join(web, "Views", "**", "*.cshtml"), recursive=True):
    texts += open(f, encoding="utf-8").read()
for f in glob.glob(os.path.join(web, "wwwroot", "js", "*.js")):
    texts += open(f, encoding="utf-8").read()
unused = [c for c in classes if not re.search(r'\b' + re.escape(c) + r'\b', texts)]
print("UNUSED classes:", unused if unused else "none")
hexs = re.findall(r'#[0-9a-fA-F]{3,8}\b', css)
print("hardcoded hex in atoms.css:", len(hexs))
