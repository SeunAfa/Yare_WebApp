using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nest;
using Stripe;
using System.Collections;
using System.Security.Cryptography;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.Enums;
using Yare.Models.ViewModels;
using Yare_WebApplication.Data.Utility;
using Product = Yare.Models.Product;

namespace Yare_WebApplication.Areas.Admin.Controllers;

[Area("Admin")]
public class CollectionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CollectionController(IUnitOfWork unitOfWork)
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

        IEnumerable<Collection> objCollectionList = _unitOfWork.Collection.GetAll();

        if (!string.IsNullOrEmpty(searchString))
        {

            var filterResult = objCollectionList
    .Where(w =>
        (w.CollectionName != null && w.CollectionName.Length > 0 && char.ToLower(w.CollectionName.First()) == char.ToLower(searchString.First()))
    )
    .OrderBy(w => w.CollectionName.Length)
    .ToList();

            if (filterResult.Any())
            {
                ViewBag.SearchString = searchString; 
                ViewBag.SearchValueResults = filterResult;
                return PartialView("_CollectionSearch_Results", filterResult);
            }
            else
            {
                ViewBag.SearchString = searchString; 
                ViewBag.NoResults = "No results found";
                return PartialView("_CollectionSearch_Results");
            }
        }

        return PartialView("_CollectionSearch_Results", objCollectionList);
    }

    // GET: HomePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        IEnumerable<Collection> objCollectionList = _unitOfWork.Collection.GetAll();

        // Perform pagination on the server side
        var paginatedList = objCollectionList.Skip((page - 1) * pageSize).Take(pageSize);

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling(objCollectionList.Count() / (double)pageSize);

        return View(paginatedList);
    }

    // GET: CreatePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    public IActionResult Create()
    {
        return View();
    }

    // POST: CreatePg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin)]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CollectionVM collectionVM)
    {
        // Check if there are any model errors
        if (ModelState.IsValid)
        {
            if (collectionVM != null)
            {
                collectionVM.CreatedDateTime = DateTime.UtcNow;
                collectionVM.LastUpdate = DateTime.UtcNow;

                // Create an instance of Watch and populate base class properties
                var collection = new Collection
                {
                    Id = collectionVM.Id,
                    CollectionName = collectionVM.CollectionName,
                    CreatedDateTime = collectionVM.CreatedDateTime,
                    LastUpdate = collectionVM.LastUpdate,   
                };

                _unitOfWork.Collection.Add(collection);
                _unitOfWork.Save();

                TempData["success"] = "Collection created successfully";

                return RedirectToAction("Index");
            }
        }

        return View(collectionVM);

    }

    // GET: DetailsPg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Details(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        CollectionVM collectionVM = new CollectionVM
        {
            Id = objFromDb.Id,
            CollectionName = objFromDb.CollectionName,
            CreatedDateTime = objFromDb.CreatedDateTime,
            LastUpdate = objFromDb.LastUpdate,
        };

        return View(collectionVM);
    }

    // POST: EditPg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        CollectionVM collectionVM = new CollectionVM
        {
            Id = objFromDb.Id,
            CollectionName = objFromDb.CollectionName,
            CreatedDateTime = objFromDb.CreatedDateTime,
            LastUpdate = objFromDb.LastUpdate,
        };

        return View(collectionVM);
    }

    // POST: EditPg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin)]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CollectionVM collectionVM)
    {

        if (ModelState.IsValid)
        {
            var objFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

            if (objFromDb == null)
            {
                return NotFound();
            }

            objFromDb.Id = collectionVM.Id;
            objFromDb.CollectionName = collectionVM.CollectionName;
            objFromDb.LastUpdate = collectionVM.LastUpdate;

            objFromDb.LastUpdate = DateTime.UtcNow;

            _unitOfWork.Collection.Update(objFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Collection updated successfully";

            return RedirectToAction("Index"); 
        }

        return View(collectionVM);
    }

    // GET: DeletePg
    [Authorize(Roles = SD.Role_MasterAdmin)]
    public IActionResult Delete(int? id)
    {
        if (id == 0 || id == null)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        return View(objFromDb);
    }

    // POST: DeletePg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin)]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {

        var objFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        _unitOfWork.Collection.Remove(objFromDb);
        _unitOfWork.Save();

        TempData["success"] = "Collection deleted successfully";

        return RedirectToAction("Index");
    }

}






















































































