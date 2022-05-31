#if cookies || role
namespace WebASP_MVC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
#if !cookies
        public int? RoleId { get; set; }
        public Role Role { get; set; } = null!;
#endif
    }
}
#endif
