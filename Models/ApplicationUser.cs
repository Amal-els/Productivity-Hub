using Microsoft.AspNetCore.Identity;
namespace TeamProject.Models;
public class ApplicationUser : IdentityUser
{
    public required string DisplayName { get; set; }
    public string? EmailPassword { get; set; }
}
