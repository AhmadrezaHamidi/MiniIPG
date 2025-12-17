namespace Authentication.Domain.Entities;

public class UserToken : IdentityUserToken<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
