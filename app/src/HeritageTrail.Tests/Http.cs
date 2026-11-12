using System.Text.RegularExpressions;

namespace HeritageTrail.Tests;

public static class Http
{
    public static async Task<string> TokenAsync(HttpClient client, string path)
    {
        var html = await client.GetStringAsync(path);
        var m = Regex.Match(html, "__RequestVerificationToken[^>]*value=\"([^\"]+)\"");
        if (!m.Success)
        {
            throw new InvalidOperationException($"No antiforgery token found at {path}");
        }

        return m.Groups[1].Value;
    }

    public static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string email, string password)
    {
        var token = await TokenAsync(client, "/auth/login");
        return await client.PostAsync("/auth/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["email"] = email,
            ["password"] = password
        }));
    }

    public static async Task<string> BodyAsync(this HttpResponseMessage response) =>
        await response.Content.ReadAsStringAsync();
}
