using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public interface IAuthService
{
    event Action? OnChange;
    Task<AppUser?> GetCurrentUserAsync();
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsInRoleAsync(string role);
    List<AppUser> GetDemoUsers();
}
