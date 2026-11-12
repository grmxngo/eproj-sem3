import re
p = r"C:\Users\me\Desktop\xem3\docs\demand-coverage.md"
s = open(p, encoding="utf-8").read()
s = s.replace("T09 | User sees/navigates, registers, selects site details, invites friends by mail | 🟡 | Register/view/detail/invite work; invite is recorded (`invitations`), real SMTP outbox not yet wired |",
              "T09 | User sees/navigates, registers, selects site details, invites friends by mail | ✅ | Register/view/detail/invite work; invites written to per-env mail outbox (`.eml`) |")
s = s.replace("T12 | Other Heritage Locations associated with the organisation, with images | 🟡 | Sites carry `isAssociated`; admin can set it; public \"associated\" grouping not yet shown |",
              "T12 | Other Heritage Locations associated with the organisation, with images | ✅ | \"Associated locations\" section on /heritage-sites + `isAssociated` flag in admin |")
s = s.replace("T17 | List of Other associated Heritage Sites — admin adds details | 🟡 | Same Site CRUD (`isAssociated`); no separate listing yet |",
              "T17 | List of Other associated Heritage Sites — admin adds details | ✅ | Site CRUD with `isAssociated`; listed publicly under Associated locations |")
s = s.replace("T20 | Gallery: images of sites and programmes added by admin | ✅ | Admin → Gallery add/delete; public `/gallery` |",
              "T20 | Gallery: images of sites and programmes added by admin | ✅ | Admin → Gallery upload (jpg/png/webp ≤5MB, .exe blocked) + URL; public `/gallery` |")
s = s.replace("T25 | Send mail to invite friends to explore a site | 🟡 | Invitation recorded with validation; mail sender (file outbox) to be wired |",
              "T25 | Send mail to invite friends to explore a site | ✅ | Invitation recorded + mail outbox (`.eml` per environment) |")
s = s.replace("T29 | Daily data backup | 🟡 | Staging uses per-env Mongo data dirs; automated daily backup script not yet added |",
              "T29 | Daily data backup | ✅ | scripts/backup.ps1 (mongodump per env → artifacts/backup); schedule note in docs |")
s = s.replace("T33 | Single ZIP: source + docs (login credentials) + DB backup; name = Batch/Group/Title; only one submission | 🟡 | Packaging script to be added; credentials in seed (admin@heritagetrail.local / Admin123!) |",
              "T33 | Single ZIP: source + docs (login credentials) + DB backup; name = Batch/Group/Title; only one submission | ✅ | scripts/package.ps1 (source+docs+reference+latest backup → artifacts/HeritageTrail-submission-*.zip); credentials seeded |")
open(p, "w", encoding="utf-8").write(s)
print("coverage updated")
