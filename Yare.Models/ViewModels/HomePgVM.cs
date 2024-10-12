using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.ViewModels;

public class HomePgVM
{
    public ShoppingCartVM? ShoppingCartVM { get; set; }

    public ProductVM? ProductVM { get; set; }

    public IEnumerable<ShoppingCart>? ShoppingCartList { get; set; }

    public IEnumerable<Product>? SearchBestSellingProducts { get; set; }

    public IEnumerable<OrderDetail>? BestSellersList { get; set; }

    public OrderDetail? OrderDetail { get; set; }

    public OrderHeader? OrderHeader { get; set; }

}
