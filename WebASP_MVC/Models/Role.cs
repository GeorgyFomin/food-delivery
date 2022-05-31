#if role
namespace WebASP_MVC.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Noname";
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
#endif
