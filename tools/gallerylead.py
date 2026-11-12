p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()
add = """
.gallery-lead .tile:first-child { grid-column: span 2; }
"""
if ".gallery-lead" not in css:
    css += add
open(p, "w", encoding="utf-8").write(css)
print("gallery lead css added")
