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
            // Move ShoppingCartList into ShoppingCartVM
            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product").ToList(), 
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

        // Get product added to cart 
        foreach (var cartItem in homePgVM.ShoppingCartVM.ShoppingCartList)
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
        // Get the current user's identity (claims)
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        // Always reload the ShoppingCartList
        homePgVM.ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
            u => u.ApplicationUserId == claim.Value,
            includeProperties: "Product").ToList();

        // Check if the ShoppingCart is empty
        if (homePgVM.ShoppingCartVM.ShoppingCartList == null || !homePgVM.ShoppingCartVM.ShoppingCartList.Any())
        {
            TempData["ErrorMessage"] = "Your shopping bag is currently empty. Please add products to proceed with your order.";
            return RedirectToAction("DeliveryDetails");
        }

        // If the ModelState is valid, process the order
        if (ModelState.IsValid)
        {
            // Order processing logic
            homePgVM.ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            homePgVM.ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            homePgVM.ShoppingCartVM.OrderHeader.OrderTotal = 0;

            // Process each cart item
            foreach (var cartItem in homePgVM.ShoppingCartVM.ShoppingCartList)
            {
                cartItem.Price = GetPrice(cartItem.Count, cartItem.Product.Price);
                homePgVM.ShoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;

                // Reduce product quantity
                cartItem.Product.RemainigQuantity -= cartItem.Count;
                _unitOfWork.product.Update(cartItem.Product);
            }

            _unitOfWork.Save(); // Save after updating product quantities

            if (homePgVM.ShoppingCartVM.OrderHeader.OrderTotal == 0)
            {
                TempData["ErrorMessage"] = "Your shopping bag is empty. Please add items to proceed.";
                return RedirectToAction("DeliveryDetails");
            }

            // Check if the total exceeds the limit
            if (homePgVM.ShoppingCartVM.OrderHeader.OrderTotal >= 999999.99)
            {
                TempData["ErrorMessage"] = "Your checkout total exceeds the £999,999.99 limit. Please reduce the amount.";
                return RedirectToAction("DeliveryDetails");
            }

            homePgVM.ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            homePgVM.ShoppingCartVM.OrderHeader.OrderStatus = SD.PaymentStatusPending;

            // Save the OrderHeader
            _unitOfWork.OrderHeader.Add(homePgVM.ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            // Add order details
            foreach (var cartItem in homePgVM.ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cartItem.ProductId,
                    OrderHeaderId = homePgVM.ShoppingCartVM.OrderHeader.Id,
                    Price = cartItem.Price,
                    Count = cartItem.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
            }

            _unitOfWork.Save();

            // Stripe payment session
            //var domain = "https://localhost:7176/";
            var domain = "https://seuna5-001-site1.jtempurl.com/";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/checkout/OrderConfirmation?id={homePgVM.ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/checkout/DeliveryDetails"
            };

            foreach (var cartItem in homePgVM.ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cartItem.Price * 100),
                        Currency = "gbp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cartItem.Product.ProductName
                        }
                    },
                    Quantity = cartItem.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            try
            {
                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(homePgVM.ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303); // Redirect to Stripe
            }
            catch (StripeException ex)
            {
                TempData["ErrorMessage"] = "There was an error processing your payment. Please try again.";
                return RedirectToAction("DeliveryDetails");
            }
        }
        else
        {
            // If ModelState is invalid, ensure we still load and return the cart items
            homePgVM.ShoppingCartVM.OrderHeader.OrderTotal = 0;

            foreach (var cartItem in homePgVM.ShoppingCartVM.ShoppingCartList)
            {
                cartItem.Price = GetPrice(cartItem.Count, cartItem.Product.Price);
                homePgVM.ShoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            // Return the view with the current cart and validation messages
            return View(homePgVM);
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
