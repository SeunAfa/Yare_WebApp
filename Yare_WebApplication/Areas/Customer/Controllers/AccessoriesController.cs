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
public class AccessoriesController : Controller
{
    private readonly ILogger<AccessoriesController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    [BindProperty]
    public HomePgVM HomePgVM { get; set; }

    public AccessoriesController(ILogger<AccessoriesController> logger, IUnitOfWork unitOfWork)
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

    [Route("Customer/Accessories/Index/WatchStrapsAccessories")]
    public IActionResult WatchStrapsAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the WatchStrapsAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.WatchStraps
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

    [Route("Customer/Accessories/Index/CasesAndBoxesAccessories")]
    public IActionResult CasesAndBoxesAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the CasesAndBoxesAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.CasesAndBoxes
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

    [Route("Customer/Accessories/Index/WalletsAccessories")]
    public IActionResult WalletsAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the WalletsAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.CasesAndBoxes
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

    [Route("Customer/Accessories/Index/KeyringsAccessories")]
    public IActionResult KeyringsAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the KeyringsAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.Keyrings
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

    [Route("Customer/Accessories/Index/LightersAccessories")]
    public IActionResult LightersAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the LightersAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.Lighters
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

    [Route("Customer/Accessories/Index/PensAccessories")]
    public IActionResult PensAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the PensAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.Pens
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

    [Route("Customer/Accessories/Index/CleaningProductsAccessories")]
    public IActionResult CleaningProductsAccessories()
    {

        var products = _unitOfWork.product.GetAll().ToList();

        // Query to get the list of products belonging to the CleaningProductsAccessories collection and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                                    && product is Accessory accessories && accessories.AccessoryCategory == AccessoryCategory.CleaningProducts
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

        // Query to get the list of products belonging to the specified Accessory and category
        var objProductList = (from product in products
                              where product.ProductCategory == ProductCategory.Accessory
                              select product).ToList();

        // Create the ViewModel for products
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

