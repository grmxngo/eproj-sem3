import io, json, os, time, urllib.request, urllib.parse
API = "https://commons.wikimedia.org/w/api.php"
UA = {"User-Agent": "HeritageTrailSchoolProject/1.0 (educational media fetch)"}
ROOT = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot"
os.makedirs(os.path.join(ROOT, "videos"), exist_ok=True)
log = io.StringIO()
def out(m): log.write(m + "\n"); print(m)
def api(params, tries=4):
    params = dict(params, format="json")
    url = API + "?" + urllib.parse.urlencode(params)
    for a in range(tries):
        try:
            req = urllib.request.Request(url, headers=UA)
            with urllib.request.urlopen(req, timeout=30) as r:
                return json.load(r)
        except Exception:
            if a == tries - 1: raise
            time.sleep(2.5 * (a + 1))
def download(url, path):
    req = urllib.request.Request(url, headers=UA)
    with urllib.request.urlopen(req, timeout=300) as r:
        data = r.read()
    with open(path, "wb") as f:
        f.write(data)
    return len(data)

TARGETS = {"angkor-wat": "Angkor Wat", "petra": "Petra Jordan", "machu-picchu": "Machu Picchu"}
picked = []
for slug, term in TARGETS.items():
    try:
        res = api({"action":"query","generator":"search","gsrsearch":"filetype:video " + term,
                   "gsrnamespace":6,"gsrlimit":10,"prop":"info"})
        pages = list(res.get("query", {}).get("pages", {}).values())
        titles = [p["title"] for p in pages if p["title"].lower().endswith(".webm")]
        out(f"{slug}: {len(titles)} webm candidates")
        for title in titles:
            vi = api({"action":"query","titles":title,"prop":"videoinfo","viprop":"derivatives"})
            for p in vi.get("query", {}).get("pages", {}).values():
                derivs = p.get("videoinfo", [{}])[0].get("derivatives", [])
                choice = None
                for d in derivs:
                    src = d.get("src", "")
                    if src.endswith(".webm") and ("720" in d.get("type", "") or "480" in d.get("type", "")):
                        choice = src
                if not choice:
                    for d in derivs:
                        if d.get("src", "").endswith(".webm"):
                            choice = d["src"]
                if not choice:
                    continue
                try:
                    size = download(choice, os.path.join(ROOT, "videos", slug + ".webm"))
                    if size > 25 * 1024 * 1024:
                        os.remove(os.path.join(ROOT, "videos", slug + ".webm"))
                        out(f"  {title}: {size//1024//1024} MB too big, skip")
                        continue
                    picked.append({"slug": slug, "file": title, "url": choice, "bytes": size})
                    out(f"  VIDEO {slug} <- {title} ({size//1024//1024} MB)")
                    break
                except Exception as de:
                    out(f"  DL-ERR {title}: {de}")
            if any(x["slug"] == slug for x in picked):
                break
        time.sleep(1.5)
    except Exception as e:
        out(f"ERR {slug}: {e}")

with open(os.path.join(ROOT, "videos", "credits.json"), "w", encoding="utf-8") as f:
    json.dump(picked, f, ensure_ascii=False, indent=2)
open(r"C:\Users\me\Desktop\xem3\artifacts\logs\video-fetch.txt", "w", encoding="utf-8").write(log.getvalue())
print("VIDEOS:", len(picked))
