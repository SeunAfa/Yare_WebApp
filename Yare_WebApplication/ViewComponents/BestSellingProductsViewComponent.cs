using Microsoft.AspNetCore.Mvc;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models.ViewModels;
using System.Linq;
using Yare.Models;

public class BestSellingProductsViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public BestSellingProductsViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IViewComponentResult Invoke()
    {
        // Define the collection name we're interested in
        string collectionName = "Best Sellers";

        // Retrieve data from repositories
        var products = _unitOfWork.product.GetAll(); // Assuming GetAll() returns IEnumerable<Product>
        var productCollections = _unitOfWork.Product_Collection.GetAll(); // Assuming GetAll() returns IEnumerable<Product_Collection>
        var collections = _unitOfWork.Collection.GetAll(); // Assuming GetAll() returns IEnumerable<Collection>

        // Find the "Best Sellers" collection ID
        var bestSellersCollection = collections.FirstOrDefault(c => c.CollectionName == collectionName);
        if (bestSellersCollection == null)
        {
            // Handle the case where the "Best Sellers" collection does not exist
            return Content("Best Sellers collection not found.");
        }
        int bestSellersCollectionId = bestSellersCollection.Id;

        // Query to get the list of products belonging to the specified collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Where(x => x.CollectionId == bestSellersCollectionId)
            .Select(x => x.Product)
            .ToList();

        // Create the ViewModel for the home page
        var homePgVM = new HomePgVM
        {
            SearchBestSellingProducts = objProductList
        };

        // Return the view with the home page ViewModel
        return View("Default", homePgVM);
    }
}
