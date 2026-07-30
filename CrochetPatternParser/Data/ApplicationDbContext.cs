using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CrochetPatternParser.Models;

namespace CrochetPatternParser.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUserEntity, IdentityRole, string>
    {
        public DbSet<PatternEntity> Patterns => Set<PatternEntity>();
        public DbSet<SectionEntity> Sections => Set<SectionEntity>();
        public DbSet<RoundEntity> Rounds => Set<RoundEntity>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(r => r.Id).HasMaxLength(450);
                entity.Property(r => r.Name).HasMaxLength(256);
                entity.Property(r => r.NormalizedName).HasMaxLength(256);
            });

            builder.Entity<ApplicationUserEntity>(entity =>
            {
                entity.Property(u => u.Id).HasMaxLength(450);
                entity.Property(u => u.UserName).HasMaxLength(256);
                entity.Property(u => u.NormalizedUserName).HasMaxLength(256);
                entity.Property(u => u.Email).HasMaxLength(256);
                entity.Property(u => u.NormalizedEmail).HasMaxLength(256);
            });
        }
    }
}
