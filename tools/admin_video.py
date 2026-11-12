import re
p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Controllers\AdminController.cs"
s = open(p, encoding="utf-8").read()
s = s.replace(
 "string? culturalBackground, string? imagesCsv, string? factsText, bool isAssociated)",
 "string? culturalBackground, string? imagesCsv, string? factsText, string? video, bool isAssociated)")
s = s.replace(
 "                Facts = ParseFacts(factsText),\n                IsAssociated = isAssociated",
 "                Facts = ParseFacts(factsText),\n                Video = string.IsNullOrWhiteSpace(video) ? null : video.Trim(),\n                IsAssociated = isAssociated")
s = s.replace(
 "                    .Set(s => s.Facts, ParseFacts(factsText))\n                    .Set(s => s.IsAssociated, isAssociated));",
 "                    .Set(s => s.Facts, ParseFacts(factsText))\n                    .Set(s => s.Video, string.IsNullOrWhiteSpace(video) ? null : video.Trim())\n                    .Set(s => s.IsAssociated, isAssociated));")
open(p, "w", encoding="utf-8").write(s)
print("admin controller video field wired")
