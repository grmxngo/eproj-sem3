import re, glob, os
web = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web"
css = open(os.path.join(web, "wwwroot", "css", "atoms.css"), encoding="utf-8").read()
classes = sorted(set(re.findall(r'\.([a-zA-Z][\w-]*)', css)))
# usage across views + js
texts = ""
for f in glob.glob(os.path.join(web, "Views", "**", "*.cshtml"), recursive=True):
    texts += open(f, encoding="utf-8").read()
for f in glob.glob(os.path.join(web, "wwwroot", "js", "*.js")):
    texts += open(f, encoding="utf-8").read()
unused = []
for c in classes:
    # class token used if appears as word in text (allow 'class="...c..."' and JS selectors)
    if not re.search(r'\b' + re.escape(c) + r'\b', texts):
        unused.append(c)
print("classes defined:", len(classes))
print("UNUSED:", len(unused))
for c in unused:
    print(c)
