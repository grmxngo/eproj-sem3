p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()
add = """
/* ---------- identity & focus ---------- */
.brand { display: inline-flex; align-items: center; gap: var(--space-2xs); color: inherit; text-decoration: none; }
.logo-mark { width: 26px; height: 26px; color: var(--color-accent); }
.mark-path { fill: none; stroke: currentColor; stroke-width: 3; stroke-linecap: round; stroke-dasharray: 120; stroke-dashoffset: 120; animation: mark-draw 1.1s var(--motion-slower) ease forwards .2s; }
@keyframes mark-draw { to { stroke-dashoffset: 0; } }
:focus-visible { outline: 2px solid var(--color-accent); outline-offset: 2px; }
"""
if ".logo-mark {" not in css:
    css += add
open(p, "w", encoding="utf-8").write(css)
print("identity css added")
