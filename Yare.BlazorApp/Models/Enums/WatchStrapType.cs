using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum WatchStrapType
{
    Leather = 0,
    Metal,
    Rubber,
    Silicone,
    Nylon,
    Mesh,
    [Display(Name = "Stainless Steel")] StainlessSteel,
    [Display(Name = "Ceramic Bracelet")] CeramicBracelet,
    [Display(Name = "PU Leather")] PULeather,
    [Display(Name = "Carbon Fiber")] CarbonFiber,
    Titanium,
    Gold,
    Platinum,
    [Display(Name = "White Gold")] WhiteGold,
    [Display(Name = "Rose Gold")] RoseGold,
    [Display(Name = "Crocodile Skin")] CrocodileSkin,
    [Display(Name = "Alligator Skin")] AlligatorSkin,
    Suede,
}
