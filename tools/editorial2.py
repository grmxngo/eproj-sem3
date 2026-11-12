p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()
add = """
/* ---------- editorial v2 ---------- */
.lead-card { position: relative; display: block; margin: var(--space-md) 0 var(--space-lg); border-radius: var(--radius-md); overflow: hidden; color: var(--color-bg); }
.lead-card img { width: 100%; height: 420px; object-fit: cover; display: block; }
.lead-card::after { content: ""; position: absolute; inset: 0; background: linear-gradient(180deg, color-mix(in srgb, var(--color-ink) var(--ui-overlay-soft), transparent) 20%, color-mix(in srgb, var(--color-ink) var(--ui-overlay-strong), transparent) 100%); }
.lead-overlay { position: absolute; left: var(--space-lg); right: var(--space-lg); bottom: var(--space-lg); z-index: 2; }
.lead-overlay .eyebrow { color: var(--color-bg); opacity: .9; }
.lead-title { font-family: var(--font-display); font-size: var(--fs-h1); font-weight: 400; margin: 0 0 var(--space-xs); }
.lead-fact { color: color-mix(in srgb, var(--color-bg) 85%, transparent); max-width: 60ch; margin: 0; font-size: var(--fs-body); }
.detail-hero { position: relative; height: 420px; border-radius: var(--radius-md); overflow: hidden; margin-bottom: var(--space-lg); }
.detail-hero img { width: 100%; height: 100%; object-fit: cover; display: block; }
.detail-hero::after { content: ""; position: absolute; inset: 0; background: linear-gradient(180deg, color-mix(in srgb, var(--color-ink) var(--ui-overlay-soft), transparent) 25%, color-mix(in srgb, var(--color-ink) var(--ui-overlay-strong), transparent) 100%); }
.detail-hero .hero-copy { position: absolute; left: var(--space-lg); right: var(--space-lg); bottom: var(--space-lg); z-index: 2; }
.detail-hero .eyebrow { color: var(--color-bg); opacity: .9; }
.detail-hero .display { color: var(--color-bg); margin: 0; }
.site-facts-dark { background: var(--color-ink); color: var(--color-bg); border-color: var(--color-ink); }
.site-facts-dark .eyebrow { color: var(--color-accent-alt); }
.site-facts-dark .auth-list { color: color-mix(in srgb, var(--color-bg) 75%, transparent); }
.site-facts-dark .auth-list li { border-bottom-color: color-mix(in srgb, var(--color-bg) 20%, transparent); }
.site-row .thumb { width: 150px; height: 100px; }
"""
if ".lead-card {" not in css:
    css += add
open(p, "w", encoding="utf-8").write(css)
print("editorial v2 css added")
