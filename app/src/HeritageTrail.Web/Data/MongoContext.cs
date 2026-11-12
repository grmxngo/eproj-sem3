using HeritageTrail.Web.Models;
using HeritageTrail.Web.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HeritageTrail.Web.Data;

/// <summary>One Mongo database per environment; instance runs on the port from environments.json.</summary>
public sealed class MongoContext
{
    private readonly MongoClient _client;

    public MongoContext(EnvironmentConfig config)
    {
        DatabaseName = config.Database;
        var settings = MongoClientSettings.FromConnectionString($"mongodb://127.0.0.1:{config.MongoPort}");
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(2);
        settings.ConnectTimeout = TimeSpan.FromSeconds(2);
        _client = new MongoClient(settings);
    }

    public string DatabaseName { get; }

    public IMongoDatabase Database => _client.GetDatabase(DatabaseName);

    public IMongoCollection<User> Users => Database.GetCollection<User>("users");
    public IMongoCollection<Category> Categories => Database.GetCollection<Category>("categories");
    public IMongoCollection<HeritageSite> Sites => Database.GetCollection<HeritageSite>("sites");
    public IMongoCollection<Event> Events => Database.GetCollection<Event>("events");
    public IMongoCollection<EventInterest> Interests => Database.GetCollection<EventInterest>("interests");
    public IMongoCollection<GalleryItem> Gallery => Database.GetCollection<GalleryItem>("gallery");
    public IMongoCollection<AboutPage> AboutPages => Database.GetCollection<AboutPage>("aboutPages");
    public IMongoCollection<Query> Queries => Database.GetCollection<Query>("queries");
    public IMongoCollection<Invitation> Invitations => Database.GetCollection<Invitation>("invitations");
    public IMongoCollection<ContactMessage> ContactMessages => Database.GetCollection<ContactMessage>("contactMessages");
    public IMongoCollection<ContactInfo> ContactInfos => Database.GetCollection<ContactInfo>("contactInfos");

    public bool Ping()
    {
        try
        {
            Database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task EnsureIndexesAsync()
    {
        await Users.Indexes.CreateOneAsync(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }));
        await Events.Indexes.CreateOneAsync(new CreateIndexModel<Event>(
            Builders<Event>.IndexKeys.Ascending(e => e.Slug),
            new CreateIndexOptions { Unique = true }));
        await Sites.Indexes.CreateOneAsync(new CreateIndexModel<HeritageSite>(
            Builders<HeritageSite>.IndexKeys.Ascending(s => s.Slug),
            new CreateIndexOptions { Unique = true }));
        await Interests.Indexes.CreateOneAsync(new CreateIndexModel<EventInterest>(
            Builders<EventInterest>.IndexKeys.Ascending(i => i.EventId).Ascending(i => i.UserId),
            new CreateIndexOptions { Unique = true }));
        await AboutPages.Indexes.CreateOneAsync(new CreateIndexModel<AboutPage>(
            Builders<AboutPage>.IndexKeys.Ascending(a => a.Slug),
            new CreateIndexOptions { Unique = true }));
    }
}
