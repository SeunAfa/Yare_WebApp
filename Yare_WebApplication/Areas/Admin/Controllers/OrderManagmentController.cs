using MediaBrowser.Model.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;
using Yare.Models.ViewModels;
using Yare_WebApplication.Data.Utility;

namespace Yare_WebApplication.Areas.Admin.Controllers;
[Area("Admin")]
public class OrderManagmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public OrderManagmentVM OrderManagmentVM { get; set; }

    public OrderManagmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        // Retrieve all OrderHeader items for ApplicationUsers
        IEnumerable<OrderHeader> OrderHeaderListStaffAccount = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

        // Sort the list by the Date property in descending order to get the most recent dates first
        var orderedList = OrderHeaderListStaffAccount.OrderByDescending(o => o.OrderDate);

        // Calculate the total number of pages
        int totalCount = orderedList.Count();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Get the paginated list of items
        var paginatedList = orderedList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Set ViewData for use in the view
        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = totalPages;

        // Create the view model with the paginated list
        var viewModel = new OrderManagmentVM
        {
            OrderHeaderListStaff = paginatedList,
        };

        // Return the view with the view model
        return View(viewModel);
    }

    [Authorize]
    public IActionResult OrderHistory(int page = 1, int pageSize = 10)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        // Retrieve all OrderHeader items for the user including the ApplicationUser
        IEnumerable<OrderHeader> OrderHeaderListUserAccount = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value,
            includeProperties: "ApplicationUser");

        // Sort the list by the Date property in descending order to get the most recent dates first
        var orderedList = OrderHeaderListUserAccount.OrderByDescending(o => o.OrderDate);

        // Calculate the total number of pages
        int totalCount = orderedList.Count();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Get the paginated list of items
        var paginatedList = orderedList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = totalPages;

        var viewModel = new OrderManagmentVM
        {
            OrderHeaderListUser = paginatedList,
        };

        return View(viewModel);
    }

    // Get - OrderManagmentPg - Details
    [Authorize]
    public IActionResult Details(int? id)
    {
            OrderManagmentVM = new OrderManagmentVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == id, includeProperties: "Product"),
            };
            return View(OrderManagmentVM);
    }

    // Post - OrderManagmentPg - Update
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderDetails()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagmentVM.OrderHeader.Id,tracked:false);
      
        orderHeaderFromDb.FirstName = OrderManagmentVM.OrderHeader.FirstName;
        orderHeaderFromDb.LastName = OrderManagmentVM.OrderHeader.LastName;    
        orderHeaderFromDb.PhoneNumber = OrderManagmentVM.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAdress = OrderManagmentVM.OrderHeader.StreetAdress;
        orderHeaderFromDb.City = OrderManagmentVM.OrderHeader.City;
        orderHeaderFromDb.Borough = OrderManagmentVM.OrderHeader.Borough;  
        orderHeaderFromDb.PostCode = OrderManagmentVM.OrderHeader.PostCode;

        if (orderHeaderFromDb.Carrier != null) 
        {
            orderHeaderFromDb.Carrier = OrderManagmentVM.OrderHeader.TrackingNumber;
        }
        if(orderHeaderFromDb.TrackingNumber != null)
        {
            orderHeaderFromDb.TrackingNumber = OrderManagmentVM.OrderHeader.TrackingNumber;
        }
        
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();
        TempData["success"] = "Order Details Updated Successfully";
        return RedirectToAction("Details", "OrderManagment", new { id = orderHeaderFromDb.Id });
    }

    // Post - StartProccessing - Update
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult StartProccessing()
    {
        _unitOfWork.OrderHeader.UpdateOrderStatus(OrderManagmentVM.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();
        TempData["success"] = "Order Status Updated Successfully";
        return RedirectToAction("Details", "OrderManagment", new { id = OrderManagmentVM.OrderHeader.Id });
    }

    // Post - StartProccessing - Update
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult ShipOrder()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagmentVM.OrderHeader.Id, tracked: false);

        orderHeaderFromDb.TrackingNumber = OrderManagmentVM.OrderHeader.TrackingNumber;
        orderHeaderFromDb.Carrier = OrderManagmentVM.OrderHeader.Carrier;
        orderHeaderFromDb.OrderStatus = SD.StatusShipped;
        orderHeaderFromDb.ShippingDate = DateTime.Now;

        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();
        TempData["success"] = "Order Shipped Successfully";
        return RedirectToAction("Details", "OrderManagment", new { id = OrderManagmentVM.OrderHeader.Id });
    }

    // Post - CancelOrder - Update
    [HttpPost]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin)]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagmentVM.OrderHeader.Id, tracked: false);
        if(orderHeaderFromDb.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions 
            { 
              Reason=RefundReasons.RequestedByCustomer,
              PaymentIntent=orderHeaderFromDb.PaymentIntentId,            
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeader.UpdateOrderStatus(orderHeaderFromDb.Id,SD.StatusCancelled,SD.StatusRefunded);
             
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateOrderStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);

        }
        _unitOfWork.Save();
        TempData["success"] = "Order Cancelled Successfully";
        return RedirectToAction("Details", "OrderManagment", new { id = OrderManagmentVM.OrderHeader.Id });
    }

}


