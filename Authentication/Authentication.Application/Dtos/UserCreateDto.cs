


namespace Authentication.Application.Dtos;

public record UserCreateDto
{
    [Required(ErrorMessage = "وارد کردن نام کاربری الزامی است")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نیست")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن نام الزامی است")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن نام خوانوادگی الزامی است")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن تأیید رمز عبور الزامی است")]
    [Compare("Password", ErrorMessage = "رمز عبور و تأیید رمز عبور باید مطابقت داشته باشند")]
    public string RePassword { get; set; } = string.Empty;

    public UserCreateDto() { }
    public UserCreateDto(string userName, string email, string firstName, string lastName, string password, string rePassword)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        RePassword = rePassword;
    }
    public CreateUserCommand ToCommand(string role) => new(this, role);
}

