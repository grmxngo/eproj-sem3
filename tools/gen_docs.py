import os
base = r"C:\Users\me\Desktop\xem3\docs\eproject"
os.makedirs(base, exist_ok=True)

docs = {
"01-certificate-of-completion.md": """# Certificate of Completion

Template — fill and sign before submission.

| Field | Value |
|---|---|
| Student name | __ |
| Enrollment No. | __ |
| Curriculum / Semester | __ / __ |
| Centre name | __ |
| eProject title | HeritageTrail — Heritage Organisation Website |
| Start / End date | __ / __ |

> This is to certify that the above student completed the eProject and submitted source code, documentation and database backup as required.
""",

"02-table-of-contents.md": """# Table of Contents

1. Certificate of Completion
2. Table of Contents
3. Problem Definition
4. Customer Requirement Specification
5. Project Plan
6. E-R Diagrams
7. Algorithms
8. GUI Standards Document
9. Interface Design Document
10. Task Sheet
11. Project Review and Monitoring Report
12. Unit Testing Check List
13. Final Check List
""",

"03-problem-definition.md": """# Problem Definition

Heritage organisations run walks, talks and exhibitions but have no single platform where visitors can
learn what survives at each site and register for programmes. HeritageTrail solves this with one website:

- Public: browse heritage sites by category, search, read history / architecture / cultural background,
  see upcoming and conducted programmes, gallery, contact, register, invite friends, raise queries,
  express interest in upcoming programmes.
- Admin: manage sites, categories, programmes, about pages, gallery, contact info, members and queries.

Traceability of every requirement: see `../demand-coverage.md` (T01–T33) and the Vietnamese analysis in `../../reference/PhanTichNghiepVu-HeritageTrail.docx`.
""",

"04-customer-requirement-specification.md": """# Customer Requirement Specification

Source of truth: `../demand-coverage.md` (T01–T33 mapped to implementation) and the business-analysis
document `../../reference/PhanTichNghiepVu-HeritageTrail.docx` (flows, business rules, data model, validation).

Scope decisions confirmed with the client:
- Content: English, worldwide heritage (Angkor, Petra, Machu Picchu…).
- Design system: `../design/decisions.md` — palette H3, typography T1, radius R1, density D2, hero C,
  buttons B1, event card E3, site card S1, gallery G1, forms F1, auth layout A1.
- Stack: .NET 10 (ASP.NET Core MVC) + MongoDB 9.0.4 (spec T32 allows MongoDB 6.0+ / VS 2022 family).
""",

"05-project-plan.md": """# Project Plan

| Phase | Work | Done |
|---|---|---|
| 1 | Environment: .NET 10 SDK (per-user), MongoDB per environment, staging/prod split | ✅ |
| 2 | Design system (tokens.json SSOT) + atom verification with client | ✅ |
| 3 | Public pages (home, sites, events, gallery, about, contact) | ✅ |
| 4 | Auth (register/login/roles) + account flows (interest, invite, query) | ✅ |
| 5 | Admin area (sites, categories, events, about, gallery, members, queries, contact) | ✅ |
| 6 | Integration tests — 16/16 passing (BA check list) | ✅ |
| 7 | Documentation set + submission package | ✅ |

Run: `scripts/staging.ps1` (http://127.0.0.1:5181), `scripts/prod.ps1` (:5182), `scripts/backup.ps1`, `scripts/package.ps1`.
""",

"06-er-diagrams.md": """# E-R Diagrams (MongoDB collections)

Entities (MongoDB collections; 1–N via ids):

- users (name, email unique, passwordHash, phone, address, occupation, organization, role)
- categories (name unique)
- sites (name, slug unique, category, location, historicalBackground, architectureSignificance,
  culturalBackground, images[], facts[], isAssociated)
- events (title, slug unique, type, description, siteId?, startDate, endDate, venue)
- interests (eventId+userId unique)
- gallery (imageUrl, caption, refType site|event, refId?, credit)
- aboutPages (slug unique, title, content) — 6 fixed pages
- queries (userId, subject, message, status Open|Answered, reply, repliedAt)
- invitations (fromUserId, toEmail, siteId, sentAt)
- contactInfos (single record) + contactMessages (name, email, message)

Relations: category 1–N site · site 1–N event · event N–N user (via interests) · user 1–N query ·
user 1–N invitation · gallery N–1 (site|event).
""",

"07-algorithms.md": """# Algorithms (core flows)

1. Register: trim email → check unique (index) → hash password (PBKDF2 via PasswordHasher) → insert → sign in.
2. Interest toggle: require login → load event → reject if startDate < now → upsert/delete interest
   (unique index eventId+userId) → notice.
3. Invite: require login → validate email contains @ → insert invitation → write .eml to per-env outbox.
4. Query reply (admin): save reply → status Answered → .eml reply to the member's email.
5. Site save (admin): required name/location → slugify → 5 required content fields → facts (one per line).
6. Event save (admin): title required → reject if endDate < startDate.
7. Category delete (admin): blocked while sites reference the category.
8. Gallery upload: allow jpg/jpeg/png/webp ≤ 5 MB → GUID filename → /uploads/{name}.
""",

"08-gui-standards-document.md": """# GUI Standards Document

Single source of truth: `app/src/HeritageTrail.Web/Content/design/tokens.json` (served at `/design/tokens.json`).

- Palette H3: bg #eef0ea · surface #e2e6dd · ink #141a17 · accent #b5311f · accentAlt #2f6b5e.
- Typography T1: Palatino/Georgia serif display · Segoe UI body · Consolas mono for every label/caption.
- Radius R1 (2/4/6px) · density D2 · fluid sizing only via clamp() · motion 120/200/320/480ms with reduced-motion.
- Buttons B1 (rectangular uppercase) · event card E3 (big date) · site card S1 (image top) · gallery G1 (grid, lead tile) · forms F1 (boxed, mono labels) · auth layout A1.
- Pages share: header (7 menu + logo mark) · page-head (eyebrow + serif h1 + lede) · section-head with rule · breadcrumb · mono captions.
""",

"09-interface-design-document.md": """# Interface Design Document

| Page | Route | Notes |
|---|---|---|
| Home | / | hero C, What's on band (E3), featured sites, categories, about strip, gallery, CTA |
| Heritage Sites | /heritage-sites | editorial index (numbered rows), search + category chips, associated locations |
| Site detail | /heritage-sites/{slug} | full-bleed hero, sections 01–05, "What to know" sidebar (facts), invite |
| Events | /events | lead next-event card, band, past list |
| Event detail | /events/{slug} | dark band, description, "What to know" sidebar, Interested |
| Gallery | /gallery | grid with lead tile, mono captions + credit |
| About | /about/{slug} | two-column + pages index (6 fixed pages) |
| Contact | /contact | form + office sidebar |
| Register / Login | /auth/register, /auth/login | layout A1 (document + margin) |
| Account | /account | interests, queries (send/replies), invitations |
| Admin | /admin/* | dark topbar + sidebar; dashboard, sites, categories, events, about, gallery, members, queries, contact |
""",

"10-task-sheet.md": """# Task Sheet

| # | Task | Status |
|---|---|---|
| 1 | Environment (.NET 10, MongoDB per env, staging/prod) | Done |
| 2 | Design system + atom verification | Done |
| 3 | Public pages | Done |
| 4 | Auth + account flows | Done |
| 5 | Admin area | Done |
| 6 | Mail outbox, uploads, associated sites, categories | Done |
| 7 | Integration tests (16) | Done |
| 8 | Backup/package/prod scripts | Done |
| 9 | 13-document set | Done |
| 10 | Final ZIP submission (Batch/Group/Title) | To do by student |
""",

"11-project-review-and-monitoring-report.md": """# Project Review and Monitoring Report

- Status report cadence (spec): 2 mails at ~10-day intervals (or 7–10 days and 3 days before end for <30-day projects).
- Subjects: `STATUS:` / `DOUBT:` / `PROJECT SUBMISSION:`.
- Git history: run `git log --oneline` — milestone commits: bootstrap → spec-first flows → design system → admin → SSOT cleanup → full features → tests.
- Status report template: `status-mail-template.md`.
""",

"12-unit-testing-check-list.md": """# Unit Testing Check List

16 integration tests (xUnit + WebApplicationFactory + isolated MongoDB) — `app/src/HeritageTrail.Tests/SpecTests.cs`.
Run: `dotnet test` (in `app/`). Result: **16/16 passed**.

| BA case | Test |
|---|---|
| Register ok / duplicate | Register_ValidUser_RedirectsToAccount · Register_DuplicateEmail_IsRejected |
| Guest → admin | Guest_AdminPage_RedirectsToLogin |
| User → admin 403 | NonAdminUser_AdminPage_IsDenied |
| Search empty / category | Search_NoResults_ShowsEmptyMessage · Search_Category_FiltersSites |
| Admin adds site | Admin_AddsSite_AppearsOnPublicList |
| Category delete blocked | Admin_DeletesCategoryInUse_IsBlocked |
| Interest duplicate / past event | Interest_Toggle_NeverDuplicates · Interest_PastEvent_IsRejected |
| Invite invalid email | Invite_InvalidEmail_IsNotRecorded |
| Query replied | Query_AdminReply_IsVisibleToUser |
| End < Start | Event_EndBeforeStart_IsRejected |
| .exe upload rejected / png accepted | Upload_Exe_IsRejected · Upload_Png_IsAcceptedAndServed |
| No password exposure | MembersPage_DoesNotExposePasswordHashes |
""",

"13-final-check-list.md": """# Final Check List

- [x] Source code (app/src) builds with .NET 10, 0 warnings
- [x] Tests: 16/16 passing
- [x] Staging (http://127.0.0.1:5181) and Production (http://127.0.0.1:5182) run from scripts
- [x] Documentation set (this folder + docs/design + demand-coverage.md)
- [x] Login credentials documented (admin@heritagetrail.local / Admin123! · demo@heritagetrail.local / Demo123!)
- [x] Database backup: scripts/backup.ps1 (mongodump → artifacts/backup)
- [x] Feedback form + status reports: see reference/eProject_*.xls and status-mail-template.md
- [ ] ZIP named Batch-Group-ProjectTitle, uploaded ONCE (no changes after submission)
""",

"status-mail-template.md": """# Status mail templates

## STATUS:
Subject: STATUS: HeritageTrail — report N/2

Progress:
- Completed: …
- Current: …
- Next: …

Challenges: …

Attachments: screenshots, source snapshot, DB script.

## DOUBT:
Subject: DOUBT: HeritageTrail — <question topic>

Question: …

Context: …

## PROJECT SUBMISSION:
Subject: PROJECT SUBMISSION: HeritageTrail — Batch <B>, Group <G>

Attachments: source ZIP, documentation (with credentials), DB backup, feedback form.

> Rule: only ONE submission per group; check all files before sending.
""",
}

for name, content in docs.items():
    open(os.path.join(base, name), "w", encoding="utf-8").write(content)
print("wrote", len(docs), "eproject docs")
