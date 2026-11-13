import os, subprocess, shutil

FF = r"C:\Users\me\AppData\Local\Microsoft\WinGet\Links\ffmpeg.exe"
V = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\videos"
ORIG = r"C:\Users\me\Desktop\xem3\artifacts\footage-original"
os.makedirs(ORIG, exist_ok=True)

for slug in ["angkor-wat", "petra", "machu-picchu"]:
    src = os.path.join(V, slug + ".webm")
    if not os.path.exists(src):
        print(slug, "missing, skip")
        continue
    shutil.copy2(src, os.path.join(ORIG, slug + ".webm"))
    tmp = os.path.join(V, slug + "-edit.webm")
    r = subprocess.run(
        [FF, "-y", "-i", src, "-t", "14",
         "-vf", "scale=1280:720:force_original_aspect_ratio=decrease,pad=1280:720:(ow-iw)/2:(oh-ih)/2:color=black,eq=contrast=1.05:saturation=1.08,unsharp=5:5:0.4",
         "-an", "-c:v", "libvpx-vp9", "-crf", "30", "-b:v", "0", tmp],
        capture_output=True)
    if r.returncode == 0 and os.path.getsize(tmp) > 50_000:
        os.replace(tmp, src)
        print(f"{slug}: edited to {os.path.getsize(src) // 1024} KB (original kept in artifacts/footage-original)")
    else:
        print(slug, "edit failed:", r.stderr.decode(errors="ignore")[-200:])
        if os.path.exists(tmp):
            os.remove(tmp)
print("done")
