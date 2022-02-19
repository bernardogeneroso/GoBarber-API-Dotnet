namespace Services.User.DTOs;

public class UserDtoRequest
{
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string LatestPassword { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}
