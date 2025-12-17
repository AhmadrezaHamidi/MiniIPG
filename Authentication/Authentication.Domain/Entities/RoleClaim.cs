
namespace Authentication.Domain.Entities;

public class RoleClaim : IdentityRoleClaim<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
