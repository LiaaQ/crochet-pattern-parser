using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CrochetPatternParser.Models;

namespace CrochetPatternParser.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity>
    {
        public DbSet<PatternEntity> Patterns => Set<PatternEntity>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
