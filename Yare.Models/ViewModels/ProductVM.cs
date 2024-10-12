using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.Models.Enums;

namespace Yare.Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        public IEnumerable<Product>? objProductList { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }

        [Display(Name = "Product Number")]
        public string? ProductNumber { get; set; }

        [Display(Name = "Model Number")]
        public string? ModelNumber { get; set; }

        [Display(Name = "Supplier")]
        public string? Supplier { get; set; }

        [Display(Name = "Cost of each product")]
        public double CostOfProduct { get; set; }

        [Display(Name = "Target Price 01")]
        public double TargetPrice01 { get; set; }

        [Display(Name = "Target Price 02")]
        public double TargetPrice02 { get; set; }

        [Display(Name = "Target Price 03")]
        public double TargetPrice03 { get; set; }

        public double Price { get; set; }

        [Display(Name = "Price Was")]
        public double PriceWas { get; set; }

        public Gender? Gender { get; set; }

        public int? Quantity { get; set; }

        [Display(Name = "Remaining Quantity")]
        public int RemainigQuantity { get; set; }

        [Display(Name = "Stock Status")]
        public string? StockStatus { get; set; }

        [Display(Name = "Warranty Years")]
        public WarrantyYears? WarrantyYears { get; set; }

        [Display(Name = "Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "Main Image")]
        public string? PrimaryDisplayImageUrl { get; set; }

        [Display(Name = "Secondary Image")]
        public string? SecondaryDisplayImageUrl { get; set; }

        [Display(Name = "Slider Image 1")]
        public string? SliderImageUrlOne { get; set; }

        [Display(Name = "Slider Image 2")]
        public string? SliderImageUrlTwo { get; set; }

        [Display(Name = "Slider Image 3")]
        public string? SliderImageUrlThree { get; set; }

        [Display(Name = "Slider Image 4")]
        public string? SliderImageUrlFour { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public int? CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        [ValidateNever]
        public Collection? Collection { get; set; }

        [Display(Name = "Collection")]
        public string? CollectionName { get; set; }

        public int? Product_CollectionId { get; set; }
        [ForeignKey("Product_CollectionId")]
        [ValidateNever]
        public Product_Collection? Product_Collection { get; set; }

        public int[]? SelectedCollectionIds { get; set; } = new int[0];

        public ICollection<Product_Collection>? Product_CollectionsList { get; set; }

        public IEnumerable<SelectListItem>? CollectionList { get; set; }

        public List<string>? CollectionNames { get; set; }

        public Watch? Watch { get; set; }

        [Display(Name = "Watch Brand")]
        public WatchBrand? WatchBrand { get; set; }

        [Display(Name = "Strap Type")]
        public WatchStrapType? WatchStrapType { get; set; }

        [Display(Name = "Watch Movement")]
        public WatchMovement? WatchMovement { get; set; }

        [Display(Name = "Water Resistant")]
        public WaterResistant? WaterResistant { get; set; }

        [Display(Name = "Dial Color")]
        public string? DialColor { get; set; }

        [Display(Name = "Watch Diameter")]
        public WatchDiameter? WatchDiameter { get; set; }

        [Display(Name = "Case Shape")]
        public WatchCaseShape? WatchCaseShape { get; set; }

        [Display(Name = "Strap Colour")]
        public string? StrapColour { get; set; }

        //Both Properties Watches and Jewellery
        [Display(Name = "Gemstone")]
        public ByGemstone? ByGemstone { get; set; }

        [Display(Name = "Metal")]
        public ByMetal? ByMetal { get; set; }

        [Display(Name = "By Occassion")]
        public ByOccassion? ByOccassion { get; set; }
        //
        public Jewellery? Jewellery { get; set; }

        [Display(Name = "Jewellery Category")]
        public JewelleryCategory? JewelleryCategory { get; set; }

        public Accessory? Accessory { get; set; }

        [Display(Name = "Accessory Category")]
        public AccessoryCategory? AccessoryCategory { get; set; }

        [Display(Name = "Product Type")]
        public ProductCategory? ProductCategory { get; set; }

        public IEnumerable<SelectListItem>? ProductCategoryList { get; set; }

    }

}
