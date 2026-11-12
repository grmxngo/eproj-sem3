# Media pipeline (real footage & images)

All imagery and footage are **real, licensed media from Wikimedia Commons** (not placeholders),
collected by `tools/fetch_media.py` and `tools/fetch_videos.py`.

| Asset | Where | License data |
|---|---|---|
| 59 site images (18 sites, 3–4 each) | `app/src/HeritageTrail.Web/wwwroot/images/sites/{slug}/img*.jpg` | `.../sites/credits.json` (file, author, license, page) |
| 3 footage clips (webm) | `.../wwwroot/videos/{slug}.webm` — angkor-wat (9.7 MB), petra (2.7 MB), machu-picchu (0.5 MB) | `.../videos/credits.json` |

Usage:
- Home: hero footage (Angkor), "Footage" band (3 clips), gallery strip (real photos).
- Sites list/detail: first image = hero/thumb; detail shows a "Site images" grid; sites with `Video` play footage in the full-bleed hero.
- Gallery: 12 seeded items reference real site images with `Credit: Wikimedia Commons`.

Re-run fetch (needs internet): `python tools/fetch_media.py`, `python tools/fetch_videos.py` (bundled python).

# Tests note

`dotnet test` is blocked on this machine by a harness sandbox restriction (testhost cannot OpenProcess its parent).
The same 16 tests run via the console runner:

```
cd app/src/SmokeRunner && dotnet run
```

Result: **16 passed, 0 failed** (register/duplicate, RBAC, search, admin add site, category delete block,
interest toggle/past-event, invalid invite, query reply, end<start, .exe reject, png accept, no password exposure).
