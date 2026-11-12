# Customer Requirement Specification

Source of truth: `../demand-coverage.md` (T01–T33 mapped to implementation) and the business-analysis
document `../../reference/PhanTichNghiepVu-HeritageTrail.docx` (flows, business rules, data model, validation).

Scope decisions confirmed with the client:
- Content: English, worldwide heritage (Angkor, Petra, Machu Picchu…).
- Design system: `../design/decisions.md` — palette H3, typography T1, radius R1, density D2, hero C,
  buttons B1, event card E3, site card S1, gallery G1, forms F1, auth layout A1.
- Stack: .NET 10 (ASP.NET Core MVC) + MongoDB 9.0.4 (spec T32 allows MongoDB 6.0+ / VS 2022 family).
