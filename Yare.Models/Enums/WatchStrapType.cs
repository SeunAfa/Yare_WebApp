using Nest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yare.Models.Enums
{
    public enum WatchStrapType
    {
        Biosourced = 0,
        [Display(Name = "Ceramic Bracelet")]
        CeramicBracelet,
        EcoPlastic,
        [Display(Name = "Expandable Bracelet")]
        ExpandableBracelet,
        [Display(Name = "Fabric (Canvas, Denim, etc.)")]
        Fabric,
        Leather,
        Mesh,
        Metal,
        [Display(Name = "PU Leather")]
        PULeather,
        [Display(Name = "Plastic Resin")]
        PlasticResin,
        Rubber,
        Silicone,
        Cuff,
        [Display(Name = "Stainless Steel")]
        StainlessSteel,
        Nylon,
        [Display(Name = "Crocodile Skin")]
        CrocodileSkin,
        [Display(Name = "Alligator Skin")]
        AlligatorSkin,
        [Display(Name = "Ostrich Skin")]
        OstrichSkin,
        [Display(Name = "Shark Skin")]
        SharkSkin,
        [Display(Name = "Snake Skin")]
        SnakeSkin,
        Suede,
        [Display(Name = "Carbon Fiber")]
        CarbonFiber,
        Titanium,
        [Display(Name = "Gold")]
        Gold,
        Platinum,
        [Display(Name = "Silver")]
        Silver,
        [Display(Name = "White Gold")]
        WhiteGold,
        [Display(Name = "Rose Gold")]
        RoseGold
    }
}