//public IActionResult Details(int? id)
//{
//    if (id == null || id == 0)
//    {
//        return NotFound();
//    }

//    var ObjCollectionFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

//    if (ObjCollectionFromDb == null)
//    {
//        return NotFound();
//    }

//    CollectionVM CollectionVM = new CollectionVM
//    {
//        Id = ObjCollectionFromDb.Id,
//        CollectionName = ObjCollectionFromDb.CollectionName,
//        ProductId = ObjCollectionFromDb.Product?.Id ?? 0
//    };

//    ViewBag.ProductId = new SelectList(_unitOfWork.product.GetAll(), "Id", "ProductName", CollectionVM.ProductId);

//    return View(CollectionVM);
//}




//public IActionResult Details(int? id)
//{
//    if (id == null || id == 0)
//    {
//        return NotFound();
//    }

//    var ObjCollectionFromDb = _unitOfWork.Collection.GetFirstOrDefault(u => u.Id == id);

//    if (ObjCollectionFromDb == null)
//    {
//        return NotFound();
//    }

//    CollectionVM CollectionVM = new CollectionVM
//    {
//        Id = ObjCollectionFromDb.Id,
//        CollectionName = ObjCollectionFromDb.CollectionName,
//        ProductId = ObjCollectionFromDb.Product?.Id ?? 0
//    };

//    ViewBag.ProductId = new SelectList(_unitOfWork.product.GetAll(), "Id", "ProductName", CollectionVM.ProductId);

//    return View(CollectionVM);
//}






































////Get - DetailsPg
//public IActionResult Details(int? id)
//{
//    if (id == null || id == 0)
//    {
//        return NotFound();
//    }

//    var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

//    if (objFromDb == null)
//    {
//        return NotFound();
//    }

//    collectionVM collectionVM = new collectionVM
//    {
//        Id = objFromDb.Id,
//        ProductName = objFromDb.ProductName,
//        ProductNumber = objFromDb.ProductNumber,
//        ModelNumber = objFromDb.ModelNumber,
//        Supplier = objFromDb.Supplier,
//        CostOfProduct = objFromDb.CostOfProduct,
//        TargetPrice01 = objFromDb.TargetPrice01,
//        TargetPrice02 = objFromDb.TargetPrice02,
//        TargetPrice03 = objFromDb.TargetPrice03,
//        Price = objFromDb.Price,
//        PriceWas = objFromDb.PriceWas,
//        Gender = objFromDb.Gender,
//        Quantity = objFromDb.Quantity,
//        RemainigQuantity = objFromDb.RemainigQuantity,
//        StockStatus = objFromDb.StockStatus,
//        WarrantyYears = objFromDb.WarrantyYears,
//        ProductDescription = objFromDb.ProductDescription,
//        PrimaryDisplayImageUrl = objFromDb.PrimaryDisplayImageUrl,
//        SecondaryDisplayImageUrl = objFromDb.SecondaryDisplayImageUrl,
//        SliderImageUrlOne = objFromDb.SliderImageUrlOne,
//        SliderImageUrlTwo = objFromDb.SliderImageUrlTwo,
//        SliderImageUrlThree = objFromDb.SliderImageUrlThree,
//        SliderImageUrlFour = objFromDb.SliderImageUrlFour,
//        CreatedDateTime = objFromDb.CreatedDateTime,
//        LastUpdate = objFromDb.LastUpdate,

//    };

//    if (objFromDb is Yare.Models.Watch watch)
//    {
//        collectionVM.Watch = watch;
//        collectionVM.WatchBrand = watch.WatchBrand;
//        collectionVM.WatchStrapType = watch.WatchStrapType;
//        collectionVM.WatchMovement = watch.WatchMovement;
//        collectionVM.WaterResistant = watch.WaterResistant;
//        collectionVM.DialColor = watch.DialColor;
//        collectionVM.WatchDiameter = watch.WatchDiameter;
//        collectionVM.WatchCaseShape = watch.WatchCaseShape;
//        collectionVM.StrapColour = watch.StrapColour;

//    }
//    else if (objFromDb is Jewellery jewellery)
//    {
//        collectionVM.Jewellery = jewellery;
//        collectionVM.JewelleryCategory = jewellery.JewelleryCategory;
//    }
//    else if (objFromDb is Accessory accessory)
//    {
//        collectionVM.Accessory = accessory;
//        collectionVM.AccessoryCategory = accessory.AccessoryCategory;
//    }



//    return View(collectionVM);

//}

