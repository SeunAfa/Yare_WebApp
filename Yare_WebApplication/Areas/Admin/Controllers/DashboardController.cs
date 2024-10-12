using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models.Enums;
using Yare_WebApplication.Data.Utility;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Yare.Models;
using Nest;
using Yare.Models.ViewModels;
using Yare.DataAccess.Repository;
using System.Threading.Tasks;


namespace Yare_WebApplication.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_MasterAdmin + "," + SD.Role_Admin + "," + SD.Role_Employee)]
    public class DashboardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;

        }

        // Calculate percentage change method
        double CalculatePercentageChange(double previousValue, double currentValue)
        {
            if (previousValue == 0)
            {
                return currentValue == 0 ? 0 : 100;
            }

            double percentageChange = ((currentValue - previousValue) / Math.Abs(previousValue)) * 100;

            // Scale down the percentage change if it exceeds 100%
            if (percentageChange > 100 || percentageChange < -100)
            {
                percentageChange /= 10;
            }

            // Round to two decimal places
            return Math.Round(percentageChange, 2);
        }

        public IActionResult Index()
        { 

            // Common time periods used across all KPIs
            var currentDateTime = DateTime.Now;
            var previousDateTime = currentDateTime.AddDays(-15); // Define this only once and use consistently

            /// Revenue 
            double previousRevenue = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate < previousDateTime)
                .Sum(order => order.OrderTotal);

            double currentRevenue = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate >= previousDateTime)
                .Sum(order => order.OrderTotal);

            double revenueChangePercentage = CalculatePercentageChange(previousRevenue, currentRevenue);

            ViewBag.PreviousRevenue = previousRevenue;
            ViewBag.CurrentRevenue = currentRevenue;
            ViewBag.RevenueChangePercentage = revenueChangePercentage;

            /// Average Order
            var previousAverageOrderDate = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate < previousDateTime)
                .ToList();

            double previousTotalOrderAverage = previousAverageOrderDate.Sum(order => order.OrderTotal);
            int previousOrderCountAverage = previousAverageOrderDate.Count();
            double previousAverageOrderAverage = previousOrderCountAverage > 0 ? previousTotalOrderAverage / previousOrderCountAverage : 0;

            var currentAverageOrderDate = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate >= previousDateTime)
                .ToList();

            double currentTotalOrderAverage = currentAverageOrderDate.Sum(order => order.OrderTotal);
            int currentOrderCountAverage = currentAverageOrderDate.Count();
            double currentAverageOrderAverage = currentOrderCountAverage > 0 ? currentTotalOrderAverage / currentOrderCountAverage : 0;

            double averageOrderChangePercentage = CalculatePercentageChange(previousAverageOrderAverage, currentAverageOrderAverage);

            ViewBag.PreviousAverageOrderAverage = previousAverageOrderAverage;
            ViewBag.CurrentAverageOrderAverage = currentAverageOrderAverage;
            ViewBag.AverageOrderChangePercentage = averageOrderChangePercentage;

            /// Users Account Total
            double previousAccountsTotal = _unitOfWork.ApplicationUser
                .GetAll(u => u.CreatedDate <= previousDateTime)
                .Count();

            double currentAccountsTotal = _unitOfWork.ApplicationUser
                .GetAll(u => u.CreatedDate <= currentDateTime)
                .Count();

            double accountsTotalChangePercentage = CalculatePercentageChange(previousAccountsTotal, currentAccountsTotal);

            ViewBag.PreviousAccountsTotal = previousAccountsTotal;
            ViewBag.CurrentAccountsTotal = currentAccountsTotal;
            ViewBag.AccountsTotalChangePercentage = accountsTotalChangePercentage;

            /// Processing Orders
            double previousProcessingOrders = _unitOfWork.OrderHeader
               .GetAll(p => p.OrderStatus == SD.StatusInProcess && p.OrderDate < previousDateTime).Count();

            double currentProcessingOrders = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusInProcess && p.OrderDate >= previousDateTime).Count();

            double processingOrdersChangePercentage = CalculatePercentageChange(previousProcessingOrders, currentProcessingOrders);

            ViewBag.PreviousProcessingOrders = previousProcessingOrders;
            ViewBag.CurrentProcessingOrders = currentProcessingOrders;
            ViewBag.ProcessingOrdersChangePercentage = processingOrdersChangePercentage;

            /// Expenses
            var previousOrders = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate < previousDateTime)
                .ToList();

            var currentOrders = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate >= previousDateTime)
                .ToList();

            //Calculate the cost of product in each order
            double CalculateProductCost(IEnumerable<OrderHeader> orders)
            {
                double totalCost = 0.0;

                foreach (var order in orders)
                {
                    var orderDetails = _unitOfWork.OrderDetail
                        .GetAll(od => od.OrderHeaderId == order.Id)
                        .ToList();

                    foreach (var orderDetail in orderDetails)
                    {
                        var product = _unitOfWork.product
                            .GetFirstOrDefault(p => p.Id == orderDetail.ProductId);

                        totalCost += product.CostOfProduct * orderDetail.Count;
                    }
                }

                return totalCost;
            }

            double previousCostOfEachProduct = CalculateProductCost(previousOrders);
            double currentCostOfEachProduct = CalculateProductCost(currentOrders);

            double expensesChangePercentage = CalculatePercentageChange(previousCostOfEachProduct, currentCostOfEachProduct);

            ViewBag.PreviousExpense = previousCostOfEachProduct;
            ViewBag.CurrentExpense = currentCostOfEachProduct;
            ViewBag.ExpensesChangePercentage = expensesChangePercentage;

            /// Total Orders   
            double previousTotalOrders = _unitOfWork.OrderHeader
               .GetAll(p => p.OrderDate < previousDateTime).Count();

            double currentTotalOrders = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderDate >= previousDateTime).Count();

            double totalOrdersChangePercentage = CalculatePercentageChange(previousTotalOrders, currentTotalOrders);

            ViewBag.PreviousTotalOrders = previousTotalOrders;
            ViewBag.CurrentTotalOrders = currentTotalOrders;
            ViewBag.TotalOrdersChangePercentage = totalOrdersChangePercentage;

            /// Order Invoices 
            double previousOrderInvoices = _unitOfWork.OrderHeader
               .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate < previousDateTime)
               .Count();

            double currentOrderInvoices = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped && p.OrderDate >= previousDateTime)
                .Count();

            double orderInvoicesChangePercentage = CalculatePercentageChange(previousOrderInvoices, currentOrderInvoices);

            ViewBag.PreviousOrderInvoices = previousOrderInvoices;
            ViewBag.CurrentOrderInvoices = currentOrderInvoices;
            ViewBag.OrderInvoicesChangePercentage = orderInvoicesChangePercentage;

            /// Cancelled Orders 
            double previousCancelledOrders = _unitOfWork.OrderHeader
               .GetAll(p => p.OrderStatus == SD.StatusCancelled && p.OrderDate < previousDateTime)
               .Count();

            double currentCancelledOrders = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusCancelled && p.OrderDate >= previousDateTime)
                .Count();

            double cancelledOrdersChangePercentage = CalculatePercentageChange(previousCancelledOrders, currentCancelledOrders);

            ViewBag.PreviousCancelledOrders = previousCancelledOrders;
            ViewBag.CurrentCancelledOrders = currentCancelledOrders;
            ViewBag.CancelledOrdersChangePercentage = cancelledOrdersChangePercentage;



            //Order Status Key
            var pendingOrdersCount = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusPending).Count();
            var approvedOrdersCount = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusApproved).Count();
            var processingOrdersCount = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusInProcess).Count();
            var shippedOrdersCount = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped).Count();
            var cancelledOrdersCount = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusCancelled).Count();
            var refundedOrdersCount = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusRefunded).Count();

            ViewBag.PendingOrders = pendingOrdersCount;
            ViewBag.ApprovedOrders = approvedOrdersCount;
            ViewBag.ProcessingOrders = processingOrdersCount;
            ViewBag.ShippedOrders = shippedOrdersCount;
            ViewBag.CancelledOrders = cancelledOrdersCount;
            ViewBag.RefundedOrders = refundedOrdersCount;

            return View();

        }

        [HttpGet]
        public IActionResult Revenue()
        {

            var Revenue = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped);

            var days = new List<string>();
            var orderTotals = new List<double>();

            foreach (var day in Revenue.GroupBy(o => o.OrderDate.Date))
            {
                days.Add(day.First().OrderDate.ToString("d,M,y"));
                orderTotals.Add(day.Sum(o => o.OrderTotal));
            }

            return Json(new { days, orderTotals });
        }

        [HttpGet]
        public IActionResult Expenses()
        {
            // Retrieve orders for the current period
            var currentOrders = _unitOfWork.OrderHeader
                .GetAll(p => p.OrderStatus == SD.StatusShipped);

            var days = new List<string>();
            var orderExpenses = new List<double>();

            foreach (var dayGroup in currentOrders.GroupBy(o => o.OrderDate.Date))
            {
                var totalCostForDay = 0.0;

                foreach (var order in dayGroup)
                {
                    var orderDetails = _unitOfWork.OrderDetail
                        .GetAll(od => od.OrderHeaderId == order.Id)
                        .ToList();

                    foreach (var orderDetail in orderDetails)
                    {
                        var product = _unitOfWork.product
                            .GetFirstOrDefault(p => p.Id == orderDetail.ProductId);

                        totalCostForDay += product.CostOfProduct * orderDetail.Count;
                    }
                }

                days.Add(dayGroup.Key.ToString("d,M,y"));
                orderExpenses.Add(totalCostForDay);
            }

            return Json(new { days, orderExpenses });
        }

        [HttpGet]
        public IActionResult ProductsSoldByCategory()
        {
            var shippedOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped);

            // Initialize a dictionary to hold category-wise product counts
            var productCategoryCount = new Dictionary<string, int>
    {
        { ProductCategory.Watch.ToString(), 0 },
        { ProductCategory.Jewellery.ToString(), 0 },
        { ProductCategory.Accessory.ToString(), 0 }
    };

            // Loop through each shipped order
            foreach (var order in shippedOrders)
            {
                // Fetch order details for each order
                var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == order.Id).ToList();

                // Loop through each order detail
                foreach (var orderDetail in orderDetails)
                {
                    // Fetch the product for each order detail
                    var product = _unitOfWork.product.GetFirstOrDefault(p => p.Id == orderDetail.ProductId);

                    // Get each product category count
                    if (product != null)
                    {
                        if (product.ProductCategory == ProductCategory.Watch)
                        {
                            productCategoryCount[ProductCategory.Watch.ToString()] += orderDetail.Count;
                        }
                        else if (product.ProductCategory == ProductCategory.Jewellery)
                        {
                            productCategoryCount[ProductCategory.Jewellery.ToString()] += orderDetail.Count;
                        }
                        else if (product.ProductCategory == ProductCategory.Accessory)
                        {
                            productCategoryCount[ProductCategory.Accessory.ToString()] += orderDetail.Count;
                        }
                    }
                }
            }

            // Return the result as JSON
            return Json(productCategoryCount);
        }

        [HttpGet]
        public IActionResult BestSellingProducts()
        {
            int minimumCountThreshold = 30; // Minimum count for  product as best-selling

            // Retrieve data from repositories
            var products = _unitOfWork.product.GetAll().ToList();
            var collections = _unitOfWork.Collection.GetAll().ToList();
            var orderDetails = _unitOfWork.OrderDetail.GetAll().ToList();
            var orderHeaders = _unitOfWork.OrderHeader.GetAll().ToList();

            // Filter orders by those that are shipped and meet the count threshold
            var shippedOrderIds = orderHeaders
                .Where(o => o.OrderStatus == SD.StatusShipped)
                .Select(o => o.Id)
                .ToList();

            // Get the best-selling products by count from shipped orders
            var bestSellingProducts = orderDetails
                .Where(od => shippedOrderIds.Contains(od.OrderHeaderId)) // Filter by shipped order IDs
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalCount = g.Sum(od => od.Count),
                })
                .Where(x => x.TotalCount >= minimumCountThreshold) // Apply the minimum count threshold
                .OrderByDescending(x => x.TotalCount)
                .ToList();

            if (!bestSellingProducts.Any())
            {
                return NotFound("No best-selling products found.");
            }

            // Ensure the best-selling products are not in the "Best Sellers" collection
            var bestSellersCollectionName = "Best Sellers";
            var bestSellersCollection = collections.FirstOrDefault(c => c.CollectionName == bestSellersCollectionName);
            if (bestSellersCollection == null)
            {
                return NotFound("Best Sellers collection not found.");
            }
            int bestSellersCollectionId = bestSellersCollection.Id;

            var productIdsInBestSellers = _unitOfWork.Product_Collection
                .GetAll(pc => pc.CollectionId == bestSellersCollectionId)
                .Select(pc => pc.ProductId)
                .ToList();

            var bestSellingProductsNotInCollection = bestSellingProducts
                .Where(p => !productIdsInBestSellers.Contains(p.ProductId))
                .ToList();

            // Add best-selling products to the "Best Sellers" collection if not already in collection
            foreach (var bestSeller in bestSellingProductsNotInCollection)
            {
                var productCollection = new Product_Collection
                {
                    ProductId = bestSeller.ProductId,
                    CollectionId = bestSellersCollectionId
                };
                _unitOfWork.Product_Collection.Add(productCollection);
            }
            _unitOfWork.Save();

            // Create a JSON response with the best-selling products
            var result = bestSellingProducts
                .Select(bs => new
                {
                    ProductId = bs.ProductId,
                    ProductName = products.FirstOrDefault(p => p.Id == bs.ProductId)?.ProductName,
                    TotalCount = bs.TotalCount
                })
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public IActionResult DeliveryStatus()
        {
            var PendingOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusPending);
            var ApprovedOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusApproved);
            var ProcessingOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusInProcess);
            var ShippedOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped);
            var CancelledOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusCancelled);
            var RefundedOrders = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusRefunded);

            var orderStatuses = new Dictionary<string, int>
    {
        { "Pending", PendingOrders.Count() },
        { "Approved", ApprovedOrders.Count() },
        { "Processing", ProcessingOrders.Count() },
        { "Shipped", ShippedOrders.Count() },
        { "Cancelled", CancelledOrders.Count() },
        { "Refunded", RefundedOrders.Count() }
    };

            return Json(orderStatuses);
        }

        [HttpGet]
        public IActionResult SalesDay()
        {
            var salesByDay = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped);

            var days = new List<string>();
            var orderTotals = new List<double>();

            foreach (var day in salesByDay.GroupBy(o => o.OrderDate.Date))
            {
                days.Add(day.First().OrderDate.ToString("yyyy-MM-dd"));
                orderTotals.Add(day.Sum(o => o.OrderTotal));
            }

            return Json(new { days, orderTotals });
        }

        [HttpGet]
        public IActionResult SalesWeekly()
        {
            var salesByWeek = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped);

            var weeks = new List<string>();
            var orderTotals = new List<double>();

            foreach (var week in salesByWeek.GroupBy(o => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.OrderDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday)))
            {
                var startDate = week.First().OrderDate.AddDays(-(int)week.First().OrderDate.DayOfWeek);
                weeks.Add(startDate.ToString("yyyy-MM-dd"));
                orderTotals.Add(week.Sum(o => o.OrderTotal));
            }

            return Json(new { weeks, orderTotals });
        }

        [HttpGet]
        public IActionResult SalesMonth()
        {
            var salesByMonth = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped);

            var months = new List<string>();
            var orderTotals = new List<double>();

            foreach (var month in salesByMonth.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month }))
            {
                months.Add(new DateTime(month.Key.Year, month.Key.Month, 1).ToString("yyyy-MM"));
                orderTotals.Add(month.Sum(o => o.OrderTotal));
            }

            return Json(new { months, orderTotals });
        }

        [HttpGet]
        public IActionResult SalesYear()
        {
            var salesByYear = _unitOfWork.OrderHeader.GetAll(p => p.OrderStatus == SD.StatusShipped);

            var years = new List<string>();
            var orderTotals = new List<double>();

            foreach (var year in salesByYear.GroupBy(o => o.OrderDate.Year))
            {
                years.Add(year.First().OrderDate.ToString("yyyy"));
                orderTotals.Add(year.Sum(o => o.OrderTotal));
            }

            return Json(new { years, orderTotals });
        }

        [HttpGet]
        public IActionResult AllOrderDataByUsers()
        {
            // Retrieve all orders including the related ApplicationUser
            var allOrders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            // Group the orders by ApplicationUserId and calculate both total and average order amounts for each user
            var orderDataByUsers = allOrders
                .GroupBy(order => order.ApplicationUserId)
                .Select(group => new
                {
                    CustomerName = group.First().ApplicationUser != null ? group.First().ApplicationUser.FirstName + " " + group.First().ApplicationUser.LastName : "Unknown",
                    TotalOrderAmount = group.Sum(order => order.OrderTotal),
                    AverageOrderAmount = group.Average(order => order.OrderTotal)
                })
                .ToList();

            // Extract labels and data for the chart
            var labels = orderDataByUsers.Select(user => user.CustomerName).ToList();
            var totalData = orderDataByUsers.Select(user => user.TotalOrderAmount).ToList();
            var averageData = orderDataByUsers.Select(user => user.AverageOrderAmount).ToList();

            // Return the result as a JSON object with `labels`, `totalData`, and `averageData` properties
            return Json(new { labels, totalData, averageData });
        }

        [HttpGet]
        public IActionResult ProductsByCategory()
        {
            var watchCount = _unitOfWork.product.GetAll(p => p.ProductCategory == ProductCategory.Watch).Count();
            var jewelleryCount = _unitOfWork.product.GetAll(p => p.ProductCategory == ProductCategory.Jewellery).Count();
            var accessoryCount = _unitOfWork.product.GetAll(p => p.ProductCategory == ProductCategory.Accessory).Count();

            var data = new
            {
                labels = new[] { "Watch", "Jewellery", "Accessory" },
                datasets = new[] {
            new {
                label = "Products Count",
                data = new[] { watchCount, jewelleryCount, accessoryCount },
                backgroundColor = new[] { "#2d53de", "#553772", "#8f3b76" },
                borderColor = new[] { "#2d53de", "#553772", "#8f3b76" },
                borderWidth = 1
            }
        }
            };

            return Json(data);
        }

        [HttpGet]
        public IActionResult ProductsByGender()
        {
            var maleProductCount = _unitOfWork.product.GetAll(p => p.Gender == Gender.Male).Count();
            var femaleProductCount = _unitOfWork.product.GetAll(p => p.Gender == Gender.Female).Count();
            var unisexProductCount = _unitOfWork.product.GetAll(p => p.Gender == Gender.Unisex).Count();

            var data = new
            {
                labels = new[] { "Mens", "Ladies", "Unisex" },
                datasets = new[] {
            new {
                label = "Product Gender Count",
                data = new[] { maleProductCount, femaleProductCount, unisexProductCount },
                backgroundColor = new[] { "#2d53de", "#553772", "#8f3b76" },
                borderColor = new[] { "#2d53de", "#553772", "#8f3b76" },
                borderWidth = 1
            }
        }
            };

            return Json(data);
        }

        [HttpGet]
        public IActionResult ProductQuantity()
        {
            // Retrieve all products
            var products = _unitOfWork.product.GetAll();

            var productIds = new List<int>();
            var productNames = new List<string>();
            var productQuantities = new List<int>();

            // Include product IDs and calculate total quantities
            foreach (var product in products)
            {
                productIds.Add(product.Id);
                productNames.Add(product.ProductName);
                productQuantities.Add(product.Quantity ?? 0); // Handle nullable Quantity
            }

            return Json(new { productIds, productNames, productQuantities });
        }

        [HttpGet]
        public IActionResult ProductRemainingQuantity()
        {
            // Retrieve all products
            var products = _unitOfWork.product.GetAll();

            var productIds = new List<int>();
            var productNames = new List<string>();
            var remainingQuantities = new List<int>();

            // Include product IDs and calculate remaining quantities
            foreach (var product in products)
            {
                productIds.Add(product.Id);
                productNames.Add(product.ProductName);
                remainingQuantities.Add(product.RemainigQuantity); // Correct spelling
            }

            return Json(new { productIds, productNames, remainingQuantities });
        }

        [HttpGet]
        public IActionResult ProductPrice()
        {
            var productPrice = _unitOfWork.product.GetAll();

            var products = productPrice.Select(product => new
            {
                productName = product.ProductName,
                price = product.Price
            }).ToList();

            return Json(products);
        }

    }
}
