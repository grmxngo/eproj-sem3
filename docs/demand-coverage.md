# Demand coverage — HeritageTrail (re-read of the project demand)

Mapping every spec requirement (T01–T33) to the current implementation. Status: ✅ done · 🟡 partial · ⬜ to do.

| ID | Requirement (from spec) | Status | Where |
|---|---|---|---|
| T01 | Website provides info about historical places, heritage sites, monuments, cultural landmarks | ✅ | `/heritage-sites`, `/heritage-sites/{slug}` |
| T02 | Each site: history, importance, architecture, cultural background | ✅ | Site detail sections (importance folded into Architecture & significance) |
| T03 | "Flash" details of all activities/events going to happen or conducted | ✅ | Home "What's on" (upcoming, big-date E3) + "Recently conducted" + `/events` |
| T04 | Allow adding new heritage site | ✅ | Admin → Sites → New/Edit/Save |
| T05 | Single platform to explore and learn heritage places | ✅ | Home hero + category browse + search |
| T06 | Home menus: Home, Heritage Sites, Explore, About us, Events, Gallery, REGISTER; no blank space | ✅ | Shared header (7 items, exact order); dense editorial home |
| T07 | Two modules: ADMIN and USER | ✅ | Role-based areas: `/admin` vs public + `/account` |
| T08 | Admin manages content: sites, about us, events, contact us, gallery | ✅ | Admin CRUD for all five + members/queries |
| T09 | User sees/navigates, registers, selects site details, invites friends by mail | 🟡 | Register/view/detail/invite work; invite is recorded (`invitations`), real SMTP outbox not yet wired |
| T10 | Navigation user friendly and uniform | ✅ | One layout, same nav/footer everywhere; breadcrumbs on detail pages |
| T11 | Heritage Sites list or categories to select (Forts, Palaces, Museums, Temples, Monuments, other) | ✅ | Category chips filter + search on `/heritage-sites` |
| T12 | Other Heritage Locations associated with the organisation, with images | 🟡 | Sites carry `isAssociated`; admin can set it; public "associated" grouping not yet shown |
| T13 | About us: 6 sublinks, each a page, info editable by admin | ✅ | `/about/{slug}` ×6; Admin → About editor |
| T14 | Contact Us page must be designed | ✅ | `/contact` (info + form) + Admin → Contact (info + messages) |
| T15 | Admin can view all registered members | ✅ | Admin → Members |
| T16 | Heritage Site Link sub fields: Site Name, Location, Historical Background, Architecture and Significance | ✅ | Site detail + admin form fields |
| T17 | List of Other associated Heritage Sites — admin adds details | 🟡 | Same Site CRUD (`isAssociated`); no separate listing yet |
| T18 | Our Programmes: heritage awareness, cultural activities, exhibitions, heritage walks | ✅ | Event `type` select (walk/cultural/exhibition/awareness) |
| T19 | Replying queries | ✅ | Admin → Queries → inline reply (Open → Answered) |
| T20 | Gallery: images of sites and programmes added by admin | ✅ | Admin → Gallery add/delete; public `/gallery` |
| T21 | Users can view pages but cannot edit content | ✅ | All write routes admin-only (`[Authorize(Roles="admin")]`) |
| T22 | Register account; few personal and professional data | 🟡 | Register = name/email/password; professional fields removed per user request ("remove all optional") |
| T23 | Search and view details of heritage sites | ✅ | Search by name/location (case-insensitive) |
| T24 | View location, history, architecture, images, other details | 🟡 | All text sections live; images = preview placeholders until real photography is supplied |
| T25 | Send mail to invite friends to explore a site | 🟡 | Invitation recorded with validation; mail sender (file outbox) to be wired |
| T26 | Raise query | ✅ | Account → My queries → send; admin replies |
| T27 | Send interest to take part in upcoming programmes | ✅ | `Interested` toggle (upcoming only, one per user); admin sees list per event |
| T28 | Code comments, logic explained, proper documentation, project report (synopsis+code+docs) | 🟡 | Code commented; docs live in `docs/`; full project report (13 documents) pending |
| T29 | Daily data backup | 🟡 | Staging uses per-env Mongo data dirs; automated daily backup script not yet added |
| T30 | 13 documentation items (Certificate … Final Check List) | ⬜ | To be produced from `docs/` + BA analysis |
| T31 | 2 status mails (subjects STATUS:/DOUBT:/PROJECT SUBMISSION:) + feedback form | ⬜ | Process for the student; templates to be added in `docs/` |
| T32 | Software: MongoDB 6.0+, NodeJS 18.x, Python SDK 3.x, VS2022 Professional, Cloud Tool (either/combination) | ✅ | MongoDB 9.0.4 + .NET 10 (VS 2022 Professional family) |
| T33 | Single ZIP: source + docs (login credentials) + DB backup; name = Batch/Group/Title; only one submission | 🟡 | Packaging script to be added; credentials in seed (admin@heritagetrail.local / Admin123!) |

Design system (SSOT = `Content/design/tokens.json`): palette H3, typography T1, radius R1, density D2, hero C, buttons B1, event card E3, site card S1, gallery G1, forms F1, auth layout A1. Hardcoded values were audited and moved into `tokens.json` (`ui` group) — see decisions in `docs/design/decisions.md`.
