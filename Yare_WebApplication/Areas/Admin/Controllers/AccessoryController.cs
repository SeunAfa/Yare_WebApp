using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nest;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.Enums;
using Yare.Models.ViewModels;
using Yare_WebApplication.Data.Utility;

namespace Yare_WebApplication.Areas.Admin.Controllers;
[Area("Admin")]
public class AccessoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AccessoryController(IUnitOfWork unitOfWork)
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

        IEnumerable<Accessory> objAccessoryList = _unitOfWork.Accessory.GetAll();

        if (!string.IsNullOrEmpty(searchString))
        {
            var filterResult = objAccessoryList
                .Where(w => ProductMatchesSearchString(w, searchString))
                .OrderBy(w => w.ProductName.Length)
                .ToList();

            if (filterResult.Any())
            {
                ViewBag.SearchString = searchString;
                ViewBag.SearchValueResults = filterResult;
                return PartialView("_AccessorySearch_Results", filterResult);
            }
            else
            {
                ViewBag.SearchString = searchString;
                ViewBag.NoResults = "No results found";
                return PartialView("_AccessorySearch_Results");
            }
        }

        return PartialView("_AccessorySearch_Results", objAccessoryList);
    }

    private bool ProductMatchesSearchString(Accessory product, string searchString)
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
        IEnumerable<Accessory> objAccessoryList = _unitOfWork.Accessory.GetAll();

        // Perform pagination on the server side
        var paginatedList = objAccessoryList.Skip((page - 1) * pageSize).Take(pageSize);

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = (int)Math.Ceiling(objAccessoryList.Count() / (double)pageSize);

        foreach (var accessory in paginatedList)
        {
            var remainigQuantity = accessory.RemainigQuantity;

            if (remainigQuantity == 0)
            {
                accessory.StockStatus = SD.OutOfStockStatus;
            }
            else if (remainigQuantity <= 50)
            {
                accessory.StockStatus = SD.RunningLowStockStatus;
            }
            else
            {
                accessory.StockStatus = SD.InStockStatus;
            }

            _unitOfWork.Save();

        }

        return View(paginatedList);
    }

    // Get: CreatePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
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

            // Initialize ProductCategoryList for the view model pre select the value of Accessory 
            ProductCategoryList = Enum.GetValues(typeof(ProductCategory))
              .Cast<ProductCategory>()
              .Select(pc => new SelectListItem
              {
                  Value = pc.ToString(),
                  Text = pc.ToString(),
                  Selected = pc == ProductCategory.Accessory
              })
        };

        return View(productVM);

    }

    // Post: CreatePg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
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
            // Repopulate any necessary data in the view model and return to the view
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

                // Create an instance of Accessory and populate base class properties
                var accessory = new Accessory
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
                    ProductCategory = ProductCategory.Accessory,
                    AccessoryCategory = productVM.AccessoryCategory,
                    Product_CollectionsList = selectedCollections,
                };

                _unitOfWork.Accessory.Add(accessory);
                _unitOfWork.Save();

                // Save Product_Collection entries
                foreach (var productCollection in selectedCollections)
                {
                    productCollection.ProductId = accessory.Id; // Assign the product ID
                    _unitOfWork.Product_Collection.Add(productCollection);
                }
                _unitOfWork.Save();

                TempData["success"] = "Accessory product created successfully";

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
                Selected = pc == ProductCategory.Accessory
            });

        return View(productVM);

    }

    // Get: DetailsPg
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

        if (objFromDb is Accessory accessory)
        {
            productVM.Accessory = accessory;
            productVM.AccessoryCategory = accessory.AccessoryCategory;
        }

        return View(productVM);
    }

    // Get: EditPg
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

        if (objFromDb is Accessory accessory)
        {
            productVM.Accessory = accessory;
            productVM.AccessoryCategory = accessory.AccessoryCategory;

        }

        return View(productVM);

    }

    // Post: EditPg
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

            // Update common model derived classes properties
            if (objFromDb is Accessory accessory)
            {
                productVM.Accessory = accessory;
                productVM.AccessoryCategory = accessory.AccessoryCategory;
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
                    productVM.SelectedCollectionIds = new int[] { noCollectionId.Value }; // Use int[] here instead of List<int>
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

            TempData["success"] = "Accessory product updated successfully";

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

    // Get: DeletePg
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var objFromDb = _unitOfWork.Accessory.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        return View(objFromDb);

    }

    // Post: DeletePg
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var objFromDb = _unitOfWork.Accessory.GetFirstOrDefault(u => u.Id == id);

        if (objFromDb == null)
        {
            return NotFound();
        }

        _unitOfWork.Accessory.Remove(objFromDb);
        _unitOfWork.Save();

        TempData["success"] = "Accessory product deleted successfully";

        return RedirectToAction("Index");
    }

}
