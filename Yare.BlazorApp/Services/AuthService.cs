using Blazored.LocalStorage;
using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    public event Action? OnChange;

    private static readonly List<AppUser> _demoUsers = new()
    {
        new AppUser
        {
            Id = "admin-001",
            Email = "admin@yare.com",
            Password = "Admin123!",
            Role = SD.Role_MasterAdmin,
            FirstName = "Alex",
            LastName = "Yare",
            PhoneNumber = "07700900001",
            StreetAdress = "1 Luxury Lane",
            City = "London",
            Borough = "Westminster",
            PostCode = "SW1A 1AA"
        },
        new AppUser
        {
            Id = "emp-001",
            Email = "employee@yare.com",
            Password = "Employee123!",
            Role = SD.Role_Employee,
            FirstName = "Sam",
            LastName = "Stone",
            PhoneNumber = "07700900002",
            StreetAdress = "22 Store Street",
            City = "London",
            Borough = "Camden",
            PostCode = "WC1E 7BT"
        },
        new AppUser
        {
            Id = "cust-001",
            Email = "customer@yare.com",
            Password = "Customer123!",
            Role = SD.Role_Customer,
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "07700900003",
            StreetAdress = "45 High Street",
            City = "London",
            Borough = "Southwark",
            PostCode = "SE1 7PB"
        },
    };

    public AuthService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public List<AppUser> GetDemoUsers() => _demoUsers;

    public async Task<AppUser?> GetCurrentUserAsync()
        => await _localStorage.GetItemAsync<AppUser>(SD.UserKey);

    public async Task<bool> LoginAsync(string email, string password)
    {
        var user = _demoUsers.FirstOrDefault(u =>
            u.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true &&
            u.Password == password);
        if (user == null) return false;
        await _localStorage.SetItemAsync(SD.UserKey, user);
        OnChange?.Invoke();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(SD.UserKey);
        OnChange?.Invoke();
    }

    public async Task<bool> IsAuthenticatedAsync()
        => await GetCurrentUserAsync() != null;

    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        return user?.Role == role;
    }
}
