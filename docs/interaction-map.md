# Interaction map — micro / macro linking

Rule: every interactive atom has (a) motion states and (b) a destination. Every entity page is reachable from at least two hubs. No orphan pages; the crawl test enforces this.

## Micro (atom → section)
- Chip category (Forts/Palaces/…) → same page section filtered: `/heritage-sites?cat=…`.
- Chip date/venue (event) → `/events/{slug}` section "Venue & when".
- Site card → `/heritage-sites/{slug}` (hover lift motion 200ms).
- Event card → `/events/{slug}`.
- Gallery tile → lightbox (focus trap, Esc closes), caption + credit inside.
- Button "Interested" → POST toggle, 120ms press motion, state morph Interested ↔ Interested ✓.
- Invite button (site detail) → opens inline panel; success toast 480ms.
- Breadcrumb crumb → parent page (macro path shown on every detail page).
- Logo → `/` (motion mark plays draw animation on load; reduced-motion: static).

## Macro (section → page → project)
- Nav: Home · Heritage Sites · Explore · About us · Events · Gallery · REGISTER (exact spec order).
- Footer sitemap: all public pages + About 6 subpages + Contact + legal/credits.
- Detail pages always link: site ↔ related events, site ↔ gallery, event → site, about page ↔ footer.
- Account area reachable from header (avatar): Profile, Invite, My Queries, My Interests.
- Admin area: topbar + sidebar; sidebar item → list → form (back link to list).
- Env badge (staging only) is non-navigational, but shows current environment on every page.

## Motion system (one pattern)
- Tokens: fast 120ms (hover/press), base 200ms (state), slow 320ms (section/panel), slower 480ms (hero/reveal).
- Enter = easeOut; exit = easeIn; reduced-motion → all transitions ≤1ms, no transforms except opacity.
- Every hover/focus state has a visible focus ring token (a11y).

## Link graph test (staging)
Crawl `/` (auth as guest, user, admin). Assert: every internal href/src resolves 200; every site/event detail reachable from hub pages; no page without outbound nav/footer; `target=_blank` only for external links.
