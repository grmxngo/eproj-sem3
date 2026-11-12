# Project Plan

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
