using MediaBrowser.Model.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class Jewellery : Product
    {

        [Display(Name = "Jewellery Category")]
        public JewelleryCategory? JewelleryCategory { get; set; }

        [Display(Name = "Gemstone")]
        public ByGemstone? ByGemstone { get; set; }

        [Display(Name = "By Occassion")]
        public ByOccassion? ByOccassion { get; set; }

    }
}
