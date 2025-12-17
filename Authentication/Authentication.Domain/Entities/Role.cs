

namespace Authentication.Domain.Entities;

public class Role: IdentityRole<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
