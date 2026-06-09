using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum WaterResistant
{
    [Display(Name = "30m / 3ATM")] Atm3 = 0,
    [Display(Name = "50m / 5ATM")] Atm5,
    [Display(Name = "100m / 10ATM")] Atm10,
    [Display(Name = "200m / 20ATM")] Atm20,
    [Display(Name = "300m / 30ATM")] Atm30,
    [Display(Name = "Not Water Resistant")] None,
}
