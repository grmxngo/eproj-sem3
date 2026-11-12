namespace HeritageTrail.Web.Services;

/// <summary>Writes outgoing mail as .eml files per environment (mock SMTP; proof of invite/reply flows).</summary>
public sealed class MailOutbox
{
    private readonly string _dir;

    public MailOutbox(IWebHostEnvironment env, EnvironmentConfig config)
    {
        var root = WorkspaceRoot(env);
        _dir = Path.Combine(root, config.MailOutbox);
        Directory.CreateDirectory(_dir);
    }

    public string OutboxDirectory => _dir;

    public void Send(string to, string subject, string body)
    {
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff");
        File.WriteAllText(
            Path.Combine(_dir, $"{stamp}.eml"),
            $"To: {to}\nSubject: {subject}\nSentAt: {DateTime.UtcNow:O}\n\n{body}\n");
    }

    internal static string WorkspaceRoot(IWebHostEnvironment env) =>
        Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "..", ".."));
}

/// <summary>Per-environment upload folder behind /uploads (static file provider in Program).</summary>
public sealed class UploadsStore
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private readonly string _dir;

    public UploadsStore(IWebHostEnvironment env, EnvironmentConfig config)
    {
        var root = MailOutbox.WorkspaceRoot(env);
        _dir = Path.Combine(root, config.Uploads);
        Directory.CreateDirectory(_dir);
    }

    public string UploadDirectory => _dir;

    /// <summary>Saves an uploaded image; returns the public URL, or null with a reason on rejection.</summary>
    public async Task<(string? Url, string? Error)> SaveAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            return (null, "Only jpg, png and webp images are allowed.");
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return (null, "Image must be 5 MB or smaller.");
        }

        var name = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(_dir, name);
        await using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream);
        return ($"/uploads/{name}", null);
    }
}
