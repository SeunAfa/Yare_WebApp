using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.ViewModels
{
    public class CollectionVM
    {

        public int Id { get; set; }

        public int? CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        [ValidateNever]
        public Collection? Collection { get; set; }

        [Display(Name = "Collection")]
        public string? CollectionName { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

    }
}
