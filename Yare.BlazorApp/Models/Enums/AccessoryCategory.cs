using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum AccessoryCategory
{
    [Display(Name = "Watch Strap")] WatchStraps = 0,
    [Display(Name = "Case & Box")] CasesAndBoxes,
    Wallets,
    Keyrings,
    Lighters,
    Pens,
    [Display(Name = "Cleaning Product")] CleaningProducts,
}
