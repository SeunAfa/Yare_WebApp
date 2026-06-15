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

    public async Task AddToCartAsync(Product product, int count = 1, string? engraving = null)
    {
        var cart = await GetCartAsync();
        var normalised = string.IsNullOrWhiteSpace(engraving) ? null : engraving.Trim();

        // Engraved lines are unique per (product, engraving text) — different SKUs effectively.
        var existing = cart.FirstOrDefault(c =>
            c.ProductId == product.Id &&
            string.Equals(c.Engraving, normalised, StringComparison.Ordinal));

        if (existing != null)
        {
            existing.Count += count;
        }
        else
        {
            cart.Add(new CartItem
            {
                Id = cart.Any() ? cart.Max(c => c.Id) + 1 : 1,
                ProductId = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Count = count,
                ImageUrl = product.PrimaryDisplayImageUrl,
                ProductCategory = product.ProductCategory?.ToString(),
                Engraving = normalised,
                EngravingFee = normalised is null ? 0 : SD.EngravingFee
            });
        }
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task IncrementAsync(int cartItemId)
    {
        var cart = await GetCartAsync();
        var item = cart.FirstOrDefault(c => c.Id == cartItemId);
        if (item != null) item.Count++;
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task DecrementAsync(int cartItemId)
    {
        var cart = await GetCartAsync();
        var item = cart.FirstOrDefault(c => c.Id == cartItemId);
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

    public async Task RemoveAsync(int cartItemId)
    {
        var cart = await GetCartAsync();
        cart.RemoveAll(c => c.Id == cartItemId);
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task UpdateEngravingAsync(int cartItemId, string? engraving, int? applyTo = null)
    {
        var cart = await GetCartAsync();
        var item = cart.FirstOrDefault(c => c.Id == cartItemId);
        if (item == null) return;

        var normalised = string.IsNullOrWhiteSpace(engraving) ? null : engraving.Trim();
        var count = Math.Clamp(applyTo ?? item.Count, 1, item.Count);

        if (count < item.Count)
        {
            // Engrave only part of the line: split it so the remaining items
            // keep their current state and can carry a different message.
            item.Count -= count;
            var split = new CartItem
            {
                Id = cart.Max(c => c.Id) + 1,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Count = count,
                ImageUrl = item.ImageUrl,
                ProductCategory = item.ProductCategory,
                Engraving = normalised,
                EngravingFee = normalised is null ? 0 : SD.EngravingFee
            };
            cart.Add(split);
            item = split;
        }
        else
        {
            item.Engraving = normalised;
            item.EngravingFee = normalised is null ? 0 : SD.EngravingFee;
        }

        // If editing produces a duplicate (same product + same engraving), merge them.
        var duplicate = cart.FirstOrDefault(c =>
            c.Id != item.Id &&
            c.ProductId == item.ProductId &&
            string.Equals(c.Engraving, item.Engraving, StringComparison.Ordinal));
        if (duplicate != null)
        {
            duplicate.Count += item.Count;
            cart.Remove(item);
        }

        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task AddEngravingMessageAsync(int productId, string engraving)
    {
        var normalised = engraving?.Trim();
        if (string.IsNullOrWhiteSpace(normalised)) return;

        var cart = await GetCartAsync();
        var group = cart.Where(c => c.ProductId == productId).ToList();
        if (!group.Any()) return;

        // Reuse an existing identical message if present
        var same = group.FirstOrDefault(c => string.Equals(c.Engraving, normalised, StringComparison.Ordinal));
        var plain = group.FirstOrDefault(c => !c.HasEngraving);

        if (same != null)
        {
            same.Count++;
            if (plain != null && plain.Count > 0) { if (--plain.Count == 0) cart.Remove(plain); }
        }
        else if (plain != null && plain.Count == 1)
        {
            plain.Engraving = normalised;
            plain.EngravingFee = SD.EngravingFee;
        }
        else
        {
            // Take one unit from the plain line when available,
            // otherwise add a brand-new unit for the extra message
            if (plain != null) plain.Count--;
            var template = group.First();
            cart.Add(new CartItem
            {
                Id = cart.Max(c => c.Id) + 1,
                ProductId = template.ProductId,
                ProductName = template.ProductName,
                Price = template.Price,
                Count = 1,
                ImageUrl = template.ImageUrl,
                ProductCategory = template.ProductCategory,
                Engraving = normalised,
                EngravingFee = SD.EngravingFee
            });
        }

        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task AddUnitAsync(int productId)
    {
        var cart = await GetCartAsync();
        var plain = cart.FirstOrDefault(c => c.ProductId == productId && !c.HasEngraving);
        if (plain != null)
        {
            plain.Count++;
        }
        else
        {
            var template = cart.FirstOrDefault(c => c.ProductId == productId);
            if (template == null) return;
            cart.Add(new CartItem
            {
                Id = cart.Max(c => c.Id) + 1,
                ProductId = template.ProductId,
                ProductName = template.ProductName,
                Price = template.Price,
                Count = 1,
                ImageUrl = template.ImageUrl,
                ProductCategory = template.ProductCategory
            });
        }
        await _localStorage.SetItemAsync(SD.CartKey, cart);
        OnChange?.Invoke();
    }

    public async Task RemoveUnitAsync(int productId)
    {
        var cart = await GetCartAsync();
        var plain = cart.FirstOrDefault(c => c.ProductId == productId && !c.HasEngraving);
        if (plain == null) return; // only un-engraved units leave via the counter
        if (plain.Count <= 1) cart.Remove(plain);
        else plain.Count--;
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
        return cart.Sum(c => c.LineTotal); // includes per-unit engraving fee
    }
}
