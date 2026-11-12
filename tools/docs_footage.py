p = r"C:\Users\me\Desktop\xem3\docs\media-pipeline.md"
s = open(p, encoding="utf-8").read()
s = s.replace(
    "| 3 footage clips (webm) | `.../wwwroot/videos/{slug}.webm` — angkor-wat (9.7 MB), petra (2.7 MB), machu-picchu (0.5 MB) | `.../videos/credits.json` |",
    "| 18 footage clips (webm) — every site | `.../wwwroot/videos/{slug}.webm` | `.../videos/credits.json` |")
s = s.replace(
    "Re-run fetch (needs internet): `python tools/fetch_media.py`, `python tools/fetch_videos.py` (bundled python).",
    "Re-run fetch (needs internet): `python tools/fetch_media.py`, `python tools/fetch_videos.py` / `fetch_videos2.py` (bundled python).\n14 clips are Wikimedia video; 4 clips (forbidden-city, kinkaku-ji, rapa-nui, sigiriya) are generated ken-burns loops from the site's own photos: `python tools/gen_kenburns.py`.")
open(p, "w", encoding="utf-8").write(s)
print("docs updated")
