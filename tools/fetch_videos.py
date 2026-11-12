import io, json, os, time, urllib.request, urllib.parse

API = "https://commons.wikimedia.org/w/api.php"
UA = {"User-Agent": "HeritageTrailSchoolProject/1.0 (educational media fetch)"}
ROOT = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot"
os.makedirs(os.path.join(ROOT, "videos"), exist_ok=True)
log = io.StringIO()

def out(m):
    log.write(m + "\n")
    print(m)

def api(params, tries=5):
    params = dict(params, format="json")
    url = API + "?" + urllib.parse.urlencode(params)
    for a in range(tries):
        try:
            req = urllib.request.Request(url, headers=UA)
            with urllib.request.urlopen(req, timeout=30) as r:
                return json.load(r)
        except Exception as e:
            if a == tries - 1:
                raise
            wait = 3 * (a + 1)
            out(f"  retry {a + 1} in {wait}s ({type(e).__name__})")
            time.sleep(wait)

def download(url, path):
    req = urllib.request.Request(url, headers=UA)
    with urllib.request.urlopen(req, timeout=600) as r:
        data = r.read()
    with open(path, "wb") as f:
        f.write(data)
    return len(data)

TARGETS = {
    "angkor-wat": "Angkor Wat Cambodia",
    "petra": "Petra Jordan",
    "machu-picchu": "Machu Picchu Peru",
    "taj-mahal": "Taj Mahal Agra",
    "great-wall": "Great Wall of China",
    "forbidden-city": "Forbidden City Beijing",
    "topkapi-palace": "Topkapi Palace Istanbul",
    "acropolis-museum": "Acropolis Athens",
    "colosseum": "Colosseum Rome",
    "chichen-itza": "Chichen Itza Mexico",
    "giza": "Pyramids Giza Egypt",
    "stonehenge": "Stonehenge England",
    "borobudur": "Borobudur temple Indonesia",
    "kinkaku-ji": "Kinkaku-ji Kyoto",
    "alhambra": "Alhambra Granada",
    "rapa-nui": "Moai Easter Island",
    "sigiriya": "Sigiriya Sri Lanka",
    "ephesus": "Ephesus Celsus Turkey",
}

picked = []
for slug, term in TARGETS.items():
    target = os.path.join(ROOT, "videos", slug + ".webm")
    if os.path.exists(target) and os.path.getsize(target) > 100_000:
        out(f"KEEP {slug} (already downloaded)")
        continue
    try:
        res = api({"action": "query", "generator": "search", "gsrsearch": "filetype:video " + term,
                   "gsrnamespace": 6, "gsrlimit": 12, "prop": "info"})
        pages = list(res.get("query", {}).get("pages", {}).values())
        titles = [p["title"] for p in pages if p["title"].lower().endswith(".webm")]
        out(f"{slug}: {len(titles)} webm candidates")
        got = False
        for title in titles:
            vi = api({"action": "query", "titles": title, "prop": "videoinfo", "viprop": "derivatives"})
            for p in vi.get("query", {}).get("pages", {}).values():
                derivs = p.get("videoinfo", [{}])[0].get("derivatives", [])
                candidates = []
                for d in derivs:
                    src = d.get("src", "")
                    if src.endswith(".webm"):
                        t = d.get("type", "")
                        score = 2 if "480" in t else 1 if "720" in t else 0
                        candidates.append((score, src))
                candidates.sort(key=lambda x: -x[0])
                for _, src in candidates:
                    try:
                        size = download(src, target)
                        if size > 25 * 1024 * 1024:
                            os.remove(target)
                            out(f"  {title}: {size // 1024 // 1024} MB too big")
                            continue
                        picked.append({"slug": slug, "file": title, "url": src, "bytes": size})
                        out(f"  VIDEO {slug} <- {title} ({size // 1024 // 1024} MB)")
                        got = True
                        break
                    except Exception as de:
                        out(f"  DL-ERR {title}: {de}")
                if got:
                    break
            if got:
                break
        if not got:
            out(f"  NO-VIDEO {slug}")
        time.sleep(2)
    except Exception as e:
        out(f"ERR {slug}: {e}")

with open(os.path.join(ROOT, "videos", "credits.json"), "w", encoding="utf-8") as f:
    json.dump(picked, f, ensure_ascii=False, indent=2)
open(r"C:\Users\me\Desktop\xem3\artifacts\logs\video-fetch.txt", "w", encoding="utf-8").write(log.getvalue())
print("VIDEOS:", len(picked))
