using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Collection Name")]
        public string? CollectionName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

    }
}
