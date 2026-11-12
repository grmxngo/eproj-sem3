import re, collections, io
files = [
    r"C:\Users\me\Desktop\xem3\reference\conv\HeritageTrail-website.txt",
    r"C:\Users\me\Desktop\xem3\reference\spec.txt",
    r"C:\Users\me\Desktop\xem3\reference\HeritageTrail\Readme-Student Guide-eProject Process.txt",
]
stop = set("""a an the and or but if then else when while for to of in on at by with from as is are was were be been being it its this that these those which who whom whose what where why how not no nor so than then there their they them he she his her we our you your i me my will would can could should may might must do does did done have has had having also more most other some such only own same too very just any each few many much both all again further once here after before above below between into through during without within up down out off over under about against among because""".split())
counts = collections.Counter()
for f in files:
    text = open(f, encoding="utf-8", errors="ignore").read().lower()
    for w in re.split(r"[^a-z0-9]+", text):
        if len(w) >= 2 and not w.isdigit() and w not in stop:
            counts[w] += 1

design_nouns = ["design","designing","designed","look","interface","gui","navigation","menu","menus","page","pages","link","links","sublink","image","images","photo","layout","colour","color","font","size","spacing","button","icon","logo","theme","style","screen","header","footer","card","table","animation","section"]
scope_nouns = ["website","site","sites","heritage","admin","user","users","module","modules","member","members","event","events","programme","programmes","gallery","query","queries","interest","mail","email","category","categories","contact","about","member","register","account","data","details","information","history","architecture","background","location","image","images","list","form","page"]
out = io.StringIO()
out.write("DESIGN NOUNS PRESENT:\n")
for w in design_nouns:
    if counts.get(w):
        out.write(f"{w} ({counts[w]})\n")
out.write("\nDESIGN NOUNS ABSENT (0):\n")
out.write(", ".join(w for w in design_nouns if not counts.get(w)) + "\n")
out.write("\nSCOPE NOUNS:\n")
for w in scope_nouns:
    if counts.get(w):
        out.write(f"{w} ({counts[w]})\n")
open(r"C:\Users\me\Desktop\xem3\artifacts\logs\nouns.txt","w",encoding="utf-8").write(out.getvalue())
print("done")
