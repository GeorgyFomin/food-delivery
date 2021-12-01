using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebASP_MVC.Models;

namespace WebASP_MVC.Data
{
    public class WebASP_MVCContext : DbContext
    {
        public WebASP_MVCContext (DbContextOptions<WebASP_MVCContext> options)
            : base(options)
        {
        }

        public DbSet<WebASP_MVC.Models.Delivery> Delivery { get; set; }
    }
}
