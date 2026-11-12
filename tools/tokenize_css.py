import re
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()

# chip padding -> token
css = css.replace("padding: 3px 8px;", "padding: var(--ui-chip-padding);")

# hero-b overlay + text -> tokens/color-mix
css = css.replace(
    ".hero-b::after { content: \"\"; position: absolute; inset: 0; background: linear-gradient(180deg, rgb(20 26 23 / .08) 30%, rgb(20 26 23 / .88) 100%); }",
    ".hero-b::after { content: \"\"; position: absolute; inset: 0; background: linear-gradient(180deg, color-mix(in srgb, var(--color-ink) var(--ui-overlay-soft), transparent) 30%, color-mix(in srgb, var(--color-ink) var(--ui-overlay-strong), transparent) 100%); }")
css = css.replace(".hero-b h1 { color: #f2efe8; }", ".hero-b h1 { color: var(--color-bg); }")
css = css.replace(".hero-b .lede { color: rgb(242 239 232 / .85); }", ".hero-b .lede { color: color-mix(in srgb, var(--color-bg) 85%, transparent); }")
css = css.replace(".hero-b .eyebrow { color: #f2efe8; opacity: .85; }", ".hero-b .eyebrow { color: var(--color-bg); opacity: .85; }")
css = css.replace(".hero-b .btn-ghost { color: #f2efe8; border-color: rgb(242 239 232 / .55); }", ".hero-b .btn-ghost { color: var(--color-bg); border-color: color-mix(in srgb, var(--color-bg) 55%, transparent); }")

# tile-overlay caption -> tokens/color-mix
css = css.replace(
    ".tile-overlay figcaption { position: absolute; left: 12px; bottom: 10px; background: rgb(20 26 23 / .65); color: #f2efe8; padding: 4px 8px; font-size: var(--fs-micro); }",
    ".tile-overlay figcaption { position: absolute; left: var(--space-sm); bottom: var(--space-xs); background: color-mix(in srgb, var(--color-ink) 65%, transparent); color: var(--color-bg); padding: var(--ui-caption-padding); font-size: var(--fs-micro); }")

# base .table styles (admin)
table_css = """
.table { width: 100%; border-collapse: collapse; font-size: var(--fs-small); }
.table th { text-align: left; font-family: var(--font-mono); font-size: var(--fs-micro); letter-spacing: .1em; text-transform: uppercase; color: var(--color-ink-muted); padding: var(--space-xs) var(--ui-table-cell-padding); border-bottom: 1px solid var(--color-border); }
.table td { padding: var(--space-xs) var(--ui-table-cell-padding); border-bottom: 1px solid var(--color-border); }
"""
if ".table {" not in css:
    css += "\n" + table_css

open(p, "w", encoding="utf-8").write(css)
print("atoms.css tokenized; has .table:", ".table {" in css)
