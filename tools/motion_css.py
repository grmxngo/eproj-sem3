p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\css\atoms.css"
css = open(p, encoding="utf-8").read()
add = """
/* ---------- motion layer ---------- */
[data-reveal] { opacity: 0; transform: translateY(14px); transition: opacity .55s var(--motion-ease), transform .55s var(--motion-ease); }
[data-revealed] { opacity: 1; transform: none; }
.detail-hero img, .detail-hero video, .lead-card img { animation: kenburns 22s ease-in-out infinite alternate; }
@keyframes kenburns { from { transform: scale(1); } to { transform: scale(1.07); } }
.nav-stub a { position: relative; }
.nav-stub a::after { content: ""; position: absolute; left: 0; right: 100%; bottom: -5px; height: 1px; background: var(--color-accent); transition: right .25s var(--motion-ease); }
.nav-stub a:hover::after, .nav-stub a[aria-current="page"]::after { right: 0; }
.btn:active { transform: translateY(1px) scale(.995); }
.marquee { overflow: hidden; border-top: 1px solid var(--color-border); border-bottom: 1px solid var(--color-border); padding: var(--space-xs) 0; background: var(--color-surface); }
.marquee-track { display: inline-flex; gap: var(--space-xl); white-space: nowrap; animation: marquee 34s linear infinite; font-family: var(--font-mono); font-size: var(--fs-micro); letter-spacing: .12em; text-transform: uppercase; color: var(--color-ink-muted); }
@keyframes marquee { to { transform: translateX(-50%); } }
.footage-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: var(--space-sm); }
.footage { margin: 0; }
.footage video { width: 100%; aspect-ratio: 16 / 9; object-fit: cover; display: block; border-radius: var(--radius-sm); background: var(--color-ink); }
.footage figcaption { font-family: var(--font-mono); font-size: var(--fs-micro); color: var(--color-ink-muted); margin-top: var(--space-2xs); }
.tile { overflow: hidden; }
.tile img { transition: transform .6s var(--motion-ease); }
.tile:hover img { transform: scale(1.04); }
@media (prefers-reduced-motion: reduce) {
  .marquee-track, .detail-hero img, .detail-hero video, .lead-card img { animation: none; }
  [data-reveal] { opacity: 1; transform: none; transition: none; }
}
"""
if "/* ---------- motion layer ---------- */" not in css:
    css += add
open(p, "w", encoding="utf-8").write(css)
print("motion css added")
