using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum ByOccassion
{
    Everyday = 0,
    Formal,
    Sport,
    Wedding,
    [Display(Name = "Special Occasion")] SpecialOccasion,
    Casual,
    Business,
}
