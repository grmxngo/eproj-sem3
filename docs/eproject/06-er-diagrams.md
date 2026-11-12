# E-R Diagrams (MongoDB collections)

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
