using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public interface IOrderService
{
    Task<List<OrderHeader>> GetAllOrdersAsync();
    Task<List<OrderHeader>> GetOrdersByUserAsync(string userId);
    Task<OrderHeader?> GetOrderByIdAsync(int id);
    Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId);
    Task<int> PlaceOrderAsync(OrderHeader order, List<CartItem> items);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
}
