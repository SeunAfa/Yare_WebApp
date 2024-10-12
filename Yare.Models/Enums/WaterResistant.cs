using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum WaterResistant
    {
        [Display(Name = "30 Meters")]
        Thirty = 0,
        [Display(Name = "50 Meters")]
        Fifty,
        [Display(Name = "100 Meters")]
        Onehundred,
        [Display(Name = "200 Meters")]
        Twohundred,
        [Display(Name = "300 Meters")]
        Threehundred
    }
}
