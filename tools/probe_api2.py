import json, urllib.request, urllib.parse, os
API = "https://commons.wikimedia.org/w/api.php"
UA = {"User-Agent": "HeritageTrailSchoolProject/1.0 (educational)"}
def api(params):
    params = dict(params, format="json")
    url = API + "?" + urllib.parse.urlencode(params)
    req = urllib.request.Request(url, headers=UA)
    with urllib.request.urlopen(req, timeout=30) as r:
        return json.load(r)
res = api({"action":"query","generator":"search","gsrsearch":"filetype:bitmap Angkor Wat temple Cambodia","gsrnamespace":6,"gsrlimit":12,"prop":"imageinfo","iiprop":"url|extmetadata","iiurlwidth":1600})
pages = list(res.get("query", {}).get("pages", {}).values())
print("pages:", len(pages))
for p in pages[:3]:
    ii = p.get("imageinfo", [{}])[0]
    url = ii.get("url", "")
    thumb = ii.get("thumburl") or url
    ext = os.path.splitext(url)[1].lower()
    print(repr(url[:80]), "| ext:", repr(ext), "| thumb http:", thumb.startswith("http"), "| hasimageinfo:", "imageinfo" in p)
