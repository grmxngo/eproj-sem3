p = r"C:\Users\me\Desktop\xem3\app\src\HeritageTrail.Web\Services\SeedService.cs"
s = open(p, encoding="utf-8").read()
s = s.replace(
    "public static async Task RunAsync(MongoContext db, EnvironmentConfig config, ILogger logger)",
    "public static async Task RunAsync(MongoContext db, EnvironmentConfig config, ILogger logger, IWebHostEnvironment env)")
# remove hardcoded video assignments (auto-detected below)
s = s.replace('                Video = "/videos/angkor-wat.webm",\n', "")
s = s.replace('                Video = "/videos/petra.webm",\n', "")
s = s.replace('                Video = "/videos/machu-picchu.webm",\n', "")
# auto-detect before InsertManyAsync(sites)
s = s.replace(
    "        await db.Sites.InsertManyAsync(sites);",
    """        // Footage: any site with a downloaded clip at wwwroot/videos/{slug}.webm gets it.
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        foreach (var site in sites)
        {
            if (File.Exists(Path.Combine(webRoot, "videos", site.Slug + ".webm")))
            {
                site.Video = $"/videos/{site.Slug}.webm";
            }
        }

        await db.Sites.InsertManyAsync(sites);""")
open(p, "w", encoding="utf-8").write(s)
print("seed auto-detect wired")
