using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum ByGemstone
    {
        Diamond = 0,
        Ruby,
        Sapphire,
        Emerald,
        Amethyst,
        Aquamarine,
        Topaz,
        Garnet,
        Peridot,
        Opal,
        Tanzanite,
        Tourmaline,
        Morganite,
        Jade,
        [Display(Name = "Lapis Lazuli")]
        LapisLazuli,
        Turquoise,
        Pearl,
        [Display(Name = "Lab Grown Diamonds")]
        LabGrownDiamonds,
    }
}
