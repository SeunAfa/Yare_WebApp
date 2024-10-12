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
public class CollectionsController : Controller
{
    private readonly ILogger<CollectionsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    [BindProperty]
    public HomePgVM HomePgVM { get; set; }

    public CollectionsController(ILogger<CollectionsController> logger, IUnitOfWork unitOfWork)
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

            var productVM = new ProductVM
            {
                objProductList = filteredProductList
            };

            var homePgVM = new HomePgVM
            {
                ProductVM = productVM
            };

            _logger.LogInformation("HomePgVM created successfully with ProductVM.");

            return PartialView("_searchBar_Results", homePgVM); 
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

    [Route("Customer/Collections/Index/OurTopTenCollection")]
    public IActionResult OurTopTenCollection()
    {
        string collectionName = "Our Top 10";


        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the OurTopTenCollection collection
        var objProductList = (from product in products
                              join productCollection in productCollections on product.Id equals productCollection.ProductId
                              join collection in collections on productCollection.CollectionId equals collection.Id
                              where collection.CollectionName == collectionName
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/ExclusiveBrandsCollection")]
    public IActionResult ExclusiveBrandsCollection()
    {
        string collectionName = "Exclusive Brands";


        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the ExclusiveBrandsCollection collection
        var objProductList = (from product in products
                              join productCollection in productCollections on product.Id equals productCollection.ProductId
                              join collection in collections on productCollection.CollectionId equals collection.Id
                              where collection.CollectionName == collectionName
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/ExclusiveWatchesCollection")]
    public IActionResult ExclusiveWatchesCollection()
    {
        string collectionName = "Exclusive Watches";


        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the ExclusiveWatchesCollection collection and category
        var objProductList = (from product in products
                              join productCollection in productCollections on product.Id equals productCollection.ProductId
                              join collection in collections on productCollection.CollectionId equals collection.Id
                              where collection.CollectionName == collectionName
                                    && product.ProductCategory == ProductCategory.Watch
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/ExclusiveJewelleryCollection")]
    public IActionResult ExclusiveJewelleryCollection()
    {
        string collectionName = "Exclusive Jewellery";


        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the ExclusiveJewelleryCollection collection and category
        var objProductList = (from product in products
                              join productCollection in productCollections on product.Id equals productCollection.ProductId
                              join collection in collections on productCollection.CollectionId equals collection.Id
                              where collection.CollectionName == collectionName
                                    && product.ProductCategory == ProductCategory.Jewellery
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/LimitedEditionCollection")]
    public IActionResult LimitedEditionCollection()
    {
        string collectionName = "Limited Edition";


        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the LimitedEditionCollection collection
        var objProductList = (from product in products
                              join productCollection in productCollections on product.Id equals productCollection.ProductId
                              join collection in collections on productCollection.CollectionId equals collection.Id
                              where collection.CollectionName == collectionName
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/ByOccasionCollection")]
    public IActionResult ByOccasionCollection()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products with ByOccasion property for watches and jewellery
        var objProductList = products
            .Where(p => (p is Yare.Models.Watch watch && watch.ByOccassion != null) ||
                        (p is Yare.Models.Jewellery jewellery && jewellery.ByOccassion != null))
            .ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/ALangeAndSohneCollection")]
    public IActionResult ALangeAndSohneCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.ALangeAndSöhne)
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

    [Route("Customer/Collections/Index/AudemarsPiguetCollection")]
    public IActionResult AudemarsPiguetCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.AudemarsPiguet)
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

    [Route("Customer/Collections/Index/BreguetCollection")]
    public IActionResult BreguetCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Breguet)
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

    [Route("Customer/Collections/Index/BlancpainCollection")]
    public IActionResult BlancpainCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Blancpain)
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

    [Route("Customer/Collections/Index/BvlgariCollection")]
    public IActionResult BvlgariCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Bvlgari)
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

    [Route("Customer/Collections/Index/BreitlingCollection")]
    public IActionResult BreitlingCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Breitling)
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

    [Route("Customer/Collections/Index/CartierCollection")]
    public IActionResult CartierCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Cartier)
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

    [Route("Customer/Collections/Index/OmegaCollection")]
    public IActionResult OmegaCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.OMEGA)
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

    [Route("Customer/Collections/Index/TAGHeuerCollection")]
    public IActionResult TAGHeuerCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.TAGHEUER)
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

    [Route("Customer/Collections/Index/HublotCollection")]
    public IActionResult HublotCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Hublot)
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

