import json, urllib.request, urllib.parse
API = "https://commons.wikimedia.org/w/api.php"
UA = {"User-Agent": "HeritageTrailSchoolProject/1.0 (educational)"}
params = {"action":"query","generator":"search","gsrsearch":"filetype:bitmap Angkor Wat temple Cambodia","gsrnamespace":6,"gsrlimit":5,"prop":"imageinfo","iiprop":"url|extmetadata","iiurlwidth":1600,"format":"json"}
url = API + "?" + urllib.parse.urlencode(params)
print(url[:180])
req = urllib.request.Request(url, headers=UA)
with urllib.request.urlopen(req, timeout=30) as r:
    data = json.load(r)
print(json.dumps(data, indent=1)[:1500])
