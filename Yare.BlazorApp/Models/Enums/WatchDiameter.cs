using System.ComponentModel.DataAnnotations;

namespace Yare.BlazorApp.Models.Enums;

public enum WatchDiameter
{
    [Display(Name = "28mm")] mm28 = 0,
    [Display(Name = "30mm")] mm30,
    [Display(Name = "32mm")] mm32,
    [Display(Name = "34mm")] mm34,
    [Display(Name = "36mm")] mm36,
    [Display(Name = "38mm")] mm38,
    [Display(Name = "40mm")] mm40,
    [Display(Name = "41mm")] mm41,
    [Display(Name = "42mm")] mm42,
    [Display(Name = "44mm")] mm44,
    [Display(Name = "45mm")] mm45,
    [Display(Name = "46mm")] mm46,
}
