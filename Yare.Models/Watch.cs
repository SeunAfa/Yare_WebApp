using MediaBrowser.Model.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yare.Models.Enums;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Yare.Models
{
    public class Watch : Product
    {

        [Required]
        [Display(Name = "Brand")]
        public WatchBrand? WatchBrand { get; set; }

        [Required]
        [Display(Name = "Strap Type")]
        public WatchStrapType? WatchStrapType { get; set; }

        [Required]
        [Display(Name = "Watch Movement")]
        public WatchMovement? WatchMovement { get; set; }

        [Required]
        [Display(Name = "Water Resistant")]
        public WaterResistant? WaterResistant { get; set; }

        [Required]
        [Display(Name = "Dial Color")]
        public string? DialColor { get; set; }

        [Required]
        [Display(Name = "Diameter")]
        public WatchDiameter? WatchDiameter { get; set; }
     
        [Required]
        [Display(Name = "Case Shape")]
        public WatchCaseShape? WatchCaseShape { get; set; }


        [Display(Name = "Gemstone")]
        public ByGemstone? ByGemstone { get; set; }

        [Display(Name = "By Occassion")]
        public ByOccassion? ByOccassion { get; set; }

        [Display(Name = "Strap Colour")]
        public string? StrapColour { get; set; }

    }
}
