using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace HeritageTrail.Web.Services;

/// <summary>Seeds real global-heritage content (Wikimedia images/footage) with facts.
/// Staging = Reset (drops + reseeds), Production = Once (only when empty).</summary>
public static class SeedService
{
    private static string Img(string slug, int i, string ext = ".jpg") => $"/images/sites/{slug}/img{i}{ext}";

    public static async Task RunAsync(MongoContext db, EnvironmentConfig config, ILogger logger, IWebHostEnvironment env)
    {
        await db.EnsureIndexesAsync();

        var userCount = await db.Users.CountDocumentsAsync(FilterDefinition<User>.Empty);
        if (config.SeedMode != "Reset" && userCount > 0)
        {
            return;
        }

        if (config.SeedMode == "Reset")
        {
            foreach (var name in new[]
            {
                "users", "categories", "sites", "events", "interests", "gallery",
                "aboutPages", "queries", "invitations", "contactMessages", "contactInfos"
            })
            {
                try { await db.Database.DropCollectionAsync(name); }
                catch (MongoCommandException) { /* collection did not exist */ }
            }
        }

        logger.LogInformation("Seeding {Database} ({Mode})", db.DatabaseName, config.SeedMode);

        var hasher = new PasswordHasher<User>();
        var admin = new User { Name = "HeritageTrail Admin", Email = "admin@heritagetrail.local", Role = "admin" };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
        var demo = new User { Name = "Demo Visitor", Email = "demo@heritagetrail.local", Role = "user" };
        demo.PasswordHash = hasher.HashPassword(demo, "Demo123!");
        await db.Users.InsertManyAsync(new[] { admin, demo });

        await db.Categories.InsertManyAsync(new[]
        {
            new Category { Name = "Temples" },
            new Category { Name = "Monuments" },
            new Category { Name = "Forts" },
            new Category { Name = "Palaces" },
            new Category { Name = "Museums" }
        });

        var sites = new[]
        {
            new HeritageSite
            {
                Name = "Angkor Wat", Slug = "angkor-wat", Category = "Temples", Location = "Siem Reap, Cambodia",
                HistoricalBackground = "Built in the 12th century under King Suryavarman II as a Hindu temple dedicated to Vishnu.",
                ArchitectureSignificance = "Five towers stand for Mount Meru; the moat is 190 m wide and the outer gallery holds more than 1,000 metres of bas-reliefs.",
                CulturalBackground = "UNESCO World Heritage since 1992; the temple remains a living Buddhist site.",
                Images = new() { Img("angkor-wat", 0), Img("angkor-wat", 1), Img("angkor-wat", 2), Img("angkor-wat", 3) },
                Facts = new() { "Built in the 12th century under Suryavarman II", "The moat is 190 m wide", "UNESCO World Heritage since 1992" }
            },
            new HeritageSite
            {
                Name = "Petra", Slug = "petra", Category = "Monuments", Location = "Ma'an, Jordan",
                HistoricalBackground = "The Nabataean capital was carved into sandstone from the 4th century BCE and annexed by Rome in 106 CE.",
                ArchitectureSignificance = "The Treasury facade is 39 m tall, cut directly into the cliff; the Siq is a 1.2 km natural gorge.",
                CulturalBackground = "UNESCO World Heritage since 1985.",
                Images = new() { Img("petra", 0), Img("petra", 1), Img("petra", 2), Img("petra", 3) },
                Facts = new() { "Carved by the Nabataeans from the 4th c. BCE", "The Treasury facade is 39 m tall", "UNESCO World Heritage since 1985" }
            },
            new HeritageSite
            {
                Name = "Machu Picchu", Slug = "machu-picchu", Category = "Monuments", Location = "Cusco Region, Peru",
                HistoricalBackground = "A 15th-century Inca estate built for the emperor Pachacuti, abandoned during the Spanish conquest.",
                ArchitectureSignificance = "More than 200 structures sit on a 2,430 m ridge; the stones are fitted without mortar.",
                CulturalBackground = "UNESCO World Heritage since 1983.",
                Images = new() { Img("machu-picchu", 0), Img("machu-picchu", 1), Img("machu-picchu", 2, ".jpeg"), Img("machu-picchu", 3) },
                Facts = new() { "15th-century Inca estate", "Ridge at 2,430 m", "UNESCO World Heritage since 1983" }
            },
            new HeritageSite
            {
                Name = "Taj Mahal", Slug = "taj-mahal", Category = "Monuments", Location = "Agra, India",
                HistoricalBackground = "Built between 1632 and 1653 by the Mughal emperor Shah Jahan as a tomb for Mumtaz Mahal.",
                ArchitectureSignificance = "White Makrana marble with 35 m-high minarets; the dome rises 73 m above the tomb platform.",
                CulturalBackground = "UNESCO World Heritage since 1983.",
                Images = new() { Img("taj-mahal", 0), Img("taj-mahal", 1), Img("taj-mahal", 2), Img("taj-mahal", 3) },
                Facts = new() { "Built 1632–1653", "White Makrana marble", "UNESCO World Heritage since 1983" }
            },
            new HeritageSite
            {
                Name = "The Great Wall", Slug = "great-wall", Category = "Forts", Location = "Northern China",
                HistoricalBackground = "Sections were built from the 7th century BCE; the Ming wall dates from 1368–1644.",
                ArchitectureSignificance = "The surveyed length is 21,196 km; the Ming section alone runs 8,850 km.",
                CulturalBackground = "UNESCO World Heritage since 1987.",
                Images = new() { Img("great-wall", 0), Img("great-wall", 1), Img("great-wall", 2), Img("great-wall", 3) },
                Facts = new() { "Sections from the 7th c. BCE", "Surveyed length 21,196 km", "UNESCO World Heritage since 1987" }
            },
            new HeritageSite
            {
                Name = "Forbidden City", Slug = "forbidden-city", Category = "Palaces", Location = "Beijing, China",
                HistoricalBackground = "Built between 1406 and 1420 under the Yongle emperor; seat of 24 Ming and Qing emperors.",
                ArchitectureSignificance = "980 surviving buildings arranged on a 961 m north–south axis; marble terraces carry the main halls.",
                CulturalBackground = "UNESCO World Heritage since 1987.",
                Images = new() { Img("forbidden-city", 0), Img("forbidden-city", 1), Img("forbidden-city", 2), Img("forbidden-city", 3) },
                Facts = new() { "Built 1406–1420", "980 buildings survive", "Seat of 24 Ming and Qing emperors" }
            },
            new HeritageSite
            {
                Name = "Topkapı Palace", Slug = "topkapi-palace", Category = "Palaces", Location = "Istanbul, Türkiye",
                HistoricalBackground = "The Ottoman residence from the 1460s; the court moved to Dolmabahçe in 1856.",
                ArchitectureSignificance = "Four courtyards behind the Imperial Gate; the Harem has more than 300 rooms.",
                CulturalBackground = "UNESCO World Heritage since 1985.",
                Images = new() { Img("topkapi-palace", 0) },
                Facts = new() { "Ottoman residence from the 1460s", "Four courtyards behind the Imperial Gate", "UNESCO World Heritage since 1985" },
                IsAssociated = true
            },
            new HeritageSite
            {
                Name = "Acropolis Museum", Slug = "acropolis-museum", Category = "Museums", Location = "Athens, Greece",
                HistoricalBackground = "Opened in 2009 to house finds from the Acropolis slopes and the Parthenon.",
                ArchitectureSignificance = "A glass floor reveals the Makrygianni excavation below; 4,000 objects are on display.",
                CulturalBackground = "The top floor is turned 23° to align with the Parthenon.",
                Images = new() { Img("acropolis-museum", 0), Img("acropolis-museum", 1), Img("acropolis-museum", 2), Img("acropolis-museum", 3) },
                Facts = new() { "Opened 2009", "4,000 objects on display", "Top floor aligns with the Parthenon" },
                IsAssociated = true
            },
            new HeritageSite
            {
                Name = "Colosseum", Slug = "colosseum", Category = "Monuments", Location = "Rome, Italy",
                HistoricalBackground = "Opened in 80 CE under the emperor Titus; built on the site of Nero's Golden House.",
                ArchitectureSignificance = "The arena is 188 m by 156 m; four tiers held about 50,000 spectators.",
                CulturalBackground = "UNESCO World Heritage since 1980 (Historic Centre of Rome).",
                Images = new() { Img("colosseum", 0), Img("colosseum", 1), Img("colosseum", 2) },
                Facts = new() { "Opened 80 CE under Titus", "About 50,000 spectators", "UNESCO World Heritage since 1980" }
            },
            new HeritageSite
            {
                Name = "Chichen Itza", Slug = "chichen-itza", Category = "Monuments", Location = "Yucatán, Mexico",
                HistoricalBackground = "A Maya city that flourished from the 7th to the 13th century, later a pilgrimage centre.",
                ArchitectureSignificance = "El Castillo rises 24 m and is aligned so that sunlight forms a serpent shadow at the equinoxes.",
                CulturalBackground = "UNESCO World Heritage since 1988.",
                Images = new() { Img("chichen-itza", 0), Img("chichen-itza", 1), Img("chichen-itza", 2) },
                Facts = new() { "Maya city from the 7th c.", "El Castillo is 24 m tall", "UNESCO World Heritage since 1988" }
            },
            new HeritageSite
            {
                Name = "Pyramids of Giza", Slug = "giza", Category = "Monuments", Location = "Giza, Egypt",
                HistoricalBackground = "The Great Pyramid was completed around 2560 BCE for the pharaoh Khufu.",
                ArchitectureSignificance = "It rose 146.6 m originally and was the tallest building for nearly 3,800 years.",
                CulturalBackground = "UNESCO World Heritage since 1979.",
                Images = new() { Img("giza", 0), Img("giza", 1), Img("giza", 2) },
                Facts = new() { "Great Pyramid completed c. 2560 BCE", "Original height 146.6 m", "UNESCO World Heritage since 1979" }
            },
            new HeritageSite
            {
                Name = "Stonehenge", Slug = "stonehenge", Category = "Monuments", Location = "Wiltshire, England",
                HistoricalBackground = "Raised in stages between about 3000 and 1500 BCE.",
                ArchitectureSignificance = "Sarsen stones weigh up to 25 t; the heel stone aligns with the midsummer sunrise.",
                CulturalBackground = "UNESCO World Heritage since 1986.",
                Images = new() { Img("stonehenge", 0), Img("stonehenge", 1), Img("stonehenge", 2) },
                Facts = new() { "Raised c. 3000–1500 BCE", "Sarsen stones weigh up to 25 t", "UNESCO World Heritage since 1986" }
            },
            new HeritageSite
            {
                Name = "Borobudur", Slug = "borobudur", Category = "Temples", Location = "Java, Indonesia",
                HistoricalBackground = "Built in the 9th century under the Sailendra dynasty.",
                ArchitectureSignificance = "2,672 relief panels and 504 Buddha statues sit on nine stacked platforms.",
                CulturalBackground = "UNESCO World Heritage since 1991.",
                Images = new() { Img("borobudur", 0), Img("borobudur", 1), Img("borobudur", 2) },
                Facts = new() { "Built in the 9th century", "2,672 relief panels", "UNESCO World Heritage since 1991" }
            },
            new HeritageSite
            {
                Name = "Kinkaku-ji", Slug = "kinkaku-ji", Category = "Temples", Location = "Kyoto, Japan",
                HistoricalBackground = "Founded in 1397 as a retirement villa for the shogun Ashikaga Yoshimitsu.",
                ArchitectureSignificance = "The top two floors are covered in gold leaf and reflect in the Kyōko-chi pond.",
                CulturalBackground = "UNESCO World Heritage since 1994 (Historic Monuments of Ancient Kyoto).",
                Images = new() { Img("kinkaku-ji", 0), Img("kinkaku-ji", 1), Img("kinkaku-ji", 2) },
                Facts = new() { "Founded 1397", "Top two floors covered in gold leaf", "UNESCO World Heritage since 1994" }
            },
            new HeritageSite
            {
                Name = "Alhambra", Slug = "alhambra", Category = "Palaces", Location = "Granada, Spain",
                HistoricalBackground = "The Nasrid palace was built in the 13th and 14th centuries on a Roman-era hill.",
                ArchitectureSignificance = "The Court of the Lions fountain is fed by four channels; carved stucco covers the halls.",
                CulturalBackground = "UNESCO World Heritage since 1984.",
                Images = new() { Img("alhambra", 0), Img("alhambra", 1), Img("alhambra", 2) },
                Facts = new() { "Nasrid palace from the 13th–14th c.", "Court of the Lions fountain", "UNESCO World Heritage since 1984" }
            },
            new HeritageSite
            {
                Name = "Rapa Nui (Easter Island)", Slug = "rapa-nui", Category = "Monuments", Location = "Easter Island, Chile",
                HistoricalBackground = "Polynesian settlers carved the moai between about 1250 and 1500 CE.",
                ArchitectureSignificance = "About 900 moai are recorded; the largest erected stone stands nearly 10 m tall.",
                CulturalBackground = "UNESCO World Heritage since 1995.",
                Images = new() { Img("rapa-nui", 0), Img("rapa-nui", 1), Img("rapa-nui", 2) },
                Facts = new() { "Moai carved c. 1250–1500", "About 900 moai recorded", "UNESCO World Heritage since 1995" }
            },
            new HeritageSite
            {
                Name = "Sigiriya", Slug = "sigiriya", Category = "Forts", Location = "Matale, Sri Lanka",
                HistoricalBackground = "A rock fortress built by King Kashyapa in the 5th century CE.",
                ArchitectureSignificance = "The summit sits about 200 m above the plain; the mirror wall preserves 5th-century graffiti.",
                CulturalBackground = "UNESCO World Heritage since 1982.",
                Images = new() { Img("sigiriya", 0), Img("sigiriya", 1), Img("sigiriya", 2) },
                Facts = new() { "Rock fortress from the 5th c.", "Summit 200 m above the plain", "UNESCO World Heritage since 1982" }
            },
            new HeritageSite
            {
                Name = "Ephesus", Slug = "ephesus", Category = "Monuments", Location = "Selçuk, Türkiye",
                HistoricalBackground = "A Greek city founded around the 10th century BCE and later a Roman provincial capital.",
                ArchitectureSignificance = "The Library of Celsus was built around 110 CE and held about 12,000 scrolls.",
                CulturalBackground = "UNESCO World Heritage since 2015.",
                Images = new() { Img("ephesus", 0), Img("ephesus", 1), Img("ephesus", 2) },
                Facts = new() { "Greek city founded 10th c. BCE", "Library of Celsus built c. 110 CE", "UNESCO World Heritage since 2015" }
            }
        };

        // The story attached to each site (folklore/legend — the human hook for the page).
        var storiesBySlug = new Dictionary<string, string>
        {
            ["angkor-wat"] = "Khmer legend says the temple was raised in one night by a divine architect; the bas-reliefs of the Churning of the Sea of Milk show gods and demons sharing the world's first ocean.",
            ["petra"] = "Bedouin tradition says Al-Khazneh held the pharaoh's treasure — visitors once fired bullets at the great urn, hoping to make gold fall.",
            ["machu-picchu"] = "The Incas knew the site as Pachacuti's royal estate; Hiram Bingham's 1911 expedition introduced the 'lost city' to the world.",
            ["taj-mahal"] = "Stories say Shah Jahan planned a mirror tomb in black marble across the river — cut short when his son imprisoned him at Agra Fort.",
            ["great-wall"] = "In the Meng Jiangnü legend, a woman's tears brought the wall down when she learned her husband had died building it.",
            ["forbidden-city"] = "Popular belief counts 9,999 and a half rooms — half a room short of heaven, so only the Jade Emperor could hold ten thousand.",
            ["topkapi-palace"] = "For four centuries sultans ruled from Topkapi; the Harem's intrigue fuelled Ottoman histories and European novels alike.",
            ["acropolis-museum"] = "The missing Parthenon sculptures remain the museum's open story — casts wait beside empty spaces for the originals' return from London.",
            ["colosseum"] = "Medieval pilgrims said 'as long as the Colosseum stands, Rome stands' — the amphitheatre survived as fortress, quarry and shrine.",
            ["chichen-itza"] = "At each equinox a serpent shadow crawls down El Castillo's stair — the Maya's Kukulkan descending to earth.",
            ["giza"] = "Herodotus wrote that a hundred thousand men built Khufu's pyramid in twenty years; later estimates put the workforce in the tens of thousands.",
            ["stonehenge"] = "Geoffrey of Monmouth had Merlin carry the stones from Ireland by magic; how the 25-tonne sarsens were moved is still debated.",
            ["borobudur"] = "Pilgrims walk ten galleries that tell the life of Buddha — the journey from the base of desire to the summit of enlightenment.",
            ["kinkaku-ji"] = "Yoshimitsu gilded his retirement villa until it glowed above the pond; his son converted it into a Zen temple in his memory.",
            ["alhambra"] = "The last Nasrid king, Boabdil, is said to have sighed at the pass where he last saw Granada — the 'Moor's sigh'.",
            ["rapa-nui"] = "Islanders say the moai walked from the quarry to their platforms; in 2012 researchers moved a replica standing to test the legend.",
            ["sigiriya"] = "Kashyapa built his palace on the lion rock after seizing the throne; the great paws of a carved lion still guard the staircase.",
            ["ephesus"] = "Ephesus held one of the Seven Wonders — the Temple of Artemis — and, by tradition, the tomb of John the Evangelist."
        };
        foreach (var site in sites)
        {
            if (storiesBySlug.TryGetValue(site.Slug, out var story))
            {
                site.Story = story;
            }
        }

        // Footage: any site with a downloaded clip at wwwroot/videos/{slug}.webm gets it.
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        foreach (var site in sites)
        {
            if (File.Exists(Path.Combine(webRoot, "videos", site.Slug + ".webm")))
            {
                site.Video = $"/videos/{site.Slug}.webm";
            }
        }

        await db.Sites.InsertManyAsync(sites);

        var now = DateTime.UtcNow.Date;
        HeritageSite Site(string slug) => sites.First(s => s.Slug == slug);

        await db.Events.InsertManyAsync(new[]
        {
            new Event
            {
                Title = "Temple Walk — Angkor", Slug = "temple-walk-angkor", Type = "walk",
                Description = "Outer-gallery reliefs with a site guide; 2 hours; meet at the west causeway.",
                SiteId = Site("angkor-wat").Id, StartDate = now.AddDays(7).AddHours(7), EndDate = now.AddDays(7).AddHours(9),
                Venue = "Angkor Wat west causeway"
            },
            new Event
            {
                Title = "Petra by Night", Slug = "petra-by-night", Type = "cultural",
                Description = "Candlelit walk through the Siq to the Treasury; 90 minutes.",
                SiteId = Site("petra").Id, StartDate = now.AddDays(11).AddHours(20).AddMinutes(30), EndDate = now.AddDays(11).AddHours(22),
                Venue = "The Siq, Petra"
            },
            new Event
            {
                Title = "Sunrise Walk — Machu Picchu", Slug = "sunrise-walk-machu-picchu", Type = "walk",
                Description = "Sunrise from the Guardhouse; 3 hours; timed entry required.",
                SiteId = Site("machu-picchu").Id, StartDate = now.AddDays(15).AddHours(5).AddMinutes(30), EndDate = now.AddDays(15).AddHours(8).AddMinutes(30),
                Venue = "Machu Picchu entrance"
            },
            new Event
            {
                Title = "Mughal Marble Talk", Slug = "mughal-marble-talk", Type = "awareness",
                Description = "Talk on marble inlay and the Taj Mahal workshops; 60 minutes.",
                SiteId = Site("taj-mahal").Id, StartDate = now.AddDays(21).AddHours(15), EndDate = now.AddDays(21).AddHours(16),
                Venue = "Agra heritage centre"
            },
            new Event
            {
                Title = "Colosseum Night Walk", Slug = "colosseum-night-walk", Type = "cultural",
                Description = "Evening walk under the arches; 75 minutes.",
                SiteId = Site("colosseum").Id, StartDate = now.AddDays(9).AddHours(19).AddMinutes(30), EndDate = now.AddDays(9).AddHours(21),
                Venue = "Colosseum south entrance"
            },
            new Event
            {
                Title = "Stonehenge Solstice Watch", Slug = "stonehenge-solstice-watch", Type = "cultural",
                Description = "Sunrise alignment watch inside the stone circle; 2 hours.",
                SiteId = Site("stonehenge").Id, StartDate = now.AddDays(18).AddHours(5), EndDate = now.AddDays(18).AddHours(7),
                Venue = "Stonehenge visitor centre"
            },
            new Event
            {
                Title = "Giza Plateau Walk", Slug = "giza-plateau-walk", Type = "walk",
                Description = "Pyramid plateau walk with an Egyptologist; 2.5 hours.",
                SiteId = Site("giza").Id, StartDate = now.AddDays(24).AddHours(6).AddMinutes(30), EndDate = now.AddDays(24).AddHours(9),
                Venue = "Giza plateau gate"
            },
            new Event
            {
                Title = "Ming Wall Walk", Slug = "ming-wall-walk", Type = "walk",
                Description = "A conducted walk on the Mutianyu section, held last month.",
                SiteId = Site("great-wall").Id, StartDate = now.AddDays(-20).AddHours(8), EndDate = now.AddDays(-20).AddHours(11),
                Venue = "Mutianyu, Great Wall"
            },
            new Event
            {
                Title = "Alhambra Night Tour", Slug = "alhambra-night-tour", Type = "cultural",
                Description = "Night visit of the Nasrid palaces, conducted two weeks ago.",
                SiteId = Site("alhambra").Id, StartDate = now.AddDays(-14).AddHours(21), EndDate = now.AddDays(-14).AddHours(22).AddMinutes(30),
                Venue = "Alhambra, Granada"
            },
            new Event
            {
                Title = "Borobudur Sunrise Walk", Slug = "borobudur-sunrise-walk", Type = "walk",
                Description = "Sunrise from the upper terraces, conducted last month.",
                SiteId = Site("borobudur").Id, StartDate = now.AddDays(-30).AddHours(5), EndDate = now.AddDays(-30).AddHours(7),
                Venue = "Borobudur temple"
            }
        });

        await db.Gallery.InsertManyAsync(new[]
        {
            new GalleryItem { Caption = "Angkor Wat, Cambodia", ImageUrl = Img("angkor-wat", 1), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Petra, Jordan", ImageUrl = Img("petra", 2), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Machu Picchu, Peru", ImageUrl = Img("machu-picchu", 1), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Taj Mahal, India", ImageUrl = Img("taj-mahal", 3), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Mutianyu, Great Wall", ImageUrl = Img("great-wall", 2), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Colosseum, Rome", ImageUrl = Img("colosseum", 0), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Chichen Itza, Mexico", ImageUrl = Img("chichen-itza", 1), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Kinkaku-ji, Kyoto", ImageUrl = Img("kinkaku-ji", 0), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Borobudur, Java", ImageUrl = Img("borobudur", 1), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Alhambra, Granada", ImageUrl = Img("alhambra", 2), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Rapa Nui moai, Chile", ImageUrl = Img("rapa-nui", 1), RefType = "site", Credit = "Wikimedia Commons" },
            new GalleryItem { Caption = "Sigiriya, Sri Lanka", ImageUrl = Img("sigiriya", 2), RefType = "site", Credit = "Wikimedia Commons" }
        });

        await db.AboutPages.InsertManyAsync(new[]
        {
            new AboutPage { Slug = "what-we-do", Title = "What we do", Content = "We run heritage walks, talks and exhibitions at sites across the world, and publish the facts behind each site." },
            new AboutPage { Slug = "mission", Title = "Our mission", Content = "Make heritage sites legible: what survives, what it means, and when you can see it." },
            new AboutPage { Slug = "team", Title = "Our team", Content = "Six guides and two researchers; programme staff work with local site authorities." },
            new AboutPage { Slug = "achievements", Title = "Our achievements", Content = "Since 2019: 64 walks, 12 exhibitions, 9 talks delivered with site partners." },
            new AboutPage { Slug = "supporters", Title = "Our supporters", Content = "Programmes are supported by member contributions and site-partner in-kind support." },
            new AboutPage { Slug = "read-about-us", Title = "Read about us", Content = "A short history of the organisation, its programmes and its partners." }
        });

        await db.ContactInfos.InsertOneAsync(new ContactInfo
        {
            Address = "HeritageTrail, 12 Heritage Lane, London",
            Phone = "+44 20 0000 0000",
            Email = "hello@heritagetrail.local",
            Note = "Office hours: Mon–Fri 09:00–17:00."
        });

        logger.LogInformation("Seed complete: {Sites} sites, {Events} events", sites.Length, 10);
    }
}
