namespace Authentication.Infrastructure.Services;


public class UserService : IUserService
{
    private readonly AppUserManager _userManager;
    private readonly AppSignInManager _signInManager;
    private readonly ITokenInfoService _tokenService;

    public UserService(
        AppUserManager userManager,
        AppSignInManager signInManager,
        ITokenInfoService tokenInfoService
       )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenInfoService;
    }

    public async Task<Result<TokenDto>> GetTokenAsync(string username, string password)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<TokenDto>.Failure("نام کاربری یا رمز عبور اشتباه است");

            if (user.LockoutEnabled)
                return Result<TokenDto>.Failure("حساب کاربری شما قفل شده است");

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, true, false);
            if (!signInResult.Succeeded)
                return Result<TokenDto>.Failure("نام کاربری یا رمز عبور اشتباه است");

            var token = await _tokenService.GenerateToken(user);
            return Result<TokenDto>.Success(token);
        }
        catch (Exception ex)
        {
            return Result<TokenDto>.Failure("خطا در احراز هویت");
        }
    }

    public async Task<Result<TokenDto>> GetRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
                return Result<TokenDto>.Failure("توکن نامعتبر است");

            var token = await _tokenService.GenerateToken(user);
            return Result<TokenDto>.Success(token);
        }
        catch (Exception ex)
        {
            return Result<TokenDto>.Failure("خطا در تازه‌سازی توکن");
        }
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return Result<UserDto>.Failure("کاربر مورد نظر یافت نشد");

            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = string.Join(",", userRoles);
            var userDto = new UserDto(user.Id, user.UserName, user.FirstName, user.LastName, roles);

            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure("خطا در دریافت اطلاعات کاربر");
        }
    }

    public async Task<Result<List<UserDto>>> GetAllAsync()
    {
        try
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .ToListAsync();
            List<UserDto> usersResult = new();
            foreach (var user in users)
            {
                var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
                if (appUser != null)
                {
                    var roles = await _userManager.GetRolesAsync(appUser);
                    usersResult.Add(new UserDto(appUser.Id, appUser.UserName ,appUser.FirstName, appUser.LastName, string.Join(",", roles)));
                }
            }
            return Result<List<UserDto>>.Success(usersResult);
        }
        catch (Exception ex)
        {
            return Result<List<UserDto>>.Failure("خطا در دریافت لیست کاربران");
        }
    }

    public async Task<Result<int>> CreateAsync(UserCreateDto userCreateDto, string role)
    {
        try
        {
            var user = new User
            {
                UserName = userCreateDto.UserName,
                Email = userCreateDto.Email,
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                CreatedAt = DateTime.Now,
            };
            user.SecurityStamp = Guid.NewGuid().ToString();

            var createResult = await _userManager.CreateAsync(user, userCreateDto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return Result<int>.Failure($"خطا در ایجاد کاربر: {errors}");
            }

            // افزودن نقش
            if (!string.IsNullOrEmpty(role))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                    await _userManager.DeleteAsync(user);
                    return Result<int>.Failure($"خطا در تخصیص نقش: {errors}");
                }
            }

            return Result<int>.Success(user.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure("خطا در ایجاد کاربر جدید");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return Result<bool>.Failure("کاربر مورد نظر یافت نشد");

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                return Result<bool>.Failure($"خطا در حذف کاربر: {errors}");
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure("خطا در حذف کاربر");
        }
    }

    public async Task<Result<bool>> UpdateAsync(UserUpdateDto userUpdateDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userUpdateDto.Id.ToString());
            if (user == null)
                return Result<bool>.Failure("کاربر مورد نظر یافت نشد");

            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            user.Email = userUpdateDto.Email;

            // اگر رمز عبور تغییر کرده باشد
            if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, userUpdateDto.Password);

                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    return Result<bool>.Failure($"خطا در تغییر رمز عبور: {errors}");
                }

                // تغییر SecurityStamp بعد از تغییر رمز عبور
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            // آپدیت کاربر
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<bool>.Failure($"خطا در بروزرسانی کاربر: {errors}");
            }

            // آپدیت نقش‌ها (اگر نیاز باشد)
            //if (userUpdateDto.Respassword != null && userUpdateDto.Roles.Any())
            //{
            //    var currentRoles = await _userManager.GetRolesAsync(user);
            //    await _userManager.RemoveFromRolesAsync(user, currentRoles);
            //    await _userManager.AddToRolesAsync(user, userUpdateDto.Roles);
            //}

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure("خطا در بروزرسانی کاربر");
        }
    }
}

