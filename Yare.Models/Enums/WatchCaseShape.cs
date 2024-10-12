using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Models.Enums
{
    public enum WatchCaseShape
    {
        Round,
        Square,
        Rectangular,
        [Display(Name = "Tonneau (Barrel-shaped)")]
        Tonneau,
        [Display(Name = "Cushion (Pillow-shaped)")]
        Cushion,
        Oval,
        Hexagonal,
        Octagonal,
        Asymmetrical,
        [Display(Name = "Tonbogiri (Sword-shaped)")]
        Tonbogiri,
        Trapezoid,
        Pentagon,
        Ellipse,
        Cylinder,
        [Display(Name = "Dodecagon (12-sided)")]
        Dodecagon,
        [Display(Name = "Heptagon (7-sided)")]
        Heptagon,
        [Display(Name = "Skeleton (Transparent)")]
        Skeleton,
        [Display(Name = "Tourbillon (Circular cage)")]
        Tourbillon,
        [Display(Name = "Square with rounded edges (TV-shaped)")]
        SquareWithRoundedEdges,
        [Display(Name = "Stadium (Half-round)")]
        Stadium,
        [Display(Name = "Anima (Animal-inspired)")]
        Anima,
    }
}
