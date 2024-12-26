using RideSharing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RideSharing.Data
{
    public class RideSharingContext : IdentityDbContext<ApplicationUser>
    {
        public RideSharingContext(DbContextOptions<RideSharingContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<RideRequest> RideRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           modelBuilder.Entity<Vehicle>()
        .HasOne(v => v.Driver)
        .WithMany()
        .HasForeignKey(v => v.DriverId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ride>()
                .HasOne(r => r.Driver)
                .WithMany(u => u.Rides)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ride>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Rides)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RideRequest>()
                .HasOne(rr => rr.Ride)
                .WithMany(r => r.RideRequests)
                .HasForeignKey(rr => rr.RideId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RideRequest>()
                .HasOne(rr => rr.Passenger)
                .WithMany(u => u.RideRequests)
                .HasForeignKey(rr => rr.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
