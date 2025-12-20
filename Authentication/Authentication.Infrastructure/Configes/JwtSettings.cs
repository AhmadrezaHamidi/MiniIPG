namespace Authentication.Infrastructure.Configes;

public class JwtSettings
{
    public TimeSpan TokenLifetime { get; set; }
    public string Secret { get; set; }
}
