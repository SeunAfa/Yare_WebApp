using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Yare.BlazorApp.Models;

public static class EnumExtensions
{
    /// <summary>
    /// Returns the [Display(Name="...")] value if present, otherwise the enum's name.
    /// Use this to render enums for users (e.g. WatchDiameter.mm42 -> "42mm").
    /// </summary>
    public static string DisplayName(this Enum value)
    {
        if (value is null) return string.Empty;
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var attr   = member?.GetCustomAttribute<DisplayAttribute>();
        return attr?.GetName() ?? value.ToString();
    }
}
