using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public interface ICartService
{
    event Action? OnChange;
    Task<List<CartItem>> GetCartAsync();
    Task AddToCartAsync(Product product, int count = 1);
    Task IncrementAsync(int productId);
    Task DecrementAsync(int productId);
    Task RemoveAsync(int productId);
    Task ClearCartAsync();
    Task<int> GetCartCountAsync();
    Task<double> GetCartTotalAsync();
}
