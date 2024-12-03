namespace RideSharing.Data;
using RideSharing.Models;

using Microsoft.EntityFrameworkCore;

public class RideSharingContext : DbContext
{

public RideSharingContext(DbContextOptions<RideSharingContext> options) : base(options)
{

}

public DbSet<User> Users { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
modelBuilder.Entity<User>().ToTable("User");
}

}
