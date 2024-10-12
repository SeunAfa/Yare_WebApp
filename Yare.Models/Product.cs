using MediaBrowser.Model.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.Models.Enums;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Yare.Models
{
    public class Product 
    {
        [Key]
        public int Id { get; set; }
     
        [Required]
        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }
     
        [Required]
        [Display(Name = "Product Number")]
        public string? ProductNumber { get; set; }
     
        [Required]
        [Display(Name = "Model Number")]
        public string? ModelNumber { get; set; }

        [Required]
        [Display(Name = "Supplier")]
        public string? Supplier { get; set; }
     
        [Display(Name = "Cost of each product")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double CostOfProduct { get; set; }

        [Required]
        [MinValue(150)]
        [Display(Name = "Target Price 01")]
        public double TargetPrice01 { get; set; }
     
        [Required]
        [MinValue(150)]
        [Display(Name = "Target Price 02")]
        public double TargetPrice02 { get; set; }
     
        [Required]
        [MinValue(150)]
        [Display(Name = "Target Price 03")]
        public double TargetPrice03 { get; set; }
     
        [Required]
        [MinValue(150)]
        public double Price { get; set; }
     
        [Display(Name = "Price Was")]
        [MinValue(150)]
        public double PriceWas { get; set; }
     
        [Required]
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }
     
        [Required]
        public int? Quantity { get; set; }
     
     [Required]
        [Display(Name = "Remaining Quantity")]
        public int RemainigQuantity { get; set; }
        
        [Display(Name = "Stock Status")]
        public string? StockStatus { get; set; }
     
        [Required]
        [Display(Name = "Manufacturer Warranty")]
        public WarrantyYears? WarrantyYears { get; set; }
     
        [Required]
        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "Description cannot be longer than 250 characters.")]
        public string? ProductDescription { get; set; }

        [Required]
        [Display(Name = "Main Image")]
        public string? PrimaryDisplayImageUrl { get; set; }
     
        [Display(Name = "Secondary Image")]
        [Required]
        public string? SecondaryDisplayImageUrl { get; set; }
     
        [Required]
        [Display(Name = "Slider Image 1")]
        public string? SliderImageUrlOne { get; set; }
     
        [Required]
        [Display(Name = "Slider Image 2")]
        public string? SliderImageUrlTwo { get; set; }
     
        [Required]
        [Display(Name = "Slider Image 3")]
        public string? SliderImageUrlThree { get; set; }
     
        [Required]
        [Display(Name = "Slider Image 4")]
        public string? SliderImageUrlFour { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public int? Product_CollectionId { get; set; }
        [ForeignKey("Product_CollectionId")]
        [ValidateNever]
        public Product_Collection? Product_Collection { get; set; }

        [NotMapped]
        [InverseProperty("Product")]
        public ICollection<Product_Collection>? Product_CollectionsList { get; set; } // Navigation property

        [Required]
        [Display(Name = "Product Type")]
        public ProductCategory? ProductCategory { get; set; }

        [Display(Name = "Metal")]
        public ByMetal? ByMetal { get; set; }

    }

}
