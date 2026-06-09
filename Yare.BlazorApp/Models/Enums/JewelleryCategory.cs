using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum JewelleryCategory
{
    Anklets = 0,
    Bangles,
    Bracelets,
    Brooches,
    Cufflinks,
    Chains,
    Charms,
    Earrings,
    [Display(Name = "Engagement Ring")] EngagementRings,
    Lockets,
    Necklaces,
    Pendants,
    Rings,
    [Display(Name = "Wedding Ring")] WeddingRings,
    [Display(Name = "Full Set")] FullSet,
}
