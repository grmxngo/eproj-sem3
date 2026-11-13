import os, glob
from PIL import Image

ROOT = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\wwwroot\images\sites"

count = 0
for img in glob.glob(os.path.join(ROOT, "*", "img*.*")):
    base, ext = os.path.splitext(img)
    if base.endswith("-sm"):
        continue
    out = base + "-sm.jpg"
    if os.path.exists(out):
        continue
    try:
        im = Image.open(img)
        im = im.convert("RGB")
        w, h = im.size
        if w > 800:
            im = im.resize((800, int(h * 800 / w)), Image.LANCZOS)
        im.save(out, "JPEG", quality=82, progressive=True, optimize=True)
        count += 1
    except Exception as e:
        print("ERR", img, e)
print("thumbs:", count)
