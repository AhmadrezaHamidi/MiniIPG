
namespace Authentication.Domain.Entities;

public class UserRole : IdentityUserRole<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
