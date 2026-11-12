import re, collections, io

files = [
    r"C:\Users\me\Desktop\xem3\reference\conv\HeritageTrail-website.txt",
    r"C:\Users\me\Desktop\xem3\reference\spec.txt",
    r"C:\Users\me\Desktop\xem3\reference\HeritageTrail\Readme-Student Guide-eProject Process.txt",
]
stop = set("""a an the and or but if then else when while for to of in on at by with from as is are was were be been being it its this that these those which who whom whose what where why how not no nor so than then there their they them he she his her we our you your i me my will would can could should may might must do does did done have has had having also more most other some such only own same too very just any each few many much both all again further once here after before above below between into through during without within up down out off over under about against among because""".split())

all_words = collections.Counter()
for f in files:
    text = open(f, encoding="utf-8", errors="ignore").read().lower()
    for w in re.split(r"[^a-z0-9]+", text):
        if len(w) >= 2 and not w.isdigit() and w not in stop:
            all_words[w] += 1

out = io.StringIO()
out.write("UNIQUE CONTENT WORDS (all 3 docs), alphabetical:\n")
for w in sorted(all_words):
    out.write(f"{w} ({all_words[w]})\n")
out.write(f"\nTOTAL unique content words: {len(all_words)}\n")

design_words = ["design","designing","designed","look","interface","gui","layout","colour","color","font","image","images","photo","menu","menus","page","pages","link","links","sublink","button","icon","logo","nav","navigation","screen","layout","style","theme"]
out.write("\nDESIGN-RELATED WORDS PRESENT:\n")
for w in design_words:
    if w in all_words:
        out.write(f"{w} ({all_words[w]})\n")
open(r"C:\Users\me\Desktop\xem3\artifacts\logs\all-words.txt","w",encoding="utf-8").write(out.getvalue())
print("done")
