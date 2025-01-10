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
                ModelState.AddModelError(string.Empty, "You cannot send a request for your own ride.");

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
                ModelState.AddModelError(string.Empty, "Ride not found.");
                return View(model);
            }

            if (ride.DriverId == user.Id)
            {
                ModelState.AddModelError(string.Empty, "You cannot send a request for your own ride.");
                return View(model);
            }
            
            if (model.SeatsRequested > ride.AvailableSeats)
            {
                ModelState.AddModelError("SeatsRequested", "You cannot request more seats than available.");
                return View(model);
            }

            var existingRequest = await _context.RideRequests
                .Where(rr => rr.RideId == model.RideId && rr.PassengerId == user.Id)
                .OrderByDescending(rr => rr.RequestedAt)
                .FirstOrDefaultAsync();

            if (existingRequest != null && existingRequest.Status != "Deleted")
            {
                string statusMessage = $"Your existing request is currently '{existingRequest.Status}'.";
                ModelState.AddModelError(string.Empty, "You already have an existing request for this ride.");
                return View(model);
            }

            if (model.SeatsRequested > ride.AvailableSeats)
            {
                ModelState.AddModelError(string.Empty, "Not enough available seats for your request.");
                return View(model);
            }

            var rideRequest = new RideRequest
            {
                Origin = model.Origin,
                Destination = model.Destination,
                SeatsRequested = model.SeatsRequested,
                MessageToDriver = model.MessageToDriver,
                RequestedAt = DateTime.UtcNow,
                PassengerId = user.Id,
                RideId = model.RideId,
                Status = "Pending"
            };

            _context.RideRequests.Add(rideRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
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
                _logger.LogWarning($"RideRequest with ID {id} was not found.");
                return NotFound();
            }

            if (rideRequest.Ride.DriverId != user.Id)
            {
                _logger.LogWarning($"User is not the owner of the ride for RideRequest with ID {id}.");
                return Unauthorized();
            }

            if (rideRequest.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Request is no longer in 'Pending' state.";
                return RedirectToAction("MyIncomingRequests");
            }

            if (rideRequest.SeatsRequested > rideRequest.Ride.AvailableSeats)
            {
                TempData["ErrorMessage"] = "Not enough available seats for this request.";
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
                _logger.LogWarning($"RideRequest with ID {id} was not found.");
                return NotFound();
            }

            if (rideRequest.Ride.DriverId != user.Id)
            {
                _logger.LogWarning($"User is not the owner of the ride for RideRequest with ID {id}.");
                return Unauthorized();
            }

            if (rideRequest.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Request is no longer in 'Pending' state.";
                return RedirectToAction("MyIncomingRequests");
            }

            rideRequest.Status = "Rejected";

            _context.Update(rideRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyIncomingRequests");
        }
        public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var rideRequest = await _context.RideRequests
        .Include(rr => rr.Ride)
        .FirstOrDefaultAsync(rr => rr.Id == id);
    
    if (rideRequest == null)
    {
        return NotFound();
    }

    return View(rideRequest);
}

    }
}
