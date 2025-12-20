using Microsoft.Extensions.Logging;

namespace Authentication.Infrastructure.Manageres;

public class AppRoleManager : RoleManager<Role>
{
    public AppRoleManager(IRoleStore<Role> store
        , IEnumerable<IRoleValidator<Role>> roleValidators
        , ILookupNormalizer keyNormalizer
        , IdentityErrorDescriber errors
        , ILogger<RoleManager<Role>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }
}
