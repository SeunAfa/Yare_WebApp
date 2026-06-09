using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models;

public class OrderHeader
{
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ShippingDate { get; set; }
    public double OrderTotal { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Phone")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Address")]
    public string? StreetAdress { get; set; }

    public string? City { get; set; }
    public string? Borough { get; set; }

    [Display(Name = "Post Code")]
    public string? PostCode { get; set; }
}
