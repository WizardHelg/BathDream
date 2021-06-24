using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BathDream.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BathDream.Data
{
    public class DBApplicationaContext : IdentityDbContext<User>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ExecutorProfile> ExecutorProfiles { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<WorkPrice> WorkPrices { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialPrice> MaterialPrices { get; set; }
        public DbSet<AdditionalWork> AdditionalWorks { get; set; }
        public DbSet<Class> Class { get; set; }
        public DBApplicationaContext(DbContextOptions<DBApplicationaContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
