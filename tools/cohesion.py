import re
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()

def sub(pattern, repl, count=1):
    global css
    css, n = re.subn(pattern, repl, css, count=count)
    return n

# 1) mono labels everywhere
sub(r"\.eyebrow \{ font-size: var\(--fs-micro\);", ".eyebrow { font-family: var(--font-mono); font-size: var(--fs-micro);")
sub(r"\.card-kicker \{ font-size: var\(--fs-micro\);", ".card-kicker { font-family: var(--font-mono); font-size: var(--fs-micro);")
sub(r"\.band-date \{ font-size: var\(--fs-micro\);", ".band-date { font-family: var(--font-mono); font-size: var(--fs-micro);")
sub(r"\.event-month \{ font-size: var\(--fs-micro\);", ".event-month { font-family: var(--font-mono); font-size: var(--fs-micro);")
sub(r"\.tile figcaption \{ padding: var\(--space-xs\) var\(--space-sm\); font-size: var\(--fs-micro\);", ".tile figcaption { font-family: var(--font-mono); padding: var(--space-xs) var(--space-sm); font-size: var(--fs-micro);")
sub(r"figcaption \{ margin-top: var\(--space-xs\); font-size: var\(--fs-micro\);", "figcaption { font-family: var(--font-mono); margin-top: var(--space-xs); font-size: var(--fs-micro);")

# 2) chips become sharp mono tags
sub(r"\.chip \{\n    display: inline-flex; align-items: center;\n    padding: var\(--space-2xs\) var\(--space-sm\);\n    border: 1px solid var\(--color-border\); border-radius: var\(--radius-pill\);",
    ".chip {\n    display: inline-flex; align-items: center;\n    padding: 3px 8px;\n    border: 1px solid var(--color-border); border-radius: var(--radius-sm);\n    font-family: var(--font-mono); font-size: var(--fs-micro); letter-spacing: .08em; text-transform: uppercase;")

# 3) section heads get a rule
sub(r"\.section-head \{ display: flex; justify-content: space-between; align-items: baseline; gap: var\(--space-md\); margin: var\(--space-xl\) 0 var\(--space-md\); \}",
    ".section-head { display: flex; justify-content: space-between; align-items: baseline; gap: var(--space-md); margin: var(--space-xl) 0 var(--space-md); padding-bottom: var(--space-xs); border-bottom: 1px solid var(--color-border); }")
sub(r"\.band-head \{ display: flex; justify-content: space-between; align-items: baseline; margin-bottom: var\(--space-md\); \}",
    ".band-head { display: flex; justify-content: space-between; align-items: baseline; margin-bottom: var(--space-md); padding-bottom: var(--space-xs); border-bottom: 1px solid color-mix(in srgb, var(--color-bg) 25%, transparent); }")

open(p, "w", encoding="utf-8").write(css)
print("cohesion pass applied")
