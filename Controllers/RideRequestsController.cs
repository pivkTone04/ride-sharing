using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RideRequestsController> _logger;

        public RideRequestsController(RideSharingContext context, UserManager<ApplicationUser> userManager, ILogger<RideRequestsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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
                .FirstOrDefaultAsync(rr => rr.Id == id);

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
                ModelState.AddModelError(string.Empty, "Ne morete poslati zahteve za svoj prevoz.");

                var modelError = new RideRequestCreateViewModel
                {
                    RideId = rideId,
                    Origin = ride.Origin,
                    Destination = ride.Destination
                };

                return View(modelError);
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var ride = await _context.Rides.FindAsync(model.RideId);
            if (ride == null)
            {
                ModelState.AddModelError(string.Empty, "Prevoz ni najden.");
                return View(model);
            }

            if (ride.DriverId == user.Id)
            {
                ModelState.AddModelError(string.Empty, "Ne morete poslati zahteve za svoj prevoz.");
                return View(model);
            }

            var existingRequest = await _context.RideRequests
                .Where(rr => rr.RideId == model.RideId && rr.PassengerId == user.Id)
                .OrderByDescending(rr => rr.RequestedAt)
                .FirstOrDefaultAsync();

            if (existingRequest != null && existingRequest.Status != "Deleted")
            {
                _logger.LogWarning("Uporabnik že ima obstoječo zahtevo.");
                ModelState.AddModelError(string.Empty, "Že imate obstoječo zahtevo za ta prevoz.");
                return View(model);
            }

            if (model.SeatsRequested > ride.AvailableSeats)
            {
                ModelState.AddModelError(string.Empty, "Ni dovolj prostih sedežev za vašo zahtevo.");
                return View(model);
            }

            var rideRequest = new RideRequest
            {
                Origin = model.Origin,
                Destination = model.Destination,
                SeatsRequested = model.SeatsRequested,
                MessageToDriver = model.MessageToDriver,
                RequestedAt = DateTime.Now,
                PassengerId = user.Id,
                RideId = model.RideId,
                Status = "Pending"
            };

            _context.RideRequests.Add(rideRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rideRequest == null)
            {
                return NotFound();
            }

            return View(rideRequest);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .FirstOrDefaultAsync(rr => rr.Id == id);

            if (rideRequest == null)
            {
                _logger.LogWarning($"RideRequest z ID {id} ni bil najden.");
                return NotFound();
            }

            if (rideRequest.Status == "Canceled" || rideRequest.Status == "Deleted")
            {
                _logger.LogWarning($"RideRequest z ID {id} je že bila preklicana.");
                ModelState.AddModelError(string.Empty, "Zahteva je že bila preklicana.");
                return View("Delete", rideRequest);
            }

            try
            {
                _context.RideRequests.Remove(rideRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Napaka pri brisanju RideRequest z ID {id}.");
                ModelState.AddModelError(string.Empty, "Napaka pri brisanju zahteve. Poskusite znova.");
                return View("Delete", rideRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .FirstOrDefaultAsync(rr => rr.Id == id);

            if (rideRequest == null)
            {
                return NotFound();
            }

            if (rideRequest.PassengerId != user.Id)
            {
                return Unauthorized();
            }

            if (rideRequest.Status == "Accepted")
            {
                rideRequest.Ride.AvailableSeats += rideRequest.SeatsRequested;
                _context.Update(rideRequest.Ride);
            }

            rideRequest.Status = "Deleted";

            _context.Update(rideRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "RideRequests");
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
                _logger.LogWarning($"RideRequest z ID {id} ni bil najden.");
                return NotFound();
            }

            if (rideRequest.Ride.DriverId != user.Id)
            {
                _logger.LogWarning($"Uporabnik ni lastnik vožnje za RideRequest z ID {id}.");
                return Unauthorized();
            }

            if (rideRequest.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Zahteva ni več v stanju 'Pending'.";
                return RedirectToAction("MyIncomingRequests");
            }

            if (rideRequest.SeatsRequested > rideRequest.Ride.AvailableSeats)
            {
                TempData["ErrorMessage"] = "Ni dovolj prostih sedežev za to zahtevo.";
                return RedirectToAction("MyIncomingRequests");
            }

            rideRequest.Status = "Accepted";
            rideRequest.Ride.AvailableSeats -= rideRequest.SeatsRequested;

            _context.Update(rideRequest);
            _context.Update(rideRequest.Ride);
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
                _logger.LogWarning($"RideRequest z ID {id} ni bil najden.");
                return NotFound();
            }

            if (rideRequest.Ride.DriverId != user.Id)
            {
                _logger.LogWarning($"Uporabnik ni lastnik vožnje za RideRequest z ID {id}.");
                return Unauthorized();
            }

            if (rideRequest.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Zahteva ni več v stanju 'Pending'.";
                return RedirectToAction("MyIncomingRequests");
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