////Get - EditPg
//public IActionResult Edit(int? id)
//{
//    if (id == null || id == 0)
//    {
//        return NotFound();
//    }

//    var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

//    if (objFromDb == null)
//    {
//        return NotFound();
//    }

//    collectionVM collectionVM = new collectionVM
//    {
//        Id = objFromDb.Id,
//        ProductName = objFromDb.ProductName,
//        ProductNumber = objFromDb.ProductNumber,
//        ModelNumber = objFromDb.ModelNumber,
//        Supplier = objFromDb.Supplier,
//        CostOfProduct = objFromDb.CostOfProduct,
//        TargetPrice01 = objFromDb.TargetPrice01,
//        TargetPrice02 = objFromDb.TargetPrice02,
//        TargetPrice03 = objFromDb.TargetPrice03,
//        Price = objFromDb.Price,
//        PriceWas = objFromDb.PriceWas,
//        Gender = objFromDb.Gender,
//        Quantity = objFromDb.Quantity,
//        RemainigQuantity = objFromDb.RemainigQuantity,
//        StockStatus = objFromDb.StockStatus,
//        WarrantyYears = objFromDb.WarrantyYears,
//        ProductDescription = objFromDb.ProductDescription,
//        PrimaryDisplayImageUrl = objFromDb.PrimaryDisplayImageUrl,
//        SecondaryDisplayImageUrl = objFromDb.SecondaryDisplayImageUrl,
//        SliderImageUrlOne = objFromDb.SliderImageUrlOne,
//        SliderImageUrlTwo = objFromDb.SliderImageUrlTwo,
//        SliderImageUrlThree = objFromDb.SliderImageUrlThree,
//        SliderImageUrlFour = objFromDb.SliderImageUrlFour,
//        CreatedDateTime = objFromDb.CreatedDateTime,
//        LastUpdate = objFromDb.LastUpdate,

//    };

//    if (objFromDb is Yare.Models.Watch watch)
//    {
//        collectionVM.Watch = watch;
//        collectionVM.WatchBrand = watch.WatchBrand;
//        collectionVM.WatchStrapType = watch.WatchStrapType;
//        collectionVM.WatchMovement = watch.WatchMovement;
//        collectionVM.WaterResistant = watch.WaterResistant;
//        collectionVM.DialColor = watch.DialColor;
//        collectionVM.WatchDiameter = watch.WatchDiameter;
//        collectionVM.WatchCaseShape = watch.WatchCaseShape;
//        collectionVM.StrapColour = watch.StrapColour;

//    }
//    else if (objFromDb is Jewellery jewellery)
//    {
//        collectionVM.Jewellery = jewellery;
//        collectionVM.JewelleryCategory = jewellery.JewelleryCategory;
//    }
//    else if (objFromDb is Accessory accessory)
//    {
//        collectionVM.Accessory = accessory;
//        collectionVM.AccessoryCategory = accessory.AccessoryCategory;
//    }

//    return View(collectionVM);

//}

//[HttpPost]
//[ValidateAntiForgeryToken]
//public IActionResult Edit(int id, collectionVM collectionVM)
//{
//    // Custom Model Errors
//    if (collectionVM.CostOfProduct == collectionVM.Price)
//    {
//        ModelState.AddModelError("CostOfProduct", "The cost of each product can't be the same as the price");
//    }
//    else if (collectionVM.Price <= collectionVM.CostOfProduct)
//    {
//        ModelState.AddModelError("Price", "The price can't be less than the cost of each product");
//    }

//    // Other conditions...

//    if (ModelState.IsValid)
//    {
//        var objFromDbproduct = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

//        if (objFromDbproduct == null)
//        {
//            return NotFound();
//        }

