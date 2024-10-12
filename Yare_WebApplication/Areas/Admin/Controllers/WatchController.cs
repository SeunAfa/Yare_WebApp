using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Yare.DataAccess.Repository.IRepository;
using Yare_WebApplication.Data.Utility;
using Yare.Models;  
using Yare.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yare.Models.ViewModels;
using Product = Yare.Models.Product;
using System.Collections;
using MediaBrowser.Model.Services;
using Microsoft.EntityFrameworkCore;

namespace Yare_WebApplication.Areas.Admin.Controllers;
[Area("Admin")]
public class WatchController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public WatchController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: SearchResults
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Filter(string searchString)
    {
        if (!Request.Headers.ContainsKey("X-Requested-With") || Request.Headers["X-Requested-With"] != "XMLHttpRequest")
        {
            return RedirectToAction("Index");
        }

        IEnumerable<Watch> objWatchList = _unitOfWork.Watch.GetAll();

        if (!string.IsNullOrEmpty(searchString))
        {
            var filterResult = objWatchList
                .Where(w => ProductMatchesSearchString(w, searchString))
                .OrderBy(w => w.ProductName.Length)
                .ToList();

            if (filterResult.Any())
            {
                ViewBag.SearchString = searchString;
                ViewBag.SearchValueResults = filterResult;
                return PartialView("_WatchSearch_Results", filterResult);
            }
            else
            {
                ViewBag.SearchString = searchString;
                ViewBag.NoResults = "No results found";
                return PartialView("_WatchSearch_Results");
            }
        }

        return PartialView("_WatchSearch_Results", objWatchList);
    }

    private bool ProductMatchesSearchString(Watch product, string searchString)
    {
        // Check if the ProductCategory matches the derived class type
        if (product.ProductCategory.ToString().ToLower() != product.GetType().Name.ToLower())
            return false;

        // Split the search string into individual search terms
        var searchTerms = searchString.ToLower().Split(' ');

        // Check if any property of the product or its derived class matches all search terms
        var properties = product.GetType().GetProperties();

        foreach (var term in searchTerms)
        {
            bool termMatched = false;
            foreach (var property in properties)
            {
                var value = property.GetValue(product);
                if (value != null && value.ToString().ToLower().Contains(term))
                {
                    termMatched = true;
                    break;
                }
            }
            // If the current term is not found in any property, return false
            if (!termMatched)
                return false;
        }

        return true; // All search terms were found in at least one property
    }

    // GET: HomePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        IEnumerable<Watch> objWatchList = _unitOfWork.Watch.GetAll();

        // Perform pagination on the server side
        var paginatedList = objWatchList.Skip((page - 1) * pageSize).Take(pageSize);

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling(objWatchList.Count() / (double)pageSize);


        foreach (var watch in paginatedList)
        {
            var remainigQuantity = watch.RemainigQuantity;

            if (remainigQuantity == 0)
            {
                watch.StockStatus = SD.OutOfStockStatus;
            }
            else if (remainigQuantity <= 50)
            {
                watch.StockStatus = SD.RunningLowStockStatus;
            }
            else
            {
                watch.StockStatus = SD.InStockStatus;
            }

            _unitOfWork.Save();

        }

        return View(paginatedList);
    }

    // GET: CreatePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," )]
    public IActionResult Create()
    {

        var productVM = new ProductVM
        {

            // Initialize Collection SelectListItem properties for the view model
            CollectionList = _unitOfWork.Collection.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CollectionName
                }),

            // Initialize ProductCategoryList for the view model pre select the value of Watches 
            ProductCategoryList = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Select(pc => new SelectListItem
                {
                    Value = pc.ToString(),
                    Text = pc.ToString(),
                    Selected = pc == ProductCategory.Watch
                })
        };

        return View(productVM);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + ",")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductVM productVM)
    {
        // Custom Model Errors
        if (productVM != null && productVM.CostOfProduct == productVM.Price)
        {
            ModelState.AddModelError("Watch.CostOfProduct", "The cost of each product can't be the same as the price");
        }
        else if (productVM != null && productVM.Price <= productVM.CostOfProduct)
        {
            ModelState.AddModelError("Watch.Price", "The price can't be less than the cost of each product");
        }

        if (productVM.SelectedCollectionIds == null || !productVM.SelectedCollectionIds.Any())
        {
            ModelState.AddModelError("", "Please select at least one collection.");
            return View(productVM);
        }

        // Check if there are any model errors
        if (ModelState.IsValid)
        {
            if (productVM != null)
            {
                productVM.CreatedDateTime = DateTime.Now;
                productVM.LastUpdate = DateTime.Now;

                var selectedCollections = new List<Product_Collection>();

                if (productVM.SelectedCollectionIds != null && productVM.SelectedCollectionIds.Any())
                {
                    selectedCollections = productVM.SelectedCollectionIds
                        .Select(collectionId => new Product_Collection
                        {
                            CollectionId = collectionId,
                            CollectionName = _unitOfWork.Collection.GetFirstOrDefault(c => c.Id == collectionId)?.CollectionName
                        })
                        .ToList();
                }
                else
                {
                    // If no collection is selected, assign "No collection" directly
                    selectedCollections = new List<Product_Collection>
                    {
                        new Product_Collection
                        {
                            CollectionId = null, // or any other default value for CollectionId
                            CollectionName = "No collection"
                        }
                    };
                }

                var remainigQuantity = productVM.RemainigQuantity;

                if (remainigQuantity == 0)
                {

                    productVM.StockStatus = SD.OutOfStockStatus;

                }
                else if (remainigQuantity <= 50)
                {

                    productVM.StockStatus = SD.RunningLowStockStatus;

                }
                else if (remainigQuantity >= 50)
                {

                    productVM.StockStatus = SD.InStockStatus;

                }

                // Create an instance of Watch and populate properties
                var watch = new Watch
                {
                    Id = productVM.Id,
                    ProductName = productVM.ProductName,
                    ProductNumber = productVM.ProductNumber,
                    ModelNumber = productVM.ModelNumber,
                    Supplier = productVM.Supplier,
                    CostOfProduct = productVM.CostOfProduct,
                    TargetPrice01 = productVM.TargetPrice01,
                    TargetPrice02 = productVM.TargetPrice02,
                    TargetPrice03 = productVM.TargetPrice03,
                    Price = productVM.Price,
                    PriceWas = productVM.PriceWas,
                    Gender = productVM.Gender,
                    Quantity = productVM.Quantity,
                    RemainigQuantity = productVM.RemainigQuantity,
                    StockStatus = productVM.StockStatus,
                    WarrantyYears = productVM.WarrantyYears,
                    ProductDescription = productVM.ProductDescription,
                    PrimaryDisplayImageUrl = productVM.PrimaryDisplayImageUrl,
                    SecondaryDisplayImageUrl = productVM.SecondaryDisplayImageUrl,
                    SliderImageUrlOne = productVM.SliderImageUrlOne,
                    SliderImageUrlTwo = productVM.SliderImageUrlTwo,
                    SliderImageUrlThree = productVM.SliderImageUrlThree,
                    SliderImageUrlFour = productVM.SliderImageUrlFour,
                    ProductCategory = ProductCategory.Watch,
                    Product_CollectionsList = selectedCollections,

                    WatchBrand = productVM.WatchBrand,
                    WatchStrapType = productVM.WatchStrapType,
                    WatchMovement = productVM.WatchMovement,
                    WaterResistant = productVM.WaterResistant,
                    DialColor = productVM.DialColor,
                    WatchDiameter = productVM.WatchDiameter,
                    WatchCaseShape = productVM.WatchCaseShape,
                    StrapColour = productVM.StrapColour,
                    ByOccassion = productVM.ByOccassion,
                    ByMetal = productVM.ByMetal,
                    ByGemstone = productVM.ByGemstone

                };

                // Add the product to the context
                _unitOfWork.Watch.Add(watch);
                _unitOfWork.Save();

                // Save Product_Collection entries
                foreach (var productCollection in selectedCollections)
                {
                    productCollection.ProductId = watch.Id; // Assign the product ID
                    _unitOfWork.Product_Collection.Add(productCollection);
                }

                _unitOfWork.Save();

                TempData["success"] = "Watch product created successfully";

                return RedirectToAction("Index");
            }
        }

        // Repopulate the CollectionList property before returning to the view
        productVM.CollectionList = _unitOfWork.Collection.GetAll()
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CollectionName
            });

        // Repopulate the ProductCategoryList property for the view model
        productVM.ProductCategoryList = Enum.GetValues(typeof(ProductCategory))
            .Cast<ProductCategory>()
            .Select(pc => new SelectListItem
            {
                Value = pc.ToString(),
                Text = pc.ToString(),
                Selected = pc == ProductCategory.Watch
            });

        return View(productVM);
    }

    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Details(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        var collectionsForProduct = _unitOfWork.Product_Collection
            .GetAll(pc => pc.ProductId == id)
            .Select(pc => pc.CollectionId ?? 0) // Handle possible null values
            .ToList()
            .ToArray();

        ProductVM productVM = new ProductVM
        {
            Id = objFromDb.Id,
            ProductName = objFromDb.ProductName,
            ProductNumber = objFromDb.ProductNumber,
            ModelNumber = objFromDb.ModelNumber,
            Supplier = objFromDb.Supplier,
            CostOfProduct = objFromDb.CostOfProduct,
            TargetPrice01 = objFromDb.TargetPrice01,
            TargetPrice02 = objFromDb.TargetPrice02,
            TargetPrice03 = objFromDb.TargetPrice03,
            Price = objFromDb.Price,
            PriceWas = objFromDb.PriceWas,
            Gender = objFromDb.Gender,
            Quantity = objFromDb.Quantity,
            RemainigQuantity = objFromDb.RemainigQuantity,
            StockStatus = objFromDb.StockStatus,
            WarrantyYears = objFromDb.WarrantyYears,
            ProductDescription = objFromDb.ProductDescription,
            PrimaryDisplayImageUrl = objFromDb.PrimaryDisplayImageUrl,
            SecondaryDisplayImageUrl = objFromDb.SecondaryDisplayImageUrl,
            SliderImageUrlOne = objFromDb.SliderImageUrlOne,
            SliderImageUrlTwo = objFromDb.SliderImageUrlTwo,
            SliderImageUrlThree = objFromDb.SliderImageUrlThree,
            SliderImageUrlFour = objFromDb.SliderImageUrlFour,
            CreatedDateTime = objFromDb.CreatedDateTime,
            LastUpdate = objFromDb.LastUpdate,
            ProductCategory = objFromDb.ProductCategory,
            SelectedCollectionIds = collectionsForProduct,
            CollectionList = _unitOfWork.Collection.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CollectionName
                }),
        };

        if (objFromDb is Watch watch)
        {
            productVM.Watch = watch;
            productVM.WatchBrand = watch.WatchBrand;
            productVM.WatchStrapType = watch.WatchStrapType;
            productVM.WatchMovement = watch.WatchMovement;
            productVM.WaterResistant = watch.WaterResistant;
            productVM.DialColor = watch.DialColor;
            productVM.WatchDiameter = watch.WatchDiameter;
            productVM.WatchCaseShape = watch.WatchCaseShape;
            productVM.StrapColour = watch.StrapColour;
            productVM.ByGemstone = watch.ByGemstone;
            productVM.ByMetal = watch.ByMetal;
            productVM.ByOccassion = watch.ByOccassion;
        }

        return View(productVM);
    }

    // GET: EditPg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        var collectionsForProduct = _unitOfWork.Product_Collection
            .GetAll(pc => pc.ProductId == id)
            .Select(pc => pc.CollectionId ?? 0) // Handle possible null values
            .ToList()
            .ToArray();

        ProductVM productVM = new ProductVM
        {
            Id = objFromDb.Id,
            ProductName = objFromDb.ProductName,
            ProductNumber = objFromDb.ProductNumber,
            ModelNumber = objFromDb.ModelNumber,
            Supplier = objFromDb.Supplier,
            CostOfProduct = objFromDb.CostOfProduct,
            TargetPrice01 = objFromDb.TargetPrice01,
            TargetPrice02 = objFromDb.TargetPrice02,
            TargetPrice03 = objFromDb.TargetPrice03,
            Price = objFromDb.Price,
            PriceWas = objFromDb.PriceWas,
            Gender = objFromDb.Gender,
            Quantity = objFromDb.Quantity,
            RemainigQuantity = objFromDb.RemainigQuantity,
            StockStatus = objFromDb.StockStatus,
            WarrantyYears = objFromDb.WarrantyYears,
            ProductDescription = objFromDb.ProductDescription,
            PrimaryDisplayImageUrl = objFromDb.PrimaryDisplayImageUrl,
            SecondaryDisplayImageUrl = objFromDb.SecondaryDisplayImageUrl,
            SliderImageUrlOne = objFromDb.SliderImageUrlOne,
            SliderImageUrlTwo = objFromDb.SliderImageUrlTwo,
            SliderImageUrlThree = objFromDb.SliderImageUrlThree,
            SliderImageUrlFour = objFromDb.SliderImageUrlFour,
            CreatedDateTime = objFromDb.CreatedDateTime,
            LastUpdate = objFromDb.LastUpdate,
            ProductCategory = objFromDb.ProductCategory,
            SelectedCollectionIds = collectionsForProduct,
            CollectionList = _unitOfWork.Collection.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CollectionName
                }),
        };

        if (objFromDb is Watch watch)
        {
            productVM.Watch = watch;
            productVM.WatchBrand = watch.WatchBrand;
            productVM.WatchStrapType = watch.WatchStrapType;
            productVM.WatchMovement = watch.WatchMovement;
            productVM.WaterResistant = watch.WaterResistant;
            productVM.DialColor = watch.DialColor;
            productVM.WatchDiameter = watch.WatchDiameter;
            productVM.WatchCaseShape = watch.WatchCaseShape;
            productVM.StrapColour = watch.StrapColour;
            productVM.ByGemstone = watch.ByGemstone;
            productVM.ByMetal = watch.ByMetal;
            productVM.ByOccassion = watch.ByOccassion;
        }

        return View(productVM);
    }

    // POST: EditPg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ProductVM productVM)
    {
        if (ModelState.IsValid)
        {
            var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            if (productVM.CostOfProduct == productVM.Price)
            {
                ModelState.AddModelError("CostOfProduct", "The cost of each product can't be the same as the price");
            }
            else if (productVM.Price <= productVM.CostOfProduct)
            {
                ModelState.AddModelError("Price", "The price can't be less than the cost of each product");
            }

            var productQuantity = productVM.Quantity;

            if (productQuantity == 0)
            {

                productVM.StockStatus = SD.OutOfStockStatus;

            }
            else if (productQuantity <= 50)
            {

                productVM.StockStatus = SD.RunningLowStockStatus;

            }
            else if (productQuantity >= 50)
            {

                productVM.StockStatus = SD.InStockStatus;

            }

            objFromDb.Id = productVM.Id;
            objFromDb.ProductName = productVM.ProductName;
            objFromDb.ProductNumber = productVM.ProductNumber;
            objFromDb.ModelNumber = productVM.ModelNumber;
            objFromDb.Supplier = productVM.Supplier;
            objFromDb.CostOfProduct = productVM.CostOfProduct;
            objFromDb.TargetPrice01 = productVM.TargetPrice01;
            objFromDb.TargetPrice02 = productVM.TargetPrice02;
            objFromDb.TargetPrice03 = productVM.TargetPrice03;
            objFromDb.Price = productVM.Price;
            objFromDb.PriceWas = productVM.PriceWas;
            objFromDb.Gender = productVM.Gender;
            objFromDb.Quantity = productVM.Quantity;
            objFromDb.RemainigQuantity = productVM.RemainigQuantity;
            objFromDb.StockStatus = productVM.StockStatus;
            objFromDb.WarrantyYears = productVM.WarrantyYears;
            objFromDb.ProductDescription = productVM.ProductDescription;
            objFromDb.PrimaryDisplayImageUrl = productVM.PrimaryDisplayImageUrl;
            objFromDb.SecondaryDisplayImageUrl = productVM.SecondaryDisplayImageUrl;
            objFromDb.SliderImageUrlOne = productVM.SliderImageUrlOne;
            objFromDb.SliderImageUrlTwo = productVM.SliderImageUrlTwo;
            objFromDb.SliderImageUrlThree = productVM.SliderImageUrlThree;
            objFromDb.SliderImageUrlFour = productVM.SliderImageUrlFour;
            objFromDb.LastUpdate = productVM.LastUpdate;

            objFromDb.ProductCategory = objFromDb.ProductCategory;

            if (objFromDb is Watch watch)
            {
                watch.WatchBrand = productVM.WatchBrand;
                watch.WatchStrapType = productVM.WatchStrapType;
                watch.WatchMovement = productVM.WatchMovement;
                watch.WaterResistant = productVM.WaterResistant;
                watch.DialColor = productVM.DialColor;
                watch.WatchDiameter = productVM.WatchDiameter;
                watch.WatchCaseShape = productVM.WatchCaseShape;
                watch.StrapColour = productVM.StrapColour;
                watch.ByGemstone = productVM.ByGemstone;
                watch.ByMetal = productVM.ByMetal;
                watch.ByOccassion = productVM.ByOccassion;
            }

            objFromDb.LastUpdate = DateTime.Now;

            // Remove existing collections associated with the product
            var existingCollections = _unitOfWork.Product_Collection.GetAll(pc => pc.ProductId == id);
            _unitOfWork.Product_Collection.RemoveRange(existingCollections);

            if (productVM.SelectedCollectionIds == null || !productVM.SelectedCollectionIds.Any())
            {
                // If no collections are selected, retrieve the ID of "No collection" from the database
                var noCollectionId = _unitOfWork.Collection.GetFirstOrDefault(c => c.CollectionName == "No collection")?.Id;
                if (noCollectionId != null)
                {
                    productVM.SelectedCollectionIds = new int[] { noCollectionId.Value }; 
                }
            }

            // Add new collections selected in the form
            foreach (var collectionId in productVM.SelectedCollectionIds)
            {
                var collectionName = _unitOfWork.Collection.GetFirstOrDefault(c => c.Id == collectionId)?.CollectionName;
                if (collectionName != null)
                {
                    var productCollection = new Product_Collection
                    {
                        ProductId = id,
                        CollectionId = collectionId,
                        CollectionName = collectionName
                    };
                    _unitOfWork.Product_Collection.Add(productCollection);
                }
            }

            // Save changes
            _unitOfWork.product.Update(objFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Watch product updated successfully";

            return RedirectToAction("Index");
        }

        // Repopulate collection list in case of validation errors
        productVM.CollectionList = _unitOfWork.Collection.GetAll()
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CollectionName
            });

        return View(productVM);
    }

    // GET: DeletePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.Watch.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        return View(objFromDb);

    }

    // POST: DeletePg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var objFromDb = _unitOfWork.Watch.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        _unitOfWork.Watch.Remove(objFromDb);
        _unitOfWork.Save();

        TempData["success"] = "Watch product deleted successfully";

        return RedirectToAction("Index");
    }

}
