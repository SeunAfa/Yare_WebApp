using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum JewelleryCategory
    {
        [Display(Name = "Anklet")]
        Anklets = 0,
        [Display(Name = "Bangle")]
        Bangles,
        [Display(Name = "Bracelet")]
        Bracelets,
        [Display(Name = "Brooch")]
        Brooches,
        [Display(Name = "Cufflink")]
        Cufflinks,
        [Display(Name = "Chain")]
        Chains,
        [Display(Name = "Charm")]
        Charms,
        [Display(Name = "Earring")]
        Earrings,
        [Display(Name = "Engagement Ring")]
        EngagementRings,
        [Display(Name = "Locket")]
        Lockets,
        [Display(Name = "Necklace")]
        Necklaces,
        [Display(Name = "Pendant")]
        Pendants,
        [Display(Name = "Ring")]
        Rings,
        [Display(Name = "Wedding Ring")]
        WeddingRings,
        [Display(Name = "Full Set")]
        FullSet

    }
}
