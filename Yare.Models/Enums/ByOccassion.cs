using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum ByOccassion
    {
        [Display(Name = "Bridal Jewellery")]
        BridalJewellery = 0,
        Anniversary,
        Birthday,
        [Display(Name = "Engagement Rings")]
        EngagementRings,
        [Display(Name = "Wedding Rings")]
        WeddingRings,
        ValentiesDay,
        Graduation

    }
}
