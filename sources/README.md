# sources — ảnh gốc bạn cung cấp

Đây là nơi bạn thả ảnh gốc (Pinterest, self-shot, museum archive...) để mình xử lý qua pipeline:

1. Đặt file vào `sources/incoming/` (jpg/png/webp, càng chất lượng cao càng tốt).
2. Mỗi file cần một dòng credit trong `sources/credits.json` (xem ví dụ bên dưới) — ghi URL nguồn + tác giả + trạng thái bản quyền. Mình không tự nhận sở hữu ảnh.
3. Mình chạy `scripts/process-images.ps1` (Phase E) để crop theo tỉ lệ token (hero 21:9, card 4:3, tile 1:1), upscale, grain, tone; kết quả vào `app/src/HeritageTrail.Web/Content/images/`.

Ví dụ `sources/credits.json`:

```json
[
  {
    "file": "hoi-an-bridge.jpg",
    "source": "https://example.com/photo",
    "author": "Tên tác giả",
    "license": "to confirm",
    "note": "Bạn sẽ liên hệ tác giả sau — ghi rõ trạng thái"
  }
]
```

Trong lúc chưa có ảnh thật, các preview dùng hình xử lý tạm (gradient/grain) để duyệt bố cục — không phải ảnh cuối.
