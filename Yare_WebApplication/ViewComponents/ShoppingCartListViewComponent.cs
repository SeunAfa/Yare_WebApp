using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.ViewModels;

namespace Yare_WebApplication.ViewComponents
{
    public class ShoppingCartListViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartListViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private double GetPrice(double quantity, double price)
        {
            return price; // Adjust pricing logic if needed
        }

        public IViewComponentResult Invoke()
        {
            return GetShoppingCartView();
        }

        private IViewComponentResult GetShoppingCartView()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return View(new HomePgVM());
            }

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return View(new HomePgVM());
            }

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

            return View(homePgVM);
        }

        
    }
}
