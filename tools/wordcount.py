import re, collections, io

files = [
    r"C:\Users\me\Desktop\xem3\reference\conv\HeritageTrail-website.txt",
    r"C:\Users\me\Desktop\xem3\reference\spec.txt",
    r"C:\Users\me\Desktop\xem3\reference\HeritageTrail\Readme-Student Guide-eProject Process.txt",
]
stop = set("""a an the and or but if then else when while for to of in on at by with from as is are was were be been being it its this that these those which who whom whose what where why how not no nor so than then there their they them he she his her we our you your i me my mine will would can could should may might must do does did done have has had having also more most other some such only own same too very just any each few many much both all again further once here after before above below between into through during without within up down out off over under about against among because""".split())

def words(text):
    return [w for w in re.split(r"[^a-z0-9]+", text.lower()) if len(w) >= 2 and not w.isdigit()]

counts = collections.Counter()
for f in files:
    text = open(f, encoding="utf-8", errors="ignore").read()
    raw = collections.Counter(w for w in words(text) if w not in stop)
    for w, c in raw.items():
        lemma = w[:-1] if w.endswith("s") and w[:-1] in counts and len(w) > 3 else w
        counts[lemma] += c

top = counts.most_common(60)
out = io.StringIO()
out.write("TOP 60 content words (3 English project docs):\n")
for w, c in top:
    out.write(f"{c:4d}  {w}\n")

watch = ["heritage","site","monument","temple","palace","fort","museum","architecture","history","cultural","culture","event","programme","gallery","image","photo","stone","brick","temple","gold","landscape","nature","garden","river","mountain","city","ancient","traditional","wood","water","light","dark","red","green","yellow","brown","white","black","blue","colour","color"]
out.write("\nWATCH LIST counts:\n")
for w in watch:
    if counts.get(w):
        out.write(f"{counts[w]:4d}  {w}\n")

open(r"C:\Users\me\Desktop\xem3\artifacts\logs\wordcount.txt","w",encoding="utf-8").write(out.getvalue())
print("done")
