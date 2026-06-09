namespace Yare.BlazorApp.Models;

public class AppUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? StreetAdress { get; set; }
    public string? City { get; set; }
    public string? Borough { get; set; }
    public string? PostCode { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string FullName => $"{FirstName} {LastName}";
}
