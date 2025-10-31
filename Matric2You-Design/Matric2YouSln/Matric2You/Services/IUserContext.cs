namespace Matric2You.Services;

public interface IUserContext
{
 Task<int?> GetUserIdAsync(CancellationToken ct = default);
 Task<string?> GetTokenAsync(CancellationToken ct = default);
}

public sealed class UserContext : IUserContext
{
 private const string TokenKey = "auth_token";
 private const string UserIdKey = "user_id";

 public async Task<int?> GetUserIdAsync(CancellationToken ct = default)
 {
 var s = await SecureStorage.GetAsync(UserIdKey);
 return int.TryParse(s, out var id) ? id : null;
 }

 public Task<string?> GetTokenAsync(CancellationToken ct = default)
 => SecureStorage.GetAsync(TokenKey);
}
