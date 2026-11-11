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

    public IMongoCollection<T> Collection<T>(string name) => Database.GetCollection<T>(name);

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
}
