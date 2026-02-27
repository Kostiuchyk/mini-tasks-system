namespace MiniTasksSystem.Domain.Users;

public sealed class RefreshToken
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
