namespace Services.User.DTOs;

public class UserDtoCreateRequest
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    public string AvatarName { get; set; }
    public bool IsBarber { get; set; } = false;
}
