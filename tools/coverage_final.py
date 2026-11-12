p = r"C:\Users\me\Desktop\xem3\docs\demand-coverage.md"
s = open(p, encoding="utf-8").read()
s = s.replace("T30 | 13 documentation items (Certificate … Final Check List) | ⬜ | To be produced from `docs/` + BA analysis |",
              "T30 | 13 documentation items (Certificate … Final Check List) | ✅ | docs/eproject/ (01–13 + status-mail-template) |")
s = s.replace("T31 | 2 status mails (subjects STATUS:/DOUBT:/PROJECT SUBMISSION:) + feedback form | ⬜ | Process for the student; templates to be added in `docs/` |",
              "T31 | 2 status mails (subjects STATUS:/DOUBT:/PROJECT SUBMISSION:) + feedback form | ✅ | docs/eproject/status-mail-template.md; forms in reference/ |")
open(p, "w", encoding="utf-8").write(s)
print("coverage final")
