using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace HeritageTrail.Web.Services;

/// <summary>Seeds global-heritage content. Staging = Reset (drops), Production = Once (only when empty).</summary>
public static class SeedService
{
    public static async Task RunAsync(MongoContext db, EnvironmentConfig config, ILogger logger)
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
                Name = "Angkor Wat", Slug = "angkor-wat", Category = "Temples",
                Location = "Siem Reap, Cambodia",
                HistoricalBackground = "Built in the 12th century under King Suryavarman II as a Hindu temple dedicated to Vishnu.",
                ArchitectureSignificance = "Five towers stand for Mount Meru; the moat is 190 m wide and the outer gallery holds more than 1,000 metres of bas-reliefs.",
                CulturalBackground = "UNESCO World Heritage since 1992; the temple remains a living Buddhist site."
            },
            new HeritageSite
            {
                Name = "Petra", Slug = "petra", Category = "Monuments",
                Location = "Ma'an, Jordan",
                HistoricalBackground = "The Nabataean capital was carved into sandstone from the 4th century BCE and annexed by Rome in 106 CE.",
                ArchitectureSignificance = "The Treasury facade is 39 m tall, cut directly into the cliff; the Siq is a 1.2 km natural gorge.",
                CulturalBackground = "UNESCO World Heritage since 1985."
            },
            new HeritageSite
            {
                Name = "Machu Picchu", Slug = "machu-picchu", Category = "Monuments",
                Location = "Cusco Region, Peru",
                HistoricalBackground = "A 15th-century Inca estate built for the emperor Pachacuti, abandoned during the Spanish conquest.",
                ArchitectureSignificance = "More than 200 structures sit on a 2,430 m ridge; the stones are fitted without mortar.",
                CulturalBackground = "UNESCO World Heritage since 1983."
            },
            new HeritageSite
            {
                Name = "Taj Mahal", Slug = "taj-mahal", Category = "Monuments",
                Location = "Agra, India",
                HistoricalBackground = "Built between 1632 and 1653 by the Mughal emperor Shah Jahan as a tomb for Mumtaz Mahal.",
                ArchitectureSignificance = "White Makrana marble with 35 m-high minarets; the dome rises 73 m above the tomb platform.",
                CulturalBackground = "UNESCO World Heritage since 1983."
            },
            new HeritageSite
            {
                Name = "The Great Wall", Slug = "great-wall", Category = "Forts",
                Location = "Northern China",
                HistoricalBackground = "Sections were built from the 7th century BCE; the Ming wall dates from 1368–1644.",
                ArchitectureSignificance = "The surveyed length is 21,196 km; the Ming section alone runs 8,850 km.",
                CulturalBackground = "UNESCO World Heritage since 1987."
            },
            new HeritageSite
            {
                Name = "Forbidden City", Slug = "forbidden-city", Category = "Palaces",
                Location = "Beijing, China",
                HistoricalBackground = "Built between 1406 and 1420 under the Yongle emperor; seat of 24 Ming and Qing emperors.",
                ArchitectureSignificance = "980 surviving buildings arranged on a 961 m north–south axis; marble terraces carry the main halls.",
                CulturalBackground = "UNESCO World Heritage since 1987."
            },
            new HeritageSite
            {
                Name = "Topkapı Palace", Slug = "topkapi-palace", Category = "Palaces",
                Location = "Istanbul, Türkiye",
                HistoricalBackground = "The Ottoman residence from the 1460s; the court moved to Dolmabahçe in 1856.",
                ArchitectureSignificance = "Four courtyards behind the Imperial Gate; the Harem has more than 300 rooms.",
                CulturalBackground = "UNESCO World Heritage since 1985."
            },
            new HeritageSite
            {
                Name = "Acropolis Museum", Slug = "acropolis-museum", Category = "Museums",
                Location = "Athens, Greece",
                HistoricalBackground = "Opened in 2009 to house finds from the Acropolis slopes and the Parthenon.",
                ArchitectureSignificance = "A glass floor reveals the Makrygianni excavation below; 4,000 objects are on display.",
                CulturalBackground = "The top floor is turned 23° to align with the Parthenon."
            }
        };
        await db.Sites.InsertManyAsync(sites);

        // Meaningful, non-obvious facts per site (shown in the detail sidebar).
        var factsBySlug = new Dictionary<string, string[]>
        {
            ["angkor-wat"] = new[] { "Built in the 12th century under Suryavarman II", "The moat is 190 m wide", "UNESCO World Heritage since 1992" },
            ["petra"] = new[] { "Carved by the Nabataeans from the 4th c. BCE", "The Treasury facade is 39 m tall", "UNESCO World Heritage since 1985" },
            ["machu-picchu"] = new[] { "15th-century Inca estate", "Ridge at 2,430 m", "UNESCO World Heritage since 1983" },
            ["taj-mahal"] = new[] { "Built 1632–1653", "White Makrana marble", "UNESCO World Heritage since 1983" },
            ["great-wall"] = new[] { "Sections from the 7th c. BCE", "Surveyed length 21,196 km", "UNESCO World Heritage since 1987" },
            ["forbidden-city"] = new[] { "Built 1406–1420", "980 buildings survive", "Seat of 24 Ming and Qing emperors" },
            ["topkapi-palace"] = new[] { "Ottoman residence from the 1460s", "Four courtyards behind the Imperial Gate", "UNESCO World Heritage since 1985" },
            ["acropolis-museum"] = new[] { "Opened 2009", "4,000 objects on display", "Top floor aligns with the Parthenon" }
        };
        foreach (var (slug, facts) in factsBySlug)
        {
            await db.Sites.UpdateOneAsync(
                s => s.Slug == slug,
                Builders<HeritageSite>.Update.Set(s => s.Facts, facts.ToList()));
        }

        // Locations associated with the organisation (T12/T17).
        await db.Sites.UpdateManyAsync(
            s => s.Slug == "topkapi-palace" || s.Slug == "acropolis-museum",
            Builders<HeritageSite>.Update.Set(s => s.IsAssociated, true));

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
                Title = "Ming Wall Walk", Slug = "ming-wall-walk", Type = "walk",
                Description = "A conducted walk on the Mutianyu section, held last month.",
                SiteId = Site("great-wall").Id, StartDate = now.AddDays(-20).AddHours(8), EndDate = now.AddDays(-20).AddHours(11),
                Venue = "Mutianyu, Great Wall"
            }
        });

        await db.Gallery.InsertManyAsync(sites.Take(4).Select((s, i) => new GalleryItem
        {
            Caption = $"{s.Name} — {s.Location}",
            RefType = "site", RefId = s.Id, Credit = "Image slot — treatment pipeline"
        }));

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

        logger.LogInformation("Seed complete: {Sites} sites, {Events} events", sites.Length, 5);
    }
}
