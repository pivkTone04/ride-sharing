using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;
using System.Threading.Tasks;

namespace RideSharing.Controllers
{
    public class HomeController : Controller
    {
        private readonly RideSharingContext _context;

        public HomeController(RideSharingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rides = await _context.Rides
                .Include(r => r.Driver)
                .Include(r => r.Vehicle)
                .Select(r => new Ride
                {
                    Id = r.Id,
                    Driver = r.Driver,
                    Vehicle = r.Vehicle,
                    Origin = r.Origin,
                    Destination = r.Destination,
                    RideDateTime = r.RideDateTime,
                    AvailableSeats = r.AvailableSeats
                })
                .ToListAsync();

            return View(rides);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Driver)
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }
    }
}
