using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum Gender
{
    [Display(Name = "Mens")]
    Male = 0,
    [Display(Name = "Ladies")]
    Female,
    Unisex,
}
