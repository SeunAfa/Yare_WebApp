using System.Net.Http.Json;
using System.Text.Json;
using Yare.BlazorApp.Models;
using Yare.BlazorApp.Models.Enums;

namespace Yare.BlazorApp.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _http;
    private List<Product>? _products;
    private List<Collection>? _collections;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    private async Task<List<Product>> LoadProductsAsync()
    {
        if (_products != null) return _products;
        try
        {
            _products = await _http.GetFromJsonAsync<List<Product>>("data/products.json?v=9", _jsonOptions) ?? new();
        }
        catch
        {
            _products = new List<Product>();
        }
        return _products;
    }

    private async Task<List<Collection>> LoadCollectionsAsync()
    {
        if (_collections != null) return _collections;
        try
        {
            _collections = await _http.GetFromJsonAsync<List<Collection>>("data/collections.json?v=6", _jsonOptions) ?? new();
        }
        catch
        {
            _collections = new List<Collection>();
        }
        return _collections;
    }

    public async Task<List<Product>> GetAllAsync()
        => await LoadProductsAsync();

    public async Task<Product?> GetByIdAsync(int id)
    {
        var products = await LoadProductsAsync();
        return products.FirstOrDefault(p => p.Id == id);
    }

    public async Task<List<Product>> GetByCategoryAsync(ProductCategory category)
    {
        var products = await LoadProductsAsync();
        return products.Where(p => p.ProductCategory == category).ToList();
    }

    public async Task<List<Product>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return await LoadProductsAsync();
        var lower = searchTerm.ToLower();
        var products = await LoadProductsAsync();
        return products.Where(p =>
            (p.ProductName?.ToLower().Contains(lower) ?? false) ||
            (p.ProductNumber?.ToLower().Contains(lower) ?? false) ||
            (p.ProductDescription?.ToLower().Contains(lower) ?? false) ||
            (p.Supplier?.ToLower().Contains(lower) ?? false) ||
            (p.ProductCategory?.ToString().ToLower().Contains(lower) ?? false)
        ).ToList();
    }

    public async Task<List<Product>> GetByCollectionAsync(int collectionId)
    {
        var collections = await LoadCollectionsAsync();
        var collection = collections.FirstOrDefault(c => c.Id == collectionId);
        if (collection == null) return new();
        var products = await LoadProductsAsync();
        return products.Where(p => collection.ProductIds.Contains(p.Id)).ToList();
    }

    public async Task<List<Collection>> GetCollectionsAsync()
        => await LoadCollectionsAsync();

    public async Task<Collection?> GetCollectionByIdAsync(int id)
    {
        var collections = await LoadCollectionsAsync();
        return collections.FirstOrDefault(c => c.Id == id);
    }

    public async Task<List<Product>> GetBestSellersAsync()
    {
        var collections = await LoadCollectionsAsync();
        var bestSellers = collections.FirstOrDefault(c => c.CollectionName == "Best Sellers");
        if (bestSellers == null) return (await LoadProductsAsync()).Take(6).ToList();
        var products = await LoadProductsAsync();
        return products.Where(p => bestSellers.ProductIds.Contains(p.Id)).Take(8).ToList();
    }

    public async Task<List<Product>> GetNewArrivalsAsync()
    {
        var products = await LoadProductsAsync();
        return products.OrderByDescending(p => p.CreatedDateTime).Take(8).ToList();
    }

    public async Task<bool> SaveProductAsync(Product product)
    {
        // In-memory only — GitHub Pages has no server to persist to
        if (_products == null) _products = new();
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing != null)
            _products[_products.IndexOf(existing)] = product;
        else
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
        }

        var collections = await LoadCollectionsAsync();
        foreach (var collection in collections)
        {
            if (product.CollectionIds.Contains(collection.Id))
            {
                if (!collection.ProductIds.Contains(product.Id))
                    collection.ProductIds.Add(product.Id);
            }
            else
            {
                collection.ProductIds.Remove(product.Id);
            }
        }

        return true;
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        _products?.RemoveAll(p => p.Id == id);
        return Task.FromResult(true);
    }

    public async Task<bool> SaveCollectionAsync(Collection collection)
    {
        // In-memory only — GitHub Pages has no server to persist to
        var collections = await LoadCollectionsAsync();
        var existing = collections.FirstOrDefault(c => c.Id == collection.Id);
        if (existing != null)
            collections[collections.IndexOf(existing)] = collection;
        else
        {
            collection.Id = collections.Any() ? collections.Max(c => c.Id) + 1 : 1;
            collections.Add(collection);
        }
        return true;
    }

    public async Task<bool> DeleteCollectionAsync(int id)
    {
        var collections = await LoadCollectionsAsync();
        collections.RemoveAll(c => c.Id == id);
        // Drop the collection from any product that still references it
        if (_products != null)
        {
            foreach (var p in _products)
                p.CollectionIds.Remove(id);
        }
        return true;
    }
}
