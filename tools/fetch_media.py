import io, json, os, time, urllib.request, urllib.parse

API = "https://commons.wikimedia.org/w/api.php"
UA = {"User-Agent": "HeritageTrailSchoolProject/1.0 (educational media fetch)"}
ROOT = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot"
os.makedirs(os.path.join(ROOT, "images", "sites"), exist_ok=True)
log = io.StringIO()

def out(msg):
    log.write(msg + "\n"); print(msg)

def api(params, tries=4):
    params = dict(params, format="json")
    url = API + "?" + urllib.parse.urlencode(params)
    for attempt in range(tries):
        try:
            req = urllib.request.Request(url, headers=UA)
            with urllib.request.urlopen(req, timeout=30) as r:
                return json.load(r)
        except Exception as e:
            if attempt == tries - 1:
                raise
            time.sleep(2.5 * (attempt + 1))

def download(url, path, tries=3):
    for attempt in range(tries):
        try:
            req = urllib.request.Request(url, headers=UA)
            with urllib.request.urlopen(req, timeout=120) as r:
                data = r.read()
            os.makedirs(os.path.dirname(path), exist_ok=True)
            with open(path, "wb") as f:
                f.write(data)
            return len(data)
        except Exception:
            if attempt == tries - 1:
                raise
            time.sleep(2)

SITES = {
    "angkor-wat": ("Angkor Wat temple Cambodia", 4),
    "petra": ("Petra Al-Khazneh Treasury Jordan", 4),
    "machu-picchu": ("Machu Picchu Peru ruins", 4),
    "taj-mahal": ("Taj Mahal Agra mausoleum", 4),
    "great-wall": ("Great Wall of China Mutianyu", 4),
    "forbidden-city": ("Forbidden City Beijing palace", 4),
    "topkapi-palace": ("Topkapi Palace Istanbul", 4),
    "acropolis-museum": ("Acropolis Museum Athens", 4),
    "colosseum": ("Colosseum Rome amphitheatre", 3),
    "chichen-itza": ("Chichen Itza El Castillo Mexico", 3),
    "giza": ("Pyramids of Giza Egypt Sphinx", 3),
    "stonehenge": ("Stonehenge England megalith", 3),
    "borobudur": ("Borobudur temple Indonesia", 3),
    "kinkaku-ji": ("Kinkaku-ji Golden Pavilion Kyoto", 3),
    "alhambra": ("Alhambra Granada Nasrid palace", 3),
    "rapa-nui": ("Moai Easter Island Rapa Nui", 3),
    "sigiriya": ("Sigiriya rock fortress Sri Lanka", 3),
    "ephesus": ("Library of Celsus Ephesus Turkey", 3),
}

credits = []
for slug, (query, want) in SITES.items():
    try:
        res = api({
            "action": "query", "generator": "search", "gsrsearch": "filetype:bitmap " + query,
            "gsrnamespace": 6, "gsrlimit": 12, "prop": "imageinfo",
            "iiprop": "url|extmetadata", "iiurlwidth": 1600,
        })
        pages = list(res.get("query", {}).get("pages", {}).values())
        got = 0
        for p in pages:
            ii = p.get("imageinfo", [{}])[0]
            url = ii.get("url", "")
            thumb = ii.get("thumburl") or url
            ext = os.path.splitext(urllib.parse.urlparse(url).path)[1].lower()
            if ext not in (".jpg", ".jpeg", ".png") or not thumb.startswith("http"):
                continue
            try:
                size = download(thumb, os.path.join(ROOT, "images", "sites", slug, f"img{got}{ext}"))
            except Exception as de:
                out(f"DL-ERR {slug}: {de}")
                continue
            if size < 5000:
                continue
            artist = (ii.get("extmetadata", {}).get("Artist", {}) or {}).get("value", "")
            lic = (ii.get("extmetadata", {}).get("LicenseShortName", {}) or {}).get("value", "")
            credits.append({"slug": slug, "file": p.get("title", ""), "page": ii.get("descriptionurl", ""),
                            "artist": artist, "license": lic,
                            "local": f"/images/sites/{slug}/img{got}{ext}"})
            got += 1
            out(f"IMG {slug} [{got}] {p.get('title')} ({size//1024} KB)")
            if got >= want:
                break
        if got == 0:
            out(f"NO-IMG {slug}")
        time.sleep(1.2)
    except Exception as e:
        out(f"ERR {slug}: {e}")

with open(os.path.join(ROOT, "images", "sites", "credits.json"), "w", encoding="utf-8") as f:
    json.dump(credits, f, ensure_ascii=False, indent=2)
with open(r"C:\Users\me\Desktop\xem3\artifacts\logs\media-fetch.txt", "w", encoding="utf-8") as f:
    f.write(log.getvalue())
print("TOTAL images:", len(credits))
