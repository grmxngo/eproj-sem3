import re
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Views\Home\Index.cshtml"
s = open(p, encoding="utf-8").read()
# remove home-quick stat blocks (all three hero variants)
s = re.sub(r'\s*<div class="home-quick">.*?</div>\s*', "\n", s, flags=re.S)
s = re.sub(r'\s*<section class="hero-quick">.*?</section>\s*', "\n", s, flags=re.S)
# remove numeric counts in links
s = s.replace('Explore all @sites.Count →', 'Explore all sites →')
open(p, "w", encoding="utf-8").write(s)
print("counts removed:", s.count("home-quick"))
