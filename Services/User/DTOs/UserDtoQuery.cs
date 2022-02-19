using Models.Helpers;

namespace Services.User.DTOs;

public class UserDtoQuery
{
    public string DisplayName { get; set; }
    public string Username { get; set; }
    public bool IsBarber { get; set; }
    public AvatarDto Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