//        // Update common properties
//        objFromDbproduct.ProductName = collectionVM.ProductName;
//        objFromDbproduct.ProductNumber = collectionVM.ProductNumber;
//        objFromDbproduct.ModelNumber = collectionVM.ModelNumber;
//        objFromDbproduct.Supplier = collectionVM.Supplier;
//        objFromDbproduct.CostOfProduct = collectionVM.CostOfProduct;
//        objFromDbproduct.TargetPrice01 = collectionVM.TargetPrice01;
//        objFromDbproduct.TargetPrice02 = collectionVM.TargetPrice02;
//        objFromDbproduct.TargetPrice03 = collectionVM.TargetPrice03;
//        objFromDbproduct.Price = collectionVM.Price;
//        objFromDbproduct.PriceWas = collectionVM.PriceWas;
//        objFromDbproduct.Gender = collectionVM.Gender;
//        objFromDbproduct.Quantity = collectionVM.Quantity;
//        objFromDbproduct.RemainigQuantity = collectionVM.RemainigQuantity;
//        objFromDbproduct.StockStatus = collectionVM.StockStatus;
//        objFromDbproduct.WarrantyYears = collectionVM.WarrantyYears;
//        objFromDbproduct.ProductDescription = collectionVM.ProductDescription;
//        objFromDbproduct.PrimaryDisplayImageUrl = collectionVM.PrimaryDisplayImageUrl;
//        objFromDbproduct.SecondaryDisplayImageUrl = collectionVM.SecondaryDisplayImageUrl;
//        objFromDbproduct.SliderImageUrlOne = collectionVM.SliderImageUrlOne;
//        objFromDbproduct.SliderImageUrlTwo = collectionVM.SliderImageUrlTwo;
//        objFromDbproduct.SliderImageUrlThree = collectionVM.SliderImageUrlThree;
//        objFromDbproduct.SliderImageUrlFour = collectionVM.SliderImageUrlFour;

//        // Update type-specific properties
//        if (objFromDbproduct is Yare.Models.Watch watch)
//        {
//            watch.WatchBrand = collectionVM.WatchBrand;
//            watch.WatchStrapType = collectionVM.WatchStrapType;
//            watch.WatchMovement = collectionVM.WatchMovement;
//            watch.WaterResistant = collectionVM.WaterResistant;
//            watch.DialColor = collectionVM.DialColor;
//            watch.WatchDiameter = collectionVM.WatchDiameter;
//            watch.WatchCaseShape = collectionVM.WatchCaseShape;
//            watch.StrapColour = collectionVM.StrapColour;
//        }
//        else if (objFromDbproduct is Jewellery jewellery)
//        {
//            jewellery.JewelleryCategory = collectionVM.JewelleryCategory;
//        }
//        else if (objFromDbproduct is Accessory accessory)
//        {
//            accessory.AccessoryCategory = collectionVM.AccessoryCategory;
//        }

//        objFromDbproduct.LastUpdate = DateTime.UtcNow;

//        _unitOfWork.product.Update(objFromDbproduct);
//        _unitOfWork.Save();

//        return RedirectToAction("Index"); // or any other action
//    }

//    return View(collectionVM);
//}

////Get - EditPg
//public IActionResult Delete(int? id)
//{
//    if (id == null || id == 0)
//    {
//        return NotFound();
//    }

//    var objFromDb = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

//    if (objFromDb == null)
//    {
//        return NotFound();
//    }

//    collectionVM collectionVM = new collectionVM
//    {
//        Id = objFromDb.Id,
//        ProductName = objFromDb.ProductName,
//        ProductNumber = objFromDb.ProductNumber,
//        ModelNumber = objFromDb.ModelNumber,
//        Supplier = objFromDb.Supplier,
//        CostOfProduct = objFromDb.CostOfProduct,
//        TargetPrice01 = objFromDb.TargetPrice01,
//        TargetPrice02 = objFromDb.TargetPrice02,
//        TargetPrice03 = objFromDb.TargetPrice03,
//        Price = objFromDb.Price,
//        PriceWas = objFromDb.PriceWas,
//        Gender = objFromDb.Gender,
//        Quantity = objFromDb.Quantity,
//        RemainigQuantity = objFromDb.RemainigQuantity,
//        StockStatus = objFromDb.StockStatus,
//        WarrantyYears = objFromDb.WarrantyYears,
//        ProductDescription = objFromDb.ProductDescription,
//        PrimaryDisplayImageUrl = objFromDb.PrimaryDisplayImageUrl,
//        SecondaryDisplayImageUrl = objFromDb.SecondaryDisplayImageUrl,
//        SliderImageUrlOne = objFromDb.SliderImageUrlOne,
//        SliderImageUrlTwo = objFromDb.SliderImageUrlTwo,
//        SliderImageUrlThree = objFromDb.SliderImageUrlThree,
//        SliderImageUrlFour = objFromDb.SliderImageUrlFour,
//        CreatedDateTime = objFromDb.CreatedDateTime,
//        LastUpdate = objFromDb.LastUpdate,

//    };

