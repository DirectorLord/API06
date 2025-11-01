namespace E_Commerce.Infrastructure.Service;

public class JWTOptions
{
    public static string SectionName { get; set; } = "JWTOptions";
    public string key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int DurationInDays { get; set; }
}
