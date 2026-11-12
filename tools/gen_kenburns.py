import os, subprocess, glob, shutil

FFMPEG = r"C:\Users\me\AppData\Local\Microsoft\WinGet\Links\ffmpeg.exe"
ROOT = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot"
SITES = ["forbidden-city", "kinkaku-ji", "rapa-nui", "sigiriya"]

for slug in SITES:
    imgs = sorted(glob.glob(os.path.join(ROOT, "images", "sites", slug, "img*.*")))
    imgs = [i for i in imgs if i.lower().endswith((".jpg", ".jpeg", ".png"))][:4]
    if not imgs:
        print(f"{slug}: no images, skip")
        continue

    parts = []
    work = os.path.join(ROOT, "videos", "_work")
    os.makedirs(work, exist_ok=True)
    for idx, img in enumerate(imgs):
        zoom_expr = (
            f"min(1+0.0013*on,1.14)" if idx % 2 == 0
            else f"max(1.14-0.0013*on,1)"
        )
        vf = (
            "scale=1280:720:force_original_aspect_ratio=increase,"
            "crop=1280:720,"
            f"zoompan=z='{zoom_expr}':x='iw/2-(iw/zoom/2)':y='ih/2-(ih/zoom/2)':d=75:s=1280x720:fps=30"
        )
        part = os.path.join(work, f"{slug}-{idx}.webm")
        r = subprocess.run(
            [FFMPEG, "-y", "-loop", "1", "-i", img, "-vf", vf, "-t", "2.5",
             "-c:v", "libvpx-vp9", "-crf", "34", "-b:v", "0", part],
            capture_output=True)
        if r.returncode != 0:
            print(f"{slug} part {idx} failed: {r.stderr.decode(errors='ignore')[:200]}")
            continue
        parts.append(part)

    if parts:
        lst = os.path.join(work, f"{slug}.txt")
        with open(lst, "w", encoding="utf-8") as f:
            for p in parts:
                f.write(f"file '{p.replace(chr(39), chr(39) + chr(39))}'\n")
        out = os.path.join(ROOT, "videos", slug + ".webm")
        r = subprocess.run(
            [FFMPEG, "-y", "-f", "concat", "-safe", "0", "-i", lst, "-c", "copy", out],
            capture_output=True)
        if r.returncode == 0 and os.path.exists(out):
            print(f"{slug}: generated {os.path.getsize(out) // 1024} KB from {len(parts)} images")
        else:
            print(f"{slug} concat failed: {r.stderr.decode(errors='ignore')[:200]}")
        for p in parts:
            os.remove(p)
        os.remove(lst)
shutil.rmtree(work, ignore_errors=True)
print("done")
