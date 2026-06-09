using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum WatchMovement
{
    [Display(Name = "Mechanical")] Mechanical = 0,
    [Display(Name = "Automatic")] Automatic = 1,
    [Display(Name = "Quartz")] Quartz = 2,
    [Display(Name = "Kinetic")] Kinetic = 3,
    [Display(Name = "Eco-Drive")] EcoDrive = 4,
    Manual = 5,
    Smart = 6,
    Solar = 8,
    [Display(Name = "Solar Powered")] SolarPowered = 9,
    [Display(Name = "Self Winding")] SelfWindingMechanic = 10,
}
