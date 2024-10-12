using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Yare.Models;

namespace Yare_WebApplication.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingBagListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public HomePgVM HomePgVM { get; set; }

        public ShoppingBagListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private double GetPrice(double quantity, double price)
        {
            return price; 
        }

        [HttpPost]
        public JsonResult IncrementItem(int cartItemId)
        {
            var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartItemId);
            _unitOfWork.ShoppingCart.IncrementCount(cartItem, 1);
            _unitOfWork.Save();

            var homePgVM = GetShoppingCartView();

            // Prepare the JSON data to be returned
            var items = homePgVM.ShoppingCartList.Select(item => new
            {
                id = item.Id,
                count = item.Count,
                price = GetPrice(item.Count, item.Product.Price).ToString("c")
            }).ToList();

            return Json(new
            {
                success = true,
                items = items,
                orderTotal = homePgVM.OrderHeader.OrderTotal.ToString("c")
            });
        }

        [HttpPost]
        public JsonResult DecrementItem(int cartItemId)
        {
            var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartItemId);
            if (cartItem.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartItem);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartItem.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32("SessionCart", count);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cartItem, 1);
            }
            _unitOfWork.Save();

            var homePgVM = GetShoppingCartView();

            // Prepare the JSON data to be returned
            var items = homePgVM.ShoppingCartList.Select(item => new
            {
                id = item.Id,
                count = item.Count,
                price = GetPrice(item.Count, item.Product.Price).ToString("c")
            }).ToList();

            return Json(new
            {
                success = true,
                items = items,
                orderTotal = homePgVM.OrderHeader.OrderTotal.ToString("c")
            });
        }

        [HttpPost]
        public JsonResult RemoveItem(int cartItemId)
        {
            var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartItemId);
            _unitOfWork.ShoppingCart.Remove(cartItem);
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartItem.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart", count);

            var homePgVM = GetShoppingCartView();

            // Prepare the JSON data to be returned
            var items = homePgVM.ShoppingCartList.Select(item => new
            {
                id = item.Id,
                count = item.Count,
                price = GetPrice(item.Count, item.Product.Price).ToString("c")
            }).ToList();

            return Json(new
            {
                success = true,
                items = items,
                orderTotal = homePgVM.OrderHeader.OrderTotal.ToString("c")
            });
        }

        private HomePgVM GetShoppingCartView()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            var homePgVM = new HomePgVM
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var cartItem in homePgVM.ShoppingCartList)
            {
                cartItem.Price = GetPrice(cartItem.Count, cartItem.Product.Price);
                homePgVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            return homePgVM;
        }
    }
}

