using System.ComponentModel.DataAnnotations;

namespace Yare.Models.Enums
{
    public enum Carrier
    {
        UPS = 0,
        [Display(Name = "Parcel Force")]
        ParcelForce,
        DPD,
        DHL,
        Hermes,
        Yodel,
        FedEx
    }
}
