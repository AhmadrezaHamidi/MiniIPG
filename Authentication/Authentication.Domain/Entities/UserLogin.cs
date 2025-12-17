
namespace Authentication.Domain.Entities;

public class UserLogin : IdentityUserLogin<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
