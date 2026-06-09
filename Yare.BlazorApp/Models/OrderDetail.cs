namespace Yare.BlazorApp.Models;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderHeaderId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
}
