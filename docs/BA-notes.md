# BA notes — stack override & recheck fixes

## Stack override (user requirement)
BA doc §10 proposed Node.js 18 + Express + EJS + Nodemailer + Multer. The user requires **.NET 10**, so the implementation stack is:

| BA proposal | Implemented |
|---|---|
| Node.js + Express | ASP.NET Core 10 MVC (Razor views) |
| MongoDB 6.0+ | MongoDB 9.0.4 (spec T32 allows 6.0+) via MongoDB.Driver 3.13 |
| Nodemailer | MailKit-free file outbox (`artifacts/mail/{env}`) behind `IMailSender`; SMTP later |
| Multer uploads | ASP.NET Core multipart + `ImageStore` validation (jpg/png/webp ≤5 MB) |
| bcrypt | `PasswordHasher` (PBKDF2) — satisfies hash requirement; deviation logged |

All flows, rules, data model and validation from the BA doc remain the SSOT.

## Recheck fixes applied in code (see recheck report)
- F1: HeritageSite required fields = name, categoryId, location, historicalBackground, architectureSignificance (5). FR-A3 copy extended; Location validation added.
- F2: Query lifecycle implemented as Open → Answered (no Closed UI).
- F3: Deleting a category that still has sites is blocked (not "move or block").
- F4: doc wording only — no code impact.

## Open doc sync
- BA docx §10 stack line should be updated to .NET 10 (+ Mongo). Ask user before editing the original docx.
