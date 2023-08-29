using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace MessageProject.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("TEXT");
            });
            base.OnModelCreating(modelBuilder); // 確保調用基類的 OnModelCreating 方法

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => e.UserId); // 使用 Id 欄位作為主鍵
            });
        }

        public DbSet<User> Users { get; set; }

    }

}
