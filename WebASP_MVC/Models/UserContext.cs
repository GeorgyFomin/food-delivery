using Microsoft.EntityFrameworkCore;

namespace WebASP_MVC.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public UserContext(DbContextOptions<UserContext> options) : base(options) => Database.EnsureCreated();
    }
}
