using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nest;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.ViewModels;
using Yare_WebApplication.Data.Utility;
using Product = Yare.Models.Product;

namespace Yare_WebApplication.Areas.Customer.Controllers;
[Area("Customer")]
[Authorize]
public class CheckoutController : Controller
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    [BindProperty]
    public HomePgVM HomePgVM { get; set; }

    public CheckoutController(ILogger<CheckoutController> logger, IUnitOfWork unitOfWork)
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

    public IActionResult DeliveryDetails()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var homePgVM = new HomePgVM()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
            includeProperties: "Product"),
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader()
            }
        };

        // Retrieve User Details
        homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

        if (homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser != null)
        {
            homePgVM.ShoppingCartVM.OrderHeader.FirstName = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.FirstName;
            homePgVM.ShoppingCartVM.OrderHeader.LastName = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.LastName;
            homePgVM.ShoppingCartVM.OrderHeader.PhoneNumber = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            homePgVM.ShoppingCartVM.OrderHeader.StreetAdress = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
            homePgVM.ShoppingCartVM.OrderHeader.City = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.City;
            homePgVM.ShoppingCartVM.OrderHeader.Borough = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.Borough;
            homePgVM.ShoppingCartVM.OrderHeader.PostCode = homePgVM.ShoppingCartVM.OrderHeader.ApplicationUser.PostCode;
        }

        //Get product added to cart 
        foreach (var cartItem in homePgVM.ShoppingCartList)
        {
            cartItem.Price = GetPrice(cartItem.Count, cartItem.Product.Price);
            homePgVM.ShoppingCartVM.OrderHeader.OrderTotal += (cartItem.Price * cartItem.Count);
        }

        return View(homePgVM);
    }

    [HttpPost]
    [ActionName("DeliveryDetails")]
    [ValidateAntiForgeryToken]
    public IActionResult DeliveryDetailsPOST(HomePgVM homePgVM, OrderDetail orderDetails)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        HomePgVM.ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
         includeProperties: "Product");

        if (HomePgVM.ShoppingCartVM.ShoppingCartList == null || !HomePgVM.ShoppingCartVM.ShoppingCartList.Any())
        {
            TempData["DebugMessage"] = "ShoppingCartList is empty.";
            TempData["ErrorMessage"] = "Your shopping bag is currently empty. Please add products to proceed with your order.";
            return RedirectToAction("DeliveryDetails");
        }

        HomePgVM.ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        HomePgVM.ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
        HomePgVM.ShoppingCartVM.OrderHeader.OrderTotal = 0; // Ensure OrderTotal is initialized

        foreach (var CartItem in HomePgVM.ShoppingCartVM.ShoppingCartList)
        {
            CartItem.Price = GetPrice(CartItem.Count, CartItem.Product.Price);
            HomePgVM.ShoppingCartVM.OrderHeader.OrderTotal += (CartItem.Price * CartItem.Count);

            CartItem.Product.RemainigQuantity -= CartItem.Count;
            _unitOfWork.product.Update(CartItem.Product);
            _unitOfWork.Save();
        }

        // Debug: Check the calculated OrderTotal
        TempData["DebugOrderTotal"] = HomePgVM.ShoppingCartVM.OrderHeader.OrderTotal.ToString();

        if (HomePgVM.ShoppingCartVM.OrderHeader.OrderTotal == 0)
        {
            TempData["DebugMessage"] = "OrderTotal is zero after calculation.";
            TempData["ErrorMessage"] = "Your shopping bag is currently empty. Please add products to proceed with your order.";
            return RedirectToAction("DeliveryDetails");
        }

        // Check if the OrderTotal exceeds the limit
        if (HomePgVM.ShoppingCartVM.OrderHeader.OrderTotal >= 999999.99)
        {
            TempData["DebugMessage"] = "OrderTotal exceeds the limit.";
            TempData["ErrorMessage"] = "Your checkout total exceeds the £999,999.99 limit. Please reduce the amount and try again.";
            return RedirectToAction("DeliveryDetails");
        }

        HomePgVM.ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        HomePgVM.ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusPending;

        _unitOfWork.OrderHeader.Add(HomePgVM.ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        foreach (var CartItem in HomePgVM.ShoppingCartVM.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = CartItem.ProductId,
                OrderHeaderId = HomePgVM.ShoppingCartVM.OrderHeader.Id,
                Price = CartItem.Price,
                Count = CartItem.Count,
            };

            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        // Stripe API Settings
        //var domain = "https://localhost:7176/";
        var domain = "https://seuna5-001-site1.jtempurl.com/";

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"customer/checkout/OrderConfirmation?id={HomePgVM.ShoppingCartVM.OrderHeader.Id}",
            CancelUrl = domain + $"customer/checkout/DeliveryDetails",
        };

        // Loop through shoppingCart
        foreach (var cartItems in HomePgVM.ShoppingCartVM.ShoppingCartList)
        {
            var SessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(cartItems.Price * 100), // 20.00 -> 2000 * 100
                    Currency = "gbp",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = cartItems.Product.ProductName
                    },
                },
                Quantity = cartItems.Count,
            };
            options.LineItems.Add(SessionLineItem);
        }

        try
        {
            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentId(HomePgVM.ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        catch (StripeException ex)
        {
            TempData["DebugMessage"] = "Stripe exception occurred.";
            TempData["ErrorMessage"] = "There was an error processing your payment. Please try again.";
            return RedirectToAction("DeliveryDetails");
        }
    }

    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
        var service = new SessionService();
        Session session = service.Get(orderHeader.SessionId);

        // Check the stripe status
        if (session.PaymentStatus.ToLower() == "paid")
        {
            _unitOfWork.OrderHeader.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
            _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
            _unitOfWork.Save();
        }

        List<ShoppingCart> shoppingCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId
        == orderHeader.ApplicationUserId).ToList();

        HttpContext.Session.Clear();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCart);
        _unitOfWork.Save();

        TempData["success"] = "Order has been paid successfully";

        var homePgVM = new HomePgVM
        {
            OrderHeader = orderHeader,
            ShoppingCartList = shoppingCart
        };

        return View(homePgVM);
    }

    private double GetPrice(double quantity, double price)
    {
        return price;
    }

}
