using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum ByMetal
{
    Gold = 0,
    Silver,
    Platinum,
    [Display(Name = "Stainless Steel")] StainlessSteel,
    Titanium,
    [Display(Name = "Rose Gold")] RoseGold,
    [Display(Name = "White Gold")] WhiteGold,
    [Display(Name = "Yellow Gold")] YellowGold,
}
