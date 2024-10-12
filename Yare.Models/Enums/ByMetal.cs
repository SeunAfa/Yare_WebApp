using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum ByMetal
    {
        Gold = 0,
        Silver,
        Platinum,
        [Display(Name = "Stainless Steel")]
        StainlessSteel,
        Titanium,
        [Display(Name = "Rose Gold")]
        RoseGold,
        [Display(Name = "White Gold")]
        WhiteGold,
        [Display(Name = "Yellow Gold")]
        YellowGold,
    }
}
