using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public interface ICartService
{
    event Action? OnChange;
    Task<List<CartItem>> GetCartAsync();
    Task AddToCartAsync(Product product, int count = 1, string? engraving = null);
    Task IncrementAsync(int cartItemId);
    Task DecrementAsync(int cartItemId);
    Task RemoveAsync(int cartItemId);
    Task UpdateEngravingAsync(int cartItemId, string? engraving, int? applyTo = null);
    Task AddEngravingMessageAsync(int productId, string engraving);
    Task AddUnitAsync(int productId);
    Task RemoveUnitAsync(int productId);
    Task ClearCartAsync();
    Task<int> GetCartCountAsync();
    Task<double> GetCartTotalAsync();
}
