using Yare.BlazorApp.Models;
using Yare.BlazorApp.Models.Enums;

namespace Yare.BlazorApp.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByCategoryAsync(ProductCategory category);
    Task<List<Product>> SearchAsync(string searchTerm);
    Task<List<Product>> GetByCollectionAsync(int collectionId);
    Task<List<Collection>> GetCollectionsAsync();
    Task<Collection?> GetCollectionByIdAsync(int id);
    Task<List<Product>> GetBestSellersAsync();
    Task<List<Product>> GetNewArrivalsAsync();
    Task<bool> SaveProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> SaveCollectionAsync(Collection collection);
    Task<bool> DeleteCollectionAsync(int id);
}
