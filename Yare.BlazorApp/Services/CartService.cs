using Blazored.LocalStorage;
using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    public event Action? OnChange;

    public CartService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<CartItem>> GetCartAsync()
    {
        return await _localStorage.GetItemAsync<List<CartItem>>(SD.CartKey) ?? new();
    }

    public async Task AddToCartAsync(Product product, int count = 1)
    {
        var cart = await GetCartAsync();
        var existing = cart.FirstOrDefault(c => c.ProductId == product.Id);
        if (existing != null)
            existing.Count += count;
        else
            cart.Add(new CartItem
            {
                Id = cart.Any() ? cart.Max(c => c.Id) + 1 : 1,
                ProductId = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Count = count,
                ImageUrl = product.PrimaryDisplayImageUrl,
                ProductCategory = product.ProductCategory?.ToString()
            });
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task IncrementAsync(int productId)
    {
        var cart = await GetCartAsync();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item != null) item.Count++;
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task DecrementAsync(int productId)
    {
        var cart = await GetCartAsync();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item != null)
        {
            if (item.Count <= 1)
                cart.Remove(item);
            else
                item.Count--;
        }
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task RemoveAsync(int productId)
    {
        var cart = await GetCartAsync();
        cart.RemoveAll(c => c.ProductId == productId);
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task ClearCartAsync()
    {
        await _localStorage.RemoveItemAsync(SD.CartKey);
        OnChange?.Invoke();
    }

    public async Task<int> GetCartCountAsync()
    {
        var cart = await GetCartAsync();
        return cart.Sum(c => c.Count);
    }

    public async Task<double> GetCartTotalAsync()
    {
        var cart = await GetCartAsync();
        return cart.Sum(c => c.Price * c.Count);
    }
}
