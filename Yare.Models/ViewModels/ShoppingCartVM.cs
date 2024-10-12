using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Yare.Models.ViewModels
{
	public class ShoppingCartVM
	{
        public int? ShoppingCartId { get; set; }
        [ForeignKey("ShoppingCartId")]
        [ValidateNever]
        public ShoppingCart? shoppingCart { get; set; }

        public int Count { get; set; }

        public double OrderTotal { get; set; }

        [NotMapped]
        public double Price { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

        public IEnumerable<Product>? RelatedProducts { get; set; }

        public IEnumerable<ShoppingCart>? ShoppingCartList { get; set; }

		public OrderHeader? OrderHeader { get; set; }

	}
}
