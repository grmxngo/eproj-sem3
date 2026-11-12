# Algorithms (core flows)

1. Register: trim email → check unique (index) → hash password (PBKDF2 via PasswordHasher) → insert → sign in.
2. Interest toggle: require login → load event → reject if startDate < now → upsert/delete interest
   (unique index eventId+userId) → notice.
3. Invite: require login → validate email contains @ → insert invitation → write .eml to per-env outbox.
4. Query reply (admin): save reply → status Answered → .eml reply to the member's email.
5. Site save (admin): required name/location → slugify → 5 required content fields → facts (one per line).
6. Event save (admin): title required → reject if endDate < startDate.
7. Category delete (admin): blocked while sites reference the category.
8. Gallery upload: allow jpg/jpeg/png/webp ≤ 5 MB → GUID filename → /uploads/{name}.
