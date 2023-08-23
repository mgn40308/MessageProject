using Microsoft.EntityFrameworkCore;

namespace MessageProject.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reply> Replys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = WebApplication.CreateBuilder();
            optionsBuilder.UseMySql(builder.Configuration.GetConnectionString("MySQL"),
                                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQL")));
        }
    }
}
