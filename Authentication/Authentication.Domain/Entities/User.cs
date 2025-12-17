
namespace Authentication.Domain.Entities;

public class User : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RefreshToken { get; set; } = "";
}
