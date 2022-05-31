using Microsoft.AspNetCore.Identity;

namespace Entities.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Alias { get; set; } = null!;
    }
}
