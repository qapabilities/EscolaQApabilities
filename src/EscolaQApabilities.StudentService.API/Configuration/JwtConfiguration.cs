namespace EscolaQApabilities.StudentService.API.Configuration;

public class JwtConfiguration
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; } = 60;

    public void Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Key))
        {
            errors.Add("JWT Key is required");
        }
        else if (Key.Length < 32)
        {
            errors.Add($"JWT Key must be at least 32 characters long. Current length: {Key.Length}");
        }

        if (string.IsNullOrWhiteSpace(Issuer))
        {
            errors.Add("JWT Issuer is required");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            errors.Add("JWT Audience is required");
        }

        if (ExpiryInMinutes <= 0)
        {
            errors.Add("JWT ExpiryInMinutes must be a positive integer");
        }

        if (errors.Any())
        {
            throw new InvalidOperationException($"JWT configuration validation failed: {string.Join("; ", errors)}");
        }
    }
}

