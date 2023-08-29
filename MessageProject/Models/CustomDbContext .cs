using Microsoft.EntityFrameworkCore;

namespace MessageProject.Models
{
    public class CustomDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<User> Users { get; set; }
    }
}
