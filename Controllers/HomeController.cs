using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;
using RideSharing.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RideSharing.Controllers
{
    public class HomeController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(RideSharingContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id;
            var currentDateTime = DateTime.Now;
            var rides = await _context.Rides
                .Include(r => r.Vehicle)
                .Include(r => r.Driver)
                .Where(r => r.AvailableSeats > 0 && r.RideDateTime >= currentDateTime)
                .ToListAsync();

            var existingRequests = await _context.RideRequests
                .Where(rr => rr.PassengerId == userId && (rr.Status == "Pending" || rr.Status == "Accepted"))
                .Select(rr => rr.RideId)
                .ToListAsync();

            var rideViewModels = rides.Select(r => new RideViewModel
            {
                Id = r.Id,
                DriverEmail = r.Driver?.Email,
                VehicleName = r.Vehicle != null ? $"{r.Vehicle.Make} {r.Vehicle.Model}" : "Ni vozila",
                Origin = r.Origin,
                Destination = r.Destination,
                RideDateTime = r.RideDateTime,
                AvailableSeats = r.AvailableSeats,
                CanRequest = r.DriverId != userId && !existingRequests.Contains(r.Id) && r.AvailableSeats > 0,
                IsDriver = r.DriverId == userId
            }).ToList();

            return View(rideViewModels);
        }
    }
}
