using HajurkoCarRental.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Models;

namespace HajurkoCarRental.Areas.Identity.Data;

public class HajurkoCarRentalDataContext : IdentityDbContext<User>
{
    public HajurkoCarRentalDataContext(DbContextOptions<HajurkoCarRentalDataContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Identity");
        builder.Entity<User>(entity =>
        {
            entity.ToTable(name: "User");
        });
        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Role");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

    }

    public DbSet<Offer>? Offer { get; set; }

    public DbSet<Car>? Car { get; set; }


    public DbSet<RentalRequest>? RentalRequest { get; set; }

    public DbSet<CarReturn>? CarReturn { get; set; }

    public DbSet<Damage>? Damage { get; set; }
}
