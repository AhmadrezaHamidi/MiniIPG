using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authentication.Infrastructure.Services
{
    public class TokenInfoService : ITokenInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration iconfiguration;
        private readonly IdentityDbContext _db;
        public TokenInfoService(IdentityDbContext db
            , IHttpContextAccessor httpContextAccessor
            , UserManager<User> userManager,
            IConfiguration iconfiguration
            )
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            this.iconfiguration = iconfiguration;
            _db = db;
        }

        public long GetCurrentUserId()
        {
            return long.Parse(_httpContextAccessor.HttpContext.User.FindFirst("UserId").Value);
        }


        public async Task<TokenDto> GenerateToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var splitedUserRoles = userRoles.Aggregate((s, p) => s + "," + p);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,splitedUserRoles),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };




            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iconfiguration["Authentication:JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(iconfiguration["Authentication:JwtExpireMins"]));

            var token = new JwtSecurityToken(
                iconfiguration["Authentication:JwtIssuer"],
                iconfiguration["Authentication:JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user).ConfigureAwait(false);


            var AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refTokenDto = new TokenDto(AccessToken, refreshToken, user.Id, user.Email, splitedUserRoles);

            return refTokenDto;
        }

        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = iconfiguration["Authentication:JwtIssuer"],

                ValidateAudience = true,
                ValidAudience = iconfiguration["Authentication:JwtAudience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iconfiguration["Authentication:JwtKey"])),
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

    }
}
