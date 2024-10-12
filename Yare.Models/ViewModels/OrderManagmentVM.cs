using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.ViewModels
{
    public class OrderManagmentVM
    {
        public OrderHeader? OrderHeader { get; set; }

        public IEnumerable<OrderHeader>? OrderHeaderListStaff { get; set; }
        
        public IEnumerable<OrderHeader>? OrderHeaderListUser { get; set; }
        
        public IEnumerable<OrderDetail>? OrderDetail{ get; set; }

       
    }
}