//    if (objFromDb is Yare.Models.Watch watch)
//    {
//        collectionVM.Watch = watch;
//        collectionVM.WatchBrand = watch.WatchBrand;
//        collectionVM.WatchStrapType = watch.WatchStrapType;
//        collectionVM.WatchMovement = watch.WatchMovement;
//        collectionVM.WaterResistant = watch.WaterResistant;
//        collectionVM.DialColor = watch.DialColor;
//        collectionVM.WatchDiameter = watch.WatchDiameter;
//        collectionVM.WatchCaseShape = watch.WatchCaseShape;
//        collectionVM.StrapColour = watch.StrapColour;

//    }
//    else if (objFromDb is Jewellery jewellery)
//    {
//        collectionVM.Jewellery = jewellery;
//        collectionVM.JewelleryCategory = jewellery.JewelleryCategory;
//    }
//    else if (objFromDb is Accessory accessory)
//    {
//        collectionVM.Accessory = accessory;
//        collectionVM.AccessoryCategory = accessory.AccessoryCategory;
//    }

//    return View(collectionVM);

//}

//[HttpPost]
//[ValidateAntiForgeryToken]
//public IActionResult DeletePost(int id, collectionVM collectionVM)
//{

//    if (ModelState.IsValid)
//    {
//        var objFromDbproduct = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id);

//        if (objFromDbproduct == null)
//        {
//            return NotFound();
//        }

//        // Update common properties
//        objFromDbproduct.ProductName = collectionVM.ProductName;
//        objFromDbproduct.ProductNumber = collectionVM.ProductNumber;
//        objFromDbproduct.ModelNumber = collectionVM.ModelNumber;
//        objFromDbproduct.Supplier = collectionVM.Supplier;
//        objFromDbproduct.CostOfProduct = collectionVM.CostOfProduct;
//        objFromDbproduct.TargetPrice01 = collectionVM.TargetPrice01;
//        objFromDbproduct.TargetPrice02 = collectionVM.TargetPrice02;
//        objFromDbproduct.TargetPrice03 = collectionVM.TargetPrice03;
//        objFromDbproduct.Price = collectionVM.Price;
//        objFromDbproduct.PriceWas = collectionVM.PriceWas;
//        objFromDbproduct.Gender = collectionVM.Gender;
//        objFromDbproduct.Quantity = collectionVM.Quantity;
//        objFromDbproduct.RemainigQuantity = collectionVM.RemainigQuantity;
//        objFromDbproduct.StockStatus = collectionVM.StockStatus;
//        objFromDbproduct.WarrantyYears = collectionVM.WarrantyYears;
//        objFromDbproduct.ProductDescription = collectionVM.ProductDescription;
//        objFromDbproduct.PrimaryDisplayImageUrl = collectionVM.PrimaryDisplayImageUrl;
//        objFromDbproduct.SecondaryDisplayImageUrl = collectionVM.SecondaryDisplayImageUrl;
//        objFromDbproduct.SliderImageUrlOne = collectionVM.SliderImageUrlOne;
//        objFromDbproduct.SliderImageUrlTwo = collectionVM.SliderImageUrlTwo;
//        objFromDbproduct.SliderImageUrlThree = collectionVM.SliderImageUrlThree;
//        objFromDbproduct.SliderImageUrlFour = collectionVM.SliderImageUrlFour;

//        // Update type-specific properties
//        if (objFromDbproduct is Yare.Models.Watch watch)
//        {
//            watch.WatchBrand = collectionVM.WatchBrand;
//            watch.WatchStrapType = collectionVM.WatchStrapType;
//            watch.WatchMovement = collectionVM.WatchMovement;
//            watch.WaterResistant = collectionVM.WaterResistant;
//            watch.DialColor = collectionVM.DialColor;
//            watch.WatchDiameter = collectionVM.WatchDiameter;
//            watch.WatchCaseShape = collectionVM.WatchCaseShape;
//            watch.StrapColour = collectionVM.StrapColour;
//        }
//        else if (objFromDbproduct is Jewellery jewellery)
//        {
//            jewellery.JewelleryCategory = collectionVM.JewelleryCategory;
//        }
//        else if (objFromDbproduct is Accessory accessory)
//        {
//            accessory.AccessoryCategory = collectionVM.AccessoryCategory;
//        }

//        objFromDbproduct.LastUpdate = DateTime.UtcNow;

//        _unitOfWork.product.Remove(objFromDbproduct);
//        _unitOfWork.Save();

//        return RedirectToAction("Index"); // or any other action
//    }

//    return View(collectionVM);

//}

//}
