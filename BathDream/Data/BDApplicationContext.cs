using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Data
{
    public class BDApplicationaContext : IdentityDbContext<User>
    {
        public DbSet<UserCustomer> UserCustomers { get; set; }
        public DbSet<UserExecutor> UserExecutors { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }

        public BDApplicationaContext(DbContextOptions<BDApplicationaContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
