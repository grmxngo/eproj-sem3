using HeritageTrail.Web.Data;
using HeritageTrail.Web.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace HeritageTrail.Web.Services;

public sealed class AuthService
{
    private readonly MongoContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(MongoContext db) => _db = db;

    public async Task<(User? User, string? Error)> RegisterAsync(
        string name, string email, string password,
        string? phone, string? address, string? occupation, string? organization)
    {
        email = email.Trim().ToLowerInvariant();
        if (await _db.Users.CountDocumentsAsync(u => u.Email == email) > 0)
        {
            return (null, "Email already registered.");
        }

        var user = new User
        {
            Name = name.Trim(),
            Email = email,
            Phone = phone,
            Address = address,
            Occupation = occupation,
            Organization = organization,
            Role = "user"
        };
        user.PasswordHash = _hasher.HashPassword(user, password);
        await _db.Users.InsertOneAsync(user);
        return (user, null);
    }

    public async Task<User?> ValidateAsync(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();
        var user = await _db.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user is null) return null;
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }

    public async Task<User?> FindAsync(string id) =>
        await _db.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> FindByEmailAsync(string email) =>
        await _db.Users.Find(u => u.Email == email.Trim().ToLowerInvariant()).FirstOrDefaultAsync();
}
