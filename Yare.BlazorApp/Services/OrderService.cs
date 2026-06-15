using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Yare.BlazorApp.Models;

namespace Yare.BlazorApp.Services;

public class OrderService : IOrderService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    private List<OrderHeader>? _seedOrders;
    private List<OrderDetail>? _seedDetails;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrderService(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    private async Task<List<OrderHeader>> GetSeedOrdersAsync()
    {
        if (_seedOrders != null) return _seedOrders;
        try
        {
            var data = await _http.GetFromJsonAsync<SeedOrderData>("data/orders.json", _jsonOptions);
            _seedOrders = data?.Orders ?? new();
            _seedDetails = data?.Details ?? new();
        }
        catch
        {
            _seedOrders = new();
            _seedDetails = new();
        }
        return _seedOrders;
    }

    public async Task<List<OrderHeader>> GetAllOrdersAsync()
    {
        var seed = await GetSeedOrdersAsync();
        var local = await _localStorage.GetItemAsync<List<OrderHeader>>("yare_orders_placed") ?? new();
        return seed.Concat(local).OrderByDescending(o => o.OrderDate).ToList();
    }

    public async Task<List<OrderHeader>> GetOrdersByUserAsync(string userId)
    {
        var all = await GetAllOrdersAsync();
        return all.Where(o => o.ApplicationUserId == userId).ToList();
    }

    public async Task<OrderHeader?> GetOrderByIdAsync(int id)
    {
        var all = await GetAllOrdersAsync();
        return all.FirstOrDefault(o => o.Id == id);
    }

    public async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId)
    {
        await GetSeedOrdersAsync();
        var seedDetails = _seedDetails?.Where(d => d.OrderHeaderId == orderId).ToList() ?? new();
        var localDetails = await _localStorage.GetItemAsync<List<OrderDetail>>($"yare_details_{orderId}") ?? new();
        return seedDetails.Concat(localDetails).ToList();
    }

    public async Task<int> PlaceOrderAsync(OrderHeader order, List<CartItem> items)
    {
        var existing = await _localStorage.GetItemAsync<List<OrderHeader>>("yare_orders_placed") ?? new();
        order.Id = (existing.Any() ? existing.Max(o => o.Id) : 1000) + 1;
        order.OrderDate = DateTime.Now;
        order.OrderStatus = SD.StatusPending;
        order.PaymentStatus = SD.PaymentStatusApproved;
        order.PaymentDate = DateTime.Now;
        existing.Add(order);
        await _localStorage.SetItemAsync("yare_orders_placed", existing);

        var details = items.Select((item, i) => new OrderDetail
        {
            Id = i + 1,
            OrderHeaderId = order.Id,
            ProductId = item.ProductId,
            Count = item.Count,
            Price = item.Price,
            Engraving = item.Engraving,
            EngravingFee = item.EngravingFee
        }).ToList();
        await _localStorage.SetItemAsync($"yare_details_{order.Id}", details);

        return order.Id;
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var orders = await _localStorage.GetItemAsync<List<OrderHeader>>("yare_orders_placed") ?? new();
        var order = orders.FirstOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            order.OrderStatus = status;
            await _localStorage.SetItemAsync("yare_orders_placed", orders);
            return true;
        }
        return false;
    }
}

public class SeedOrderData
{
    public List<OrderHeader> Orders { get; set; } = new();
    public List<OrderDetail> Details { get; set; } = new();
}
