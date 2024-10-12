using System.ComponentModel.DataAnnotations;

namespace Yare.Models.Enums
{
    public enum Gender
    {
        [Display(Name = "Mens")]
        Male = 0,
        [Display(Name = "Ladies")]
        Female,
        Unisex,
    }
}
