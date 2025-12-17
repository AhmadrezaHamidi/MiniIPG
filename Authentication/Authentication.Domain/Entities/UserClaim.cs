
namespace Authentication.Domain.Entities;

public class UserClaim : IdentityUserClaim<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
