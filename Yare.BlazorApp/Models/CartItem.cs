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

    /// <summary>Optional engraving text (Watches & Jewellery only).</summary>
    public string? Engraving { get; set; }

    /// <summary>Per-unit engraving fee. Zero when no engraving.</summary>
    public double EngravingFee { get; set; }

    /// <summary>Line total = (Price + EngravingFee) * Count.</summary>
    public double LineTotal => (Price + EngravingFee) * Count;

    public bool HasEngraving => !string.IsNullOrWhiteSpace(Engraving);
}
