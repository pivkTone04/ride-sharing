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
                .ToListAsync();
            return View(rides);
        }
    }
}
