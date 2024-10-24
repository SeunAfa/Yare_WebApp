using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nest;
using Stripe;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.Enums;
using Yare.Models.ViewModels;
using Yare_WebApplication.Data.Utility;
using System.Collections.Generic;

using Product = Yare.Models.Product;

namespace Yare_WebApplication.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
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

        IEnumerable<Product> objProductList = _unitOfWork.product.GetAll();

        if (!string.IsNullOrEmpty(searchString))
        {
            var filterResult = objProductList
                .Where(w => ProductMatchesSearchString(w, searchString))
                .OrderBy(w => w.ProductName.Length)
                .ToList();

            if (filterResult.Any())
            {
                ViewBag.SearchString = searchString;
                ViewBag.SearchValueResults = filterResult;
                return PartialView("_ProductSearch_Results", filterResult);
            }
            else
            {
                ViewBag.SearchString = searchString;
                ViewBag.NoResults = "No results found";
                return PartialView("_ProductSearch_Results");
            }
        }

        return PartialView("_ProductSearch_Results", objProductList);
    }

    private bool ProductMatchesSearchString(Product product, string searchString)
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
                    break; // No need to check other properties if the term is found
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
        IEnumerable<Product> objProductList = _unitOfWork.product.GetAll();

        //Perform pagination on the server side
        var paginatedList = objProductList.Skip((page - 1) * pageSize).Take(pageSize);

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling(objProductList.Count() / (double)pageSize);

        foreach (var product in paginatedList)
        {
            var remainigQuantity = product.RemainigQuantity;

            if (remainigQuantity == 0)
            {
                product.StockStatus = SD.OutOfStockStatus;
            }
            else if (remainigQuantity <= 50)
            {
                product.StockStatus = SD.RunningLowStockStatus;
            }
            else
            {
                product.StockStatus = SD.InStockStatus;
            }

            _unitOfWork.Save();

        }

        return View(paginatedList);
    }

    // GET: DetailsPg
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
         .Select(pc => pc.CollectionId ?? 0)
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

        if (objFromDb is Yare.Models.Watch watch)
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
        else if (objFromDb is Jewellery jewellery)
        {
            productVM.Jewellery = jewellery;
            productVM.JewelleryCategory = jewellery.JewelleryCategory;
            productVM.ByGemstone = jewellery.ByGemstone;
            productVM.ByMetal = jewellery.ByMetal;
            productVM.ByOccassion = jewellery.ByOccassion;
        }
        else if (objFromDb is Accessory accessory)
        {
            productVM.Accessory = accessory;
            productVM.AccessoryCategory = accessory.AccessoryCategory;
        }

        return View(productVM);

    }

    // GET: EditPg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
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

        if (objFromDb is Yare.Models.Watch watch)
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
        else if (objFromDb is Jewellery jewellery)
        {
            productVM.Jewellery = jewellery;
            productVM.JewelleryCategory = jewellery.JewelleryCategory;
            productVM.ByGemstone = jewellery.ByGemstone;
            productVM.ByMetal = jewellery.ByMetal;
            productVM.ByOccassion = jewellery.ByOccassion;
        }
        else if (objFromDb is Accessory accessory)
        {
            productVM.Accessory = accessory;
            productVM.AccessoryCategory = accessory.AccessoryCategory;
        }

        return View(productVM);

    }

    // POST: EditPg 
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin)]
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

            // Custom Model Errors
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

            // Update common properties model base class
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

            // Update common model derived classes properties
            if (objFromDb is Yare.Models.Watch watch)
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
            else if (objFromDb is Jewellery jewellery)
            {
                jewellery.JewelleryCategory = productVM.JewelleryCategory;
                jewellery.ByGemstone = productVM.ByGemstone;
                jewellery.ByMetal = productVM.ByMetal;
                jewellery.ByOccassion = productVM.ByOccassion;
            }
            else if (objFromDb is Accessory accessory)
            {
                accessory.AccessoryCategory = productVM.AccessoryCategory;
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

            _unitOfWork.product.Update(objFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Product updated successfully";


            return RedirectToAction("Index");
        }

        productVM.CollectionList = _unitOfWork.Collection.GetAll()
        .Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.CollectionName
        });

        return View(productVM);
    }

    // GET: DeletePg
    [Authorize(Roles = SD.Role_MasterAdmin)]
    public IActionResult Delete(int? id)
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
        };

        if (objFromDb is Yare.Models.Watch watch)
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

        }
        else if (objFromDb is Jewellery jewellery)
        {
            productVM.Jewellery = jewellery;
            productVM.JewelleryCategory = jewellery.JewelleryCategory;
        }
        else if (objFromDb is Accessory accessory)
        {
            productVM.Accessory = accessory;
            productVM.AccessoryCategory = accessory.AccessoryCategory;
        }

        return View(productVM);

    }

    // POST: DeletePg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin)]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int id, ProductVM productVM)
    {

        if (ModelState.IsValid)
        {
            var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            // Get common properties
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

            //  Get common model derived classes properties 
            if (objFromDb is Yare.Models.Watch watch)
            {
                watch.WatchBrand = productVM.WatchBrand;
                watch.WatchStrapType = productVM.WatchStrapType;
                watch.WatchMovement = productVM.WatchMovement;
                watch.WaterResistant = productVM.WaterResistant;
                watch.DialColor = productVM.DialColor;
                watch.WatchDiameter = productVM.WatchDiameter;
                watch.WatchCaseShape = productVM.WatchCaseShape;
                watch.StrapColour = productVM.StrapColour;
            }
            else if (objFromDb is Jewellery jewellery)
            {
                jewellery.JewelleryCategory = productVM.JewelleryCategory;
            }
            else if (objFromDb is Accessory accessory)
            {
                accessory.AccessoryCategory = productVM.AccessoryCategory;
            }

            objFromDb.LastUpdate = DateTime.Now;

            _unitOfWork.product.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index");
        }

        return View(productVM);

    }

}
