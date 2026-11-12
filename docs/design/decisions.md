# Design decisions log (SSOT)

Each row records an atom the user approved in chat. No atom goes into code without a row here.

## Product decisions (confirmed in chat)

| Decision | Choice | Note | Date |
|---|---|---|---|
| Main purpose | Bộ mặt chính thức + kênh vận hành của tổ chức di sản (thông tin đáng tin · chương trình mời tham gia · cộng đồng · admin tự cập nhật) | Lớp học thuật: eProject Aptech mô phỏng dự án thật | 2026-11-11 |
| Vibe | **Trail guide — nền sáng** | Wayfinding, dữ kiện là trung tâm; palette tối bị loại | 2026-11-11 |
| Org model | A — org tự tổ chức chương trình; user bấm Interested để tham gia (mặc định, bạn có thể đổi sang B/C) | Khớp spec "Our Programmes ... conducted" | 2026-11-11 |
| Content scope | **English only, di sản toàn cầu** (Angkor, Petra, Kyoto, Machu Picchu…), không còn Việt Nam | Chốt bởi user; spec không giới hạn địa lý | 2026-11-11 |
| Priority | **Spec-first** — làm đủ chức năng spec trước (public + admin + auth + flows), design gọn; Trail System (map/chapters/rail/passport) hoãn lại, chỉ thêm 1 signature device nếu còn thời gian | User chọn sau khi mình nêu conflict scope-vs-goal | 2026-11-11 |
| Provisional design v0 | Palette P4 (cát + gạch nung + teal), serif display + sans body, density editorial | Tạm dùng để build; verify page-level sau | 2026-11-11 |

## Atom decisions

| Atom | Decision | Evidence | Date |
|---|---|---|---|
| Palette | **H3 — Cool archive** (bg #eef0ea · surface #e2e6dd · ink #141a17 · accent #b5311f · alt #2f6b5e) | home-h3.png — user: "h3 is good" | 2026-11-11 |
| Typography | **T1 — Classical editorial** (Palatino/Georgia display + Segoe UI body) | type-t1.png approved | 2026-11-11 |
| Radius | **R1 — Sharp heritage** (2/4/6px) | radius-r1.png approved | 2026-11-11 |
| Density | **D2 — Regular** (1.0) | density-d2.png approved | 2026-11-11 |
| Hero layout | **C — Editorial band** (ảnh band trên + copy bất đối xứng) | hero-c.png approved; default on `/` | 2026-11-11 |
| Buttons | **B1 — Rectangular uppercase** (solid accent + ghost viền) | buttons-b1.png approved | 2026-11-11 |
| Event card | **E3 — Big date** (số ngày lớn kiểu lịch) | events-e3.png approved; default on `/` | 2026-11-11 |
| Site card | **S1 — Image top** | cards-s1.png approved | 2026-11-11 |
| Gallery | **G1 — Grid 4 cột** | gallery-g1.png approved | 2026-11-11 |
| Forms | **F1 — Boxed** + labels mono uppercase | form-f1.png approved | 2026-11-11 |
| Auth layout | **A1 — Document + margin** (áp cho Register + Login) | auth-a1.png approved | 2026-11-11 |
| Counts | Bỏ toàn bộ số đếm vô nghĩa trên homepage (user: "make no sense") | home-fold.png | 2026-11-11 |
| Base/Palette exploratory (A-01a/b) | Superseded by H3 | decisions history above | 2026-11-11 |
