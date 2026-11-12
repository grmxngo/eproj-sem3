import re, collections, io
files = [
    r"C:\Users\me\Desktop\xem3\reference\conv\HeritageTrail-website.txt",
    r"C:\Users\me\Desktop\xem3\reference\spec.txt",
    r"C:\Users\me\Desktop\xem3\reference\HeritageTrail\Readme-Student Guide-eProject Process.txt",
]
text = " ".join(open(f, encoding="utf-8", errors="ignore").read() for f in files).lower()
tokens = re.split(r"[^a-z0-9]+", text)
words = [w for w in tokens if w]

def count_forms(*forms):
    return sum(words.count(f) for f in forms)

domain = {
    "heritage": count_forms("heritage"),
    "site(s)": count_forms("site","sites"),
    "place(s)": count_forms("place","places"),
    "monument(s)": count_forms("monument","monuments"),
    "temple(s)": count_forms("temple","temples"),
    "palace(s)": count_forms("palace","palaces"),
    "fort(s)": count_forms("fort","forts"),
    "museum(s)": count_forms("museum","museums"),
    "landmark(s)": count_forms("landmark","landmarks"),
    "architecture": count_forms("architecture"),
    "history": count_forms("history"),
    "historical": count_forms("historical"),
    "cultural": count_forms("cultural"),
    "background": count_forms("background","backgrounds"),
    "event(s)": count_forms("event","events"),
    "programme(s)": count_forms("programme","programmes","program","programs"),
    "gallery": count_forms("gallery"),
    "image(s)": count_forms("image","images"),
    "location(s)": count_forms("location","locations"),
    "walk(s)": count_forms("walk","walks"),
    "exhibition(s)": count_forms("exhibition","exhibitions"),
    "awareness": count_forms("awareness"),
    "explore/explor*": count_forms("explore","exploring","exploration"),
    "register/member": count_forms("register","registered","member","members"),
    "history+importance+significance": count_forms("history","importance","significance"),
}

bigrams = collections.Counter()
for i in range(len(words)-1):
    bigrams[words[i] + " " + words[i+1]] += 1
interesting = [b for b in bigrams if b.startswith("heritage ") or " heritage" in b or b in (
    "historical places","cultural background","heritage sites","heritage places","heritage walk",
    "heritage awareness","cultural activities","cultural locations","historical and cultural",
    "architecture and significance","history importance","our mission","about us")]
out = io.StringIO()
out.write("DOMAIN WORD COUNTS (whole corpus):\n")
for k, v in domain.items():
    out.write(f"{v:4d}  {k}\n")
out.write("\nBIGRAMS co-occurring with 'heritage' / key pairs:\n")
for b, c in bigrams.most_common():
    if b in interesting or b.startswith("heritage "):
        out.write(f"{c:4d}  {b}\n")
open(r"C:\Users\me\Desktop\xem3\artifacts\logs\wordcount2.txt","w",encoding="utf-8").write(out.getvalue())
print("done")
