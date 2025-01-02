using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;
using RideSharing.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RideSharing.Controllers
{
    [Authorize]
    public class RideRequestsController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RideRequestsController(RideSharingContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var rideRequests = await _context.RideRequests
                .Include(rr => rr.Ride)
                    .ThenInclude(r => r.Vehicle)
                .Where(rr => rr.PassengerId == user.Id)
                .ToListAsync();

            return View(rideRequests);
        }

        public async Task<IActionResult> MyIncomingRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var incomingRequests = await _context.RideRequests
                .Include(rr => rr.Passenger)
                .Include(rr => rr.Ride)
                .Where(rr => rr.Ride.DriverId == user.Id)
                .ToListAsync();

            return View(incomingRequests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Passenger)
                .Include(rr => rr.Ride)
                    .ThenInclude(r => r.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rideRequest == null)
            {
                return NotFound();
            }

            return View(rideRequest);
        }

        public async Task<IActionResult> Create(int rideId)
        {
            var ride = await _context.Rides.FindAsync(rideId);
            if (ride == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (ride.DriverId == user.Id)
            {
                return BadRequest("Ne morete poslati zahteve za svoj prevoz.");
            }

            var model = new RideRequestCreateViewModel
            {
                RideId = rideId,
                Origin = ride.Origin,
                Destination = ride.Destination
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RideRequestCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var ride = await _context.Rides.FindAsync(model.RideId);
                if (ride == null)
                {
                    return NotFound();
                }

                if (ride.DriverId == user.Id)
                {
                    return BadRequest("Ne morete poslati zahteve za svoj prevoz.");
                }

                var existingRequest = await _context.RideRequests
                    .FirstOrDefaultAsync(rr => rr.RideId == model.RideId && rr.PassengerId == user.Id);
                if (existingRequest != null)
                {
                    ModelState.AddModelError("", "Že imate obstoječo zahtevo za ta prevoz.");
                    return View(model);
                }

                var rideRequest = new RideRequest
                {
                    Origin = model.Origin,
                    Destination = model.Destination,
                    RequestedAt = DateTime.UtcNow,
                    PassengerId = user.Id,
                    RideId = model.RideId,
                    Status = "Pending"
                };

                _context.RideRequests.Add(rideRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .FirstOrDefaultAsync(rr => rr.Id == id);

            if (rideRequest == null)
            {
                return NotFound();
            }

            if (rideRequest.Ride.DriverId != user.Id)
            {
                return Unauthorized();
            }

            if (rideRequest.Status != "Pending")
            {
                return BadRequest("Zahteva ni več v stanju 'Pending'.");
            }

            rideRequest.Status = "Accepted";

            _context.Update(rideRequest);
            await _context.SaveChangesAsync();


            return RedirectToAction("MyIncomingRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .FirstOrDefaultAsync(rr => rr.Id == id);

            if (rideRequest == null)
            {
                return NotFound();
            }

            if (rideRequest.Ride.DriverId != user.Id)
            {
                return Unauthorized();
            }

            if (rideRequest.Status != "Pending")
            {
                return BadRequest("Zahteva ni več v stanju 'Pending'.");
            }

            rideRequest.Status = "Rejected";

            _context.Update(rideRequest);
            await _context.SaveChangesAsync();


            return RedirectToAction("MyIncomingRequests");
        }


        private bool RideRequestExists(int id)
        {
            return _context.RideRequests.Any(e => e.Id == id);
        }
    }
}
