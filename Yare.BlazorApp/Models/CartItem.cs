namespace Yare.BlazorApp.Models;

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public double Price { get; set; }
    public int Count { get; set; }
    public string? ImageUrl { get; set; }
    public string? ProductCategory { get; set; }
}
