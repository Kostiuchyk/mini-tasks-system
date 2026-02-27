namespace MiniTasksSystem.Api.Endpoints.Users;

public sealed record UserResponse(string Id, string FullName, string Email, string Role);
