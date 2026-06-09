using System.ComponentModel.DataAnnotations;
using Yare.BlazorApp.Models.Enums;

namespace Yare.BlazorApp.Models;

public class Product
{
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string? ProductName { get; set; }

    [Display(Name = "Product Number")]
    public string? ProductNumber { get; set; }

    [Display(Name = "Model Number")]
    public string? ModelNumber { get; set; }

    public string? Supplier { get; set; }
    public double CostOfProduct { get; set; }
    public double TargetPrice01 { get; set; }
    public double TargetPrice02 { get; set; }
    public double TargetPrice03 { get; set; }
    public double Price { get; set; }
    public double PriceWas { get; set; }
    public Gender? Gender { get; set; }
    public int? Quantity { get; set; }
    public int RemainigQuantity { get; set; }
    public string? StockStatus { get; set; }
    public WarrantyYears? WarrantyYears { get; set; }

    [StringLength(500)]
    public string? ProductDescription { get; set; }

    public string? PrimaryDisplayImageUrl { get; set; }
    public string? SecondaryDisplayImageUrl { get; set; }
    public string? SliderImageUrlOne { get; set; }
    public string? SliderImageUrlTwo { get; set; }
    public string? SliderImageUrlThree { get; set; }
    public string? SliderImageUrlFour { get; set; }

    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    public DateTime LastUpdate { get; set; } = DateTime.Now;

    public ProductCategory? ProductCategory { get; set; }
    public ByMetal? ByMetal { get; set; }
    public int? Product_CollectionId { get; set; }

    // Watch-specific
    public WatchBrand? WatchBrand { get; set; }
    public WatchStrapType? WatchStrapType { get; set; }
    public WatchMovement? WatchMovement { get; set; }
    public WaterResistant? WaterResistant { get; set; }
    public string? DialColor { get; set; }
    public WatchDiameter? WatchDiameter { get; set; }
    public WatchCaseShape? WatchCaseShape { get; set; }
    public ByGemstone? ByGemstone { get; set; }
    public ByOccassion? ByOccassion { get; set; }
    public string? StrapColour { get; set; }

    // Jewellery-specific
    public JewelleryCategory? JewelleryCategory { get; set; }

    // Accessory-specific
    public AccessoryCategory? AccessoryCategory { get; set; }
}
