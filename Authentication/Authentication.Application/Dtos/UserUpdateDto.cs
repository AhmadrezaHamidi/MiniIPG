
namespace Authentication.Application.Dtos;

public record UserUpdateDto
{
    public int Id { get; set; }

    [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نیست")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن نام الزامی است")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "وارد کردن نام خوانوادگی الزامی است")]
    public string LastName { get; set; } = string.Empty;

    // Password اختیاریه (کاربر ممکنه نخواد عوض کنه)
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "رمز عبور و تایید رمز عبور مطابقت ندارند")]
    public string? Respassword { get; set; }

    public UserUpdateDto() { }

    public UserUpdateDto(int id, string email, string firstName, string lastName, string? password = null, string? respassword = null)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        Respassword = respassword;
    }
    public UpdateUserCommand ToCommand() => new(this);
}
