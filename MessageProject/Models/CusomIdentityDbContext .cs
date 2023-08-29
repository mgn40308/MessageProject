using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MessageProject.Models
{
    public class CusomIdentityDbContext : IdentityDbContext<User>
    {

        public CusomIdentityDbContext(DbContextOptions<CusomIdentityDbContext> options)
        : base(options)
        { }
        public DbSet<User> Users { get; set; }
    }

}
