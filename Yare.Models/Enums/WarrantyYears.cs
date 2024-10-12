using Nest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yare.Models.Enums
{
    public enum WarrantyYears
    {
        [Display(Name = "1 Year")]
        OneYear = 0,
        [Display(Name = "2 Years")]
        TwoYears,
        [Display(Name = "3 Years")]
        ThreeYears,
        [Display(Name = "4 Years")]
        FourYears,
        [Display(Name = "5 Years")]
        FiveYears,
        [Display(Name = "6 Years")]
        SixYears,
        [Display(Name = "7 Years")]
        SevenYears,
        [Display(Name = "8 Years")]
        EightYears,
        [Display(Name = "9 Years")]
        NineYears,
        [Display(Name = "10 Years")]
        TenYears,
        [Display(Name = "11 Years")]
        ElevenYears,
        [Display(Name = "12 Years")]
        TweleveYears
    }
}
