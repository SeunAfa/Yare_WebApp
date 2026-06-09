using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum ByGemstone
{
    Diamond = 0,
    Ruby,
    Sapphire,
    Emerald,
    Pearl,
    Amethyst,
    Topaz,
    Opal,
    [Display(Name = "No Gemstone")] None,
}
