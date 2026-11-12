p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()
add = """
/* ---------- editorial index + detail ---------- */
.site-index { margin-top: var(--space-md); }
.site-row { display: grid; grid-template-columns: 48px 1fr 120px; gap: var(--space-md); align-items: center; padding: var(--space-md) 0; border-top: 1px solid var(--color-border); color: inherit; text-decoration: none; }
.site-row:last-of-type { border-bottom: 1px solid var(--color-border); }
.site-row:hover .row-title { color: var(--color-accent); }
.site-row .num { font-family: var(--font-mono); font-size: var(--fs-small); color: var(--color-accent); }
.site-row .row-title { font-family: var(--font-display); font-size: var(--fs-h2); font-weight: 400; margin: 2px 0 0; transition: color var(--motion-fast) var(--motion-ease); }
.site-row .thumb { width: 120px; height: 84px; overflow: hidden; border-radius: var(--radius-sm); }
.site-row .thumb img { width: 100%; height: 100%; object-fit: cover; display: block; }
.detail-cols { display: grid; grid-template-columns: 1.15fr .85fr; gap: var(--space-xl); align-items: start; margin-top: var(--space-md); }
.site-facts { border: 1px solid var(--color-border); background: var(--color-surface); padding: var(--space-md); position: sticky; top: var(--space-md); }
.sec-num { font-family: var(--font-mono); font-size: var(--fs-micro); color: var(--color-accent); margin-right: var(--space-xs); }
"""
if ".site-index {" not in css:
    css += add
open(p, "w", encoding="utf-8").write(css)
print("editorial css added")
