
namespace RideSharing.Data;
using RideSharing.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class RideSharingContext : IdentityDbContext<ApplicationUser>
{

public RideSharingContext(DbContextOptions<RideSharingContext> options) : base(options)
{

}

public DbSet<User> Users { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
modelBuilder.Entity<User>().ToTable("User");
base.OnModelCreating(modelBuilder);
}

}
