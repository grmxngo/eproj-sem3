# Interface Design Document

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
