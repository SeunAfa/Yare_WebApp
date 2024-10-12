using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using Stripe;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.Enums;
using Yare.Models.ViewModels;
using Yare_WebApplication.Data.Utility;
using Product = Yare.Models.Product;


namespace Yare_WebApplication.Areas.Customer.Controllers;
[Area("Customer")]
public class WatchesController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

     [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    [BindProperty]
    public HomePgVM HomePgVM { get; set; }

    public WatchesController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    //GET: SearchResults
    public IActionResult Filter(string searchString)
    {
        if (!Request.Headers.ContainsKey("X-Requested-With") || Request.Headers["X-Requested-With"] != "XMLHttpRequest")
        {
            return RedirectToAction("Index");
        }

        try
        {
            // Retrieve product list from the repository
            IEnumerable<Yare.Models.Product> objProductList = _unitOfWork.product.GetAll();

            if (objProductList == null)
            {
                _logger.LogError("Product list retrieval failed, objProductList is null.");
                objProductList = new List<Yare.Models.Product>();
            }
            else
            {
                _logger.LogInformation($"Product list retrieved with {objProductList.Count()} items.");
            }

            var filteredProductList = objProductList;

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredProductList = objProductList
                    .Where(w => ProductMatchesSearchString(w, searchString))
                    .OrderBy(w => w.ProductName.Length)
                    .ToList();

                if (!filteredProductList.Any())
                {
                    ViewBag.NoResults = "No results found";
                }
            }

            // Create and populate ProductVM
            var productVM = new ProductVM
            {
                objProductList = filteredProductList
            };

            // Create and populate HomePgVM
            var homePgVM = new HomePgVM
            {
                ProductVM = productVM
            };

            _logger.LogInformation("HomePgVM created successfully with ProductVM.");

            return PartialView("_searchBar_Results", homePgVM); // Pass HomePgVM to the view
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while preparing the HomePgVM.");
            return View("Error");
        }
    }

    private bool ProductMatchesSearchString(Product product, string searchString)
    {
        var excludedProperties = new List<string> {
        "CostOfProduct", "TargetPrice01", "TargetPrice02", "TargetPrice03", "Price", "PriceWas",
        "Quantity", "RemainigQuantity", "StockStatus", "WarrantyYears", "ProductDescription",
        "PrimaryDisplayImageUrl", "SecondaryDisplayImageUrl", "SliderImageUrlOne", "SliderImageUrlTwo",
        "SliderImageUrlThree", "SliderImageUrlOne", "SliderImageUrlFour", "LastUpdate",
        "SliderImageUrlThree", "SliderImageUrlOne"
    };

        if (product.ProductCategory.ToString().ToLower() != product.GetType().Name.ToLower())
            return false;

        var searchTerms = searchString.ToLower().Split(' ');

        var properties = product.GetType().GetProperties();

        foreach (var term in searchTerms)
        {
            bool termMatched = false;
            foreach (var property in properties)
            {
                if (excludedProperties.Contains(property.Name))
                    continue;

                var value = property.GetValue(product);
                if (value != null && value.ToString().ToLower().Contains(term))
                {
                    termMatched = true;
                    break;
                }
            }
            if (!termMatched)
                return false;
        }

        return true;
    }

    [Route("Customer/Watches/Index/NewInWatches")]
    public IActionResult NewInWatches()
    {

        DateTime newInProducts = DateTime.Now.AddDays(-30);

        // Get the list of watches created in the last 7 days
        IEnumerable<Yare.Models.Product> objProductList = _unitOfWork.product.GetAll(w => w.ProductCategory == ProductCategory.Watch && w.CreatedDateTime >= newInProducts);

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/ExclusiveWatches")]
    public IActionResult ExclusiveWatches()
    {
        string collectionName = "Exclusive Watches";

        var products = _unitOfWork.product.GetAll();
        var productCollections = _unitOfWork.Product_Collection.GetAll();
        var collections = _unitOfWork.Collection.GetAll(); 

        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/LimitedEditionWatches")]
    public IActionResult LimitedEditionWatches()
    {
        string collectionName = "Limited Edition";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll();
        var collections = _unitOfWork.Collection.GetAll();

        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/MensWatches")]
    public IActionResult MensWatches()
    {
        IEnumerable<Yare.Models.Product> objProductList = _unitOfWork.product.GetAll(w => w.Gender == Gender.Male && w.ProductCategory == ProductCategory.Watch);


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/LadiesWatches")]
    public IActionResult LadiesWatches()
    {
        IEnumerable<Yare.Models.Product> objProductList = _unitOfWork.product.GetAll(w => w.Gender == Gender.Female && w.ProductCategory == ProductCategory.Watch);


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/UnisexWatches")]
    public IActionResult UnisexWatches()
    {
        IEnumerable<Yare.Models.Product> objProductList = _unitOfWork.product.GetAll(w => w.Gender == Gender.Unisex && w.ProductCategory == ProductCategory.Watch);


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/BestSellersWatches")]
    public IActionResult BestSellersWatches()
    {

        string collectionName = "Best Sellers";
        int topCount = 80;

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll(); 
        var orderDetails = _unitOfWork.OrderDetail.GetAll(); 

        var bestSellersCollection = collections.FirstOrDefault(c => c.CollectionName == collectionName);
        if (bestSellersCollection == null)
        {
            return NotFound("Best Sellers collection not found.");
        }
        int bestSellersCollectionId = bestSellersCollection.Id;

        // Get the best-selling products by count and quantity
        var bestSellingProducts = orderDetails
            .GroupBy(od => od.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalCount = g.Sum(od => od.Count),
                TotalQuantity = g.Sum(od => od.Count * od.Price)
            })
            .OrderByDescending(x => x.TotalCount) 
            .Take(topCount)
            .ToList();

        // Get product details for the best-selling products
        var bestSellersList = bestSellingProducts
            .Join(products,
                  bs => bs.ProductId,
                  p => p.Id,
                  (bs, p) => new { bs.ProductId, Product = p, bs.TotalCount, bs.TotalQuantity })
            .ToList();

        // Find products that are not already in the "Best Sellers" collection
        var bestSellersNotInCollection = bestSellersList
            .Where(p => !productCollections.Any(pc => pc.ProductId == p.ProductId && pc.CollectionId == bestSellersCollectionId))
            .ToList();

        // Add best-selling products to the "Best Sellers" collection if not already present
        foreach (var bestSeller in bestSellersNotInCollection)
        {
            var productCollection = new Product_Collection
            {
                ProductId = bestSeller.ProductId,
                CollectionId = bestSellersCollectionId
            };
            _unitOfWork.Product_Collection.Add(productCollection);
        }
        _unitOfWork.Save();

        // Query to get the list of products belonging to the best-selling products collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM,
            BestSellersList = bestSellersList.Select(bs => new OrderDetail
            {
                ProductId = bs.ProductId,
                Product = bs.Product,
                Count = bs.TotalCount,
                Price = bs.TotalQuantity
            }).ToList()
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/LuxuryWatches")]
    public IActionResult LuxuryWatches()
    {
        string collectionName = "Luxury Watches";

        var products = _unitOfWork.product.GetAll();
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Luxury Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/DiamondWatches")]
    public IActionResult DiamondWatches()
    {
        string collectionName = "Diamond Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll();
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Diamond Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);

    }

    [Route("Customer/Watches/Index/SwissWatches")]
    public IActionResult SwissWatches()
    {
        string collectionName = "Swiss Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Swiss Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/LeatherWatches")]
    public IActionResult LeatherWatches()
    {
        string collectionName = "Leather Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Leather Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName &&
                        x.Product is Yare.Models.Watch watch && watch.WatchStrapType == WatchStrapType.Leather)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/HeritageWatches")]
    public IActionResult HeritageWatches()
    {
        string collectionName = "Heritage Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Heritage Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }
  
    [Route("Customer/Watches/Index/PocketWatches")]
    public IActionResult PocketWatches()
    {
        string collectionName = "Pocket Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Pocket Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/SportWatches")]
    public IActionResult SportWatches()
    {
        string collectionName = "Sport Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the Pocket Watches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/DiveWatches")]
    public IActionResult DiveWatches()
    {
        string collectionName = "Dive Watches";

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll();

        // Query to get the list of products belonging to the DiveWatches collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Product.ProductCategory == ProductCategory.Watch &&
                        x.Collection.CollectionName == collectionName)
            .Select(x => x.Product)
            .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList,
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count 
        ViewBag.TotalProductCount = objProductList.Count;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/MechanicalWatches")]
    public IActionResult MechanicalWatches()
    {

        var products = _unitOfWork.product.GetAll();
 
        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchMovement == WatchMovement.Mechanical)
     .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/AutomaticWatches")]
    public IActionResult AutomaticWatches()
    {

        var products = _unitOfWork.product.GetAll();
 
        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchMovement == WatchMovement.Automatic)
     .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/QuartzWatches")]
    public IActionResult QuartzWatches()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchMovement == WatchMovement.Quartz)
     .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/KineticWatches")]
    public IActionResult KineticWatches()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchMovement == WatchMovement.Kinetic)
     .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    [Route("Customer/Watches/Index/EcoDriveWatches")]
    public IActionResult EcoDriveWatches()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchMovement == WatchMovement.EcoDrive)
     .ToList();

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View("Index", homePgVM);
    }

    public IActionResult Index()
    {
        IEnumerable<Yare.Models.Product> objProductList = _unitOfWork.product.GetAll(w => w.ProductCategory == ProductCategory.Watch);

        var productVM = new ProductVM
        {
            objProductList = objProductList
        };

        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        int totalProductCount = objProductList.Count();

        ViewBag.TotalProductCount = totalProductCount;

        return View(homePgVM);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}