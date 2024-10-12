using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum WatchMovement
    {
        [Display(Name = "Mechanical")]
        Mechanical = 0,
        [Display(Name = "Automatic")]
        Automatic = 1,
        [Display(Name = "Quartz")]
        Quartz = 2,
        [Display(Name = "Kinetic")]
        Kinetic = 3,
        [Display(Name = "Eco-Drive")]
        EcoDrive = 4,
        Manual = 5,
        Smart = 6,
        Analog = 7, 
        Solar = 8,
        [Display(Name = "Solar Powered")]
        SolarPowered = 9,
        [Display(Name = "Self Winding Mechanic")]
        SelfWindingMechanic = 10
    }
}



                            