    [Route("Customer/Collections/Index/TudorCollection")]
    public IActionResult TudorCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Tudor)
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

    [Route("Customer/Collections/Index/BellAndRossCollection")]
    public IActionResult BellAndRossCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.BellAndRoss)
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

    [Route("Customer/Collections/Index/BaumeAndMercierCollection")]
    public IActionResult BaumeAndMercierCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.BaumeAndMercier)
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

    [Route("Customer/Collections/Index/ChopardCollection")]
    public IActionResult ChopardCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.Chopard)
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

    [Route("Customer/Collections/Index/GirardPerregauxCollection")]
    public IActionResult GirardPerregauxCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.GirardPerregaux)
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

    [Route("Customer/Collections/Index/JaegerLeCoultreCollection")]
    public IActionResult JaegerLeCoultreCollection()
    {

        var products = _unitOfWork.product.GetAll();

        var objProductList = products
     .Where(p => p.ProductCategory == ProductCategory.Watch && p is Yare.Models.Watch watch && watch.WatchBrand == WatchBrand.JaegerLeCoultre)
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

    [Route("Customer/Collections/Index/CartierRingsCollection")]
    public IActionResult CartierRingsCollection()
    {
        string collectionName = "Cartier Rings";


        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the CartierRingsCollection collection and category
        var objProductList = (from product in products
                              join productCollection in productCollections on product.Id equals productCollection.ProductId
                              join collection in collections on productCollection.CollectionId equals collection.Id
                              where collection.CollectionName == collectionName
                                    && product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Rings
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/EngagementRingsCollection")]
    public IActionResult EngagementRingsCollection()
    {


        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the specified collection and category
        var objProductList = (from product in products     
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Rings
                                     && product is Jewellery jewellery01 && jewellery01.ByOccassion == ByOccassion.EngagementRings
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/BestSellersCollection")]
    public IActionResult BestSellersCollection()
    {
        string collectionName = "Best Sellers";
        int topCount = 80;

        var products = _unitOfWork.product.GetAll(); 
        var productCollections = _unitOfWork.Product_Collection.GetAll(); 
        var collections = _unitOfWork.Collection.GetAll(); 
        var orderDetails = _unitOfWork.OrderDetail.GetAll(); 

        // Find the "Best Sellers" collection ID
        var bestSellersCollection = collections.FirstOrDefault(c => c.CollectionName == collectionName);
        if (bestSellersCollection == null)
        {
            // Handle the case where the "Best Sellers" collection does not exist
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

        // Query to get the list of products belonging to the BestSellersCollection collection
        var objProductList = products
            .Join(productCollections,
                  p => p.Id,
                  pc => pc.ProductId,
                  (p, pc) => new { Product = p, pc.CollectionId })
            .Join(collections,
                  pc => pc.CollectionId,
                  c => c.Id,
                  (pc, c) => new { Product = pc.Product, Collection = c })
            .Where(x => x.Collection.CollectionName == collectionName)
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

    [Route("Customer/Collections/Index/BraceletsCollection")]
    public IActionResult BraceletsCollection()
    {


        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the BraceletsCollection collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Bracelets
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    [Route("Customer/Collections/Index/BroochesCollection")]
    public IActionResult BroochesCollection()
    {


        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the BroochesCollection collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Brooches
                              select product).ToList();


        var productVM = new ProductVM
        {
            objProductList = objProductList
        };


        var homePgVM = new HomePgVM
        {
            ProductVM = productVM
        };

        // Set the total product count
        ViewBag.TotalProductCount = objProductList.Count;


        return View("Index", homePgVM);
    }

    public IActionResult Index()
    {
        try
        {
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

            var productVM = new ProductVM
            {
                objProductList = objProductList
            };

            var homePgVM = new HomePgVM
            {
                ProductVM = productVM
            };

            _logger.LogInformation("HomePgVM created successfully with ProductVM.");

            return View(homePgVM);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while preparing the HomePgVM.");
            return View("Error");
        }
    }
}
