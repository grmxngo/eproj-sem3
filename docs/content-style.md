# Content style — "no re-description" rule

The website shows **only information the visitor does not already know**. No filler, no obvious descriptions.

## Rules
1. Every sentence answers: *what will the visitor learn?*
   - Good: "Hội An Lantern Festival — 14th night of the lunar month, An Hội island; lanterns are released on the Thu Bồn river."
   - Bad: "This is a temple." / "Welcome to our heritage website." / "Here you can see many beautiful things."
2. Dates, places, dynasties, travel, opening hours, programmes — concrete facts. No marketing adjectives without facts.
3. No lorem ipsum, no placeholder sentences shipped to staging sign-off.
4. Microcopy only where it helps action: labels, validation messages, empty states ("No heritage site found for this search").
5. Every fact gets a quick check before sign-off. Uncertain facts are marked `[verify]` in `staging` seed and listed in `docs/content/verify.md`; never silently invented.
6. Images carry a credit line (source URL + author) in `credits.json`; status "to confirm" until the user clears licensing.

## Banned phrases (copy audit)
- "This is a …", "Welcome to our website", "We have many …", "Click here", "Beautiful", "Amazing".
- Any sentence that restates the heading.

## Example page copy (staging, English, worldwide heritage)
- Hero: "Angkor Wat: built in the 12th century under Suryavarman II; the moat runs 190 m wide; five towers stand at the centre."
- Event chip: "Temple walk — Angkor · Sat 07:00".
