
namespace Authentication.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Local"));
        });
        
        services.AddScoped<IUserService, UserService>();

        services.AddIdentity<User, Role>(option => option.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<IdentityDbContext>()
                               .AddDefaultTokenProviders();

        services.AddScoped<ITokenInfoService, TokenInfoService>();


        var jwtSettings = new JwtSettings();
        configuration.Bind(key: nameof(jwtSettings), jwtSettings);
        services.AddSingleton(jwtSettings);

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            options.User.RequireUniqueEmail = false;
            options.Lockout.AllowedForNewUsers = false;
            options.SignIn.RequireConfirmedEmail = false;
        });


        services.AddScoped<AppUserManager>();
        services.AddScoped<AppSignInManager>();
        services.AddScoped<AppRoleManager>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                  JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme =
                  JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
       {
           options.RequireHttpsMetadata = false;
           options.SaveToken = true;
           options.ClaimsIssuer = configuration["Authentication:JwtIssuer"];

           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidIssuer = configuration["Authentication:JwtIssuer"],

               ValidateAudience = true,
               ValidAudience = configuration["Authentication:JwtAudience"],

               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtKey"])),
               RequireExpirationTime = true,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero
           };
       });


        services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromDays(3));

        return services;
    }
}