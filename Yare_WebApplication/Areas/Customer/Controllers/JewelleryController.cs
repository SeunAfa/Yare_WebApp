using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
public class JewelleryController : Controller
{
    private readonly ILogger<JewelleryController> _logger;
    private readonly IUnitOfWork _unitOfWork;
   
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    [BindProperty]
    public HomePgVM HomePgVM { get; set; }

    public JewelleryController(ILogger<JewelleryController> logger, IUnitOfWork unitOfWork)
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

    [Route("Customer/Jewellery/Index/MensJewellery")]
    public IActionResult MensJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        var objProductList = (from product in products
                              where product.Gender == Gender.Male
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

    [Route("Customer/Jewellery/Index/LadiesJewellery")]
    public IActionResult LadiesJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        var objProductList = (from product in products
                              where product.Gender == Gender.Female
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

    [Route("Customer/Jewellery/Index/ExclusiveJewellery")]
    public IActionResult ExclusiveJewellery()
    {
        string collectionName = "Exclusive Jewellery";

        var products = _unitOfWork.product.GetAll().ToList();
        var productCollections = _unitOfWork.Product_Collection.GetAll().ToList();
        var collections = _unitOfWork.Collection.GetAll().ToList();

        // Query to get the list of products belonging to the specified ExclusiveJewellery collection and category
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

    [Route("Customer/Jewellery/Index/AnkletsJewellery")]
    public IActionResult AnkletsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the specified AnkletsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Anklets
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

    [Route("Customer/Jewellery/Index/BanglesJewellery")]
    public IActionResult BanglesJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the BanglesJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Bangles
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

    [Route("Customer/Jewellery/Index/BraceletsJewellery")]
    public IActionResult BraceletsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the BraceletsJewellery collection and category
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

    [Route("Customer/Jewellery/Index/BroochesJewellery")]
    public IActionResult BroochesJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the BroochesJewellery collection and category
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

    [Route("Customer/Jewellery/Index/CufflinksJewellery")]
    public IActionResult CufflinksJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the CufflinksJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Cufflinks
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

    [Route("Customer/Jewellery/Index/ChainsJewellery")]
    public IActionResult ChainsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the ChainsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Chains
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

    [Route("Customer/Jewellery/Index/CharmsJewellery")]
    public IActionResult CharmsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the CharmsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Charms
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

    [Route("Customer/Jewellery/Index/EarringsJewellery")]
    public IActionResult EarringsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the EarringsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Earrings
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

    [Route("Customer/Jewellery/Index/EngagementRingsJewellery")]
    public IActionResult EngagementRingsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the EngagementRingsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.EngagementRings
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

    [Route("Customer/Jewellery/Index/LocketsJewellery")]
    public IActionResult LocketsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the LocketsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Lockets
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

    [Route("Customer/Jewellery/Index/NecklacesJewellery")]
    public IActionResult NecklacesJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the NecklacesJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Necklaces
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

    [Route("Customer/Jewellery/Index/PendantsJewellery")]
    public IActionResult PendantsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the PendantsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.Pendants
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

    [Route("Customer/Jewellery/Index/RingsJewellery")]
    public IActionResult RingsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the RingsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
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

    [Route("Customer/Jewellery/Index/WeddingRingsJewellery")]
    public IActionResult WeddingRingsJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the WeddingRingsJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.JewelleryCategory == JewelleryCategory.WeddingRings
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

    [Route("Customer/Jewellery/Index/ByOccasionJewellery")]
    public IActionResult ByOccasionJewellery()
    {
        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products with ByOccasion and jewellery
        var objProductList = products
            .Where(p => (p is Yare.Models.Jewellery jewellery && jewellery.ByOccassion != null))
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

    [Route("Customer/Jewellery/Index/GoldJewellery")]
    public IActionResult GoldJewellery()
    {
        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the GoldJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByMetal == ByMetal.Gold
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

    [Route("Customer/Jewellery/Index/WhiteGoldJewellery")]
    public IActionResult WhiteGoldJewellery()
    {
        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the WhiteGoldJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByMetal == ByMetal.WhiteGold
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

    [Route("Customer/Jewellery/Index/RoseGoldJewellery")]
    public IActionResult RoseGoldJewellery()
    {
        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the RoseGoldJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByMetal == ByMetal.RoseGold
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

    [Route("Customer/Jewellery/Index/YellowGoldJewellery")]
    public IActionResult YellowGoldJewellery()
    {
        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the YellowGoldJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByMetal == ByMetal.YellowGold
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

    [Route("Customer/Jewellery/Index/SilverJewellery")]
    public IActionResult SilverJewellery()
    {
        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the SilverJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByMetal == ByMetal.Silver
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

    [Route("Customer/Jewellery/Index/PlatinumJewellery")]
    public IActionResult PlatinumJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the PlatinumJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByMetal == ByMetal.Platinum
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

    [Route("Customer/Jewellery/Index/DiamondJewellery")]
    public IActionResult DiamondJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the DiamondJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByGemstone == ByGemstone.Diamond
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

    [Route("Customer/Jewellery/Index/LabGrownDiamondJewellery")]
    public IActionResult LabGrownDiamondJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the LabGrownDiamondJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByGemstone == ByGemstone.LabGrownDiamonds
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

    [Route("Customer/Jewellery/Index/PearlJewellery")]
    public IActionResult PearlJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the PearlJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByGemstone == ByGemstone.Pearl
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

    [Route("Customer/Jewellery/Index/RubyJewellery")]
    public IActionResult RubyJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the RubyJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByGemstone == ByGemstone.Ruby
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

    [Route("Customer/Jewellery/Index/EmeraldJewellery")]
    public IActionResult EmeraldJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the EmeraldJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByGemstone == ByGemstone.Emerald
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

    [Route("Customer/Jewellery/Index/SapphireJewellery")]
    public IActionResult SapphireJewellery()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the SapphireJewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
                                    && product is Jewellery jewellery && jewellery.ByGemstone == ByGemstone.Sapphire
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

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the Jewellery collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Jewellery
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


        return View(homePgVM);
    }
}
