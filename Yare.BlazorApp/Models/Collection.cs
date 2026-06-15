namespace Yare.BlazorApp.Models;

public class Collection
{
    public int Id { get; set; }
    public string? CollectionName { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    /// <summary>Grouping for the admin collection picker: "Audience", "General", "Featured", or "Brand".</summary>
    public string? Group { get; set; }

    public List<int> ProductIds { get; set; } = new();
}
