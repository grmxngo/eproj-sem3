p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()
add = """
/* ---------- interior page head (same grammar as home) ---------- */
.page-head { padding: var(--space-lg) 0 var(--space-md); border-bottom: 1px solid var(--color-border); margin-bottom: var(--space-lg); }
.page-head h1 { font-family: var(--font-display); font-size: var(--fs-h1); font-weight: 400; margin: var(--space-2xs) 0 var(--space-sm); }
.page-head .lede { color: var(--color-ink-muted); max-width: 62ch; margin: 0; }
.detail-media img { width: 100%; height: 100%; object-fit: cover; display: block; }
"""
if ".page-head {" not in css:
    css += add
open(p, "w", encoding="utf-8").write(css)
print("page-head css added")
