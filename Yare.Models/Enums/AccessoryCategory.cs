using System.ComponentModel.DataAnnotations;

namespace Yare.Models.Enums
{
    public enum AccessoryCategory
    {
        [Display(Name = "Watch Strap")]
        WatchStraps = 0,
        [Display(Name = "Case & Box")]
        CasesAndBoxes,
        [Display(Name = "Wallet")]
        Wallets,
        [Display(Name = "Keyring")]
        Keyrings,
        [Display(Name = "Lighter")]
        Lighters,
        [Display(Name = "Pen")]
        Pens,
        [Display(Name = "Cleaning Product")]
        CleaningProducts
    }
}
