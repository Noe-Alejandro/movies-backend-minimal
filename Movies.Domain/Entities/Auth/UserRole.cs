namespace Movies.Domain.Entities.Auth;

public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public UserAuth User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}
