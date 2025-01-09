using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;
using RideSharing.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using RideSharing.Services;

namespace RideSharing.Controllers
{
    [Authorize]
    public class RidesController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly GoogleMapsService _googleMapsService;

        public RidesController(RideSharingContext context, UserManager<ApplicationUser> userManager, IMapper mapper, GoogleMapsService googleMapsService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _googleMapsService = googleMapsService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var currentDateTime = DateTime.UtcNow;
            var userId = user.Id;
            var rides = await _context.Rides
                .Where(r => r.DriverId == userId)
                .Include(r => r.Vehicle)
                .ToListAsync();

            return View(rides);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var vehicles = await _context.Vehicles
                .Where(v => v.DriverId == userId)
                .ToListAsync();

            var model = new RideCreateViewModel();
            ViewBag.Vehicles = new SelectList(vehicles, "Id", "LicensePlate");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RideCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var vehicles = await _context.Vehicles
                    .Where(v => v.DriverId == userId)
                    .ToListAsync();

                ViewBag.Vehicles = new SelectList(vehicles, "Id", "LicensePlate", model.VehicleId);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var ride = _mapper.Map<Ride>(model);
            ride.DriverId = user.Id;
            ride.CreatedAt = DateTime.UtcNow;
            ride.UpdatedAt = DateTime.UtcNow;

            _context.Add(ride);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving ride: " + ex.Message);
                ModelState.AddModelError("", "An error occurred while saving the ride.");
                var vehicles = await _context.Vehicles
                    .Where(v => v.DriverId == user.Id)
                    .ToListAsync();
                ViewBag.Vehicles = new SelectList(vehicles, "Id", "LicensePlate", model.VehicleId);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides.FindAsync(id);
            if (ride == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<RideEditViewModel>(ride);

            var vehicles = await _context.Vehicles
                .Where(v => v.DriverId == ride.DriverId)
                .ToListAsync();

            ViewBag.Vehicles = new SelectList(vehicles, "Id", "LicensePlate", ride.VehicleId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RideEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var vehicles = await _context.Vehicles
                    .Where(v => v.DriverId == _userManager.GetUserId(User))
                    .ToListAsync();
                ViewBag.Vehicles = new SelectList(vehicles, "Id", "LicensePlate", model.VehicleId);
                return View(model);
            }

            var ride = await _context.Rides.FindAsync(id);
            if (ride == null)
            {
                return NotFound();
            }

            _mapper.Map(model, ride);
            ride.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Update(ride);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideExists(ride.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ride = await _context.Rides.FindAsync(id);
            if (ride != null)
            {
                _context.Rides.Remove(ride);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Vehicle)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ride == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id;

            RideRequest existingRequest = null;
            if (!string.IsNullOrEmpty(userId))
            {
                    existingRequest = await _context.RideRequests
                        .Where(rr => rr.RideId == id && rr.PassengerId == userId && rr.Status != "Deleted")
                        .OrderByDescending(rr => rr.RequestedAt)
                        .FirstOrDefaultAsync();
            }

            var model = new RideDetailsViewModel
            {
                Id = ride.Id,
                DriverEmail = ride.Driver.Email,
                DriverId = ride.DriverId,
                VehicleName = ride.Vehicle.Model,
                Origin = ride.Origin,
                Destination = ride.Destination,
                RideDateTime = ride.RideDateTime,
                AvailableSeats = ride.AvailableSeats,
                PricePerSeat = ride.PricePerSeat,
                PickupLocation = ride.PickupLocation,
                RideDescription = ride.RideDescription,
                ExistingRequestStatus = existingRequest?.Status
            };

            return View(model);
        }

        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.Id == id);
        }
        [HttpGet]
        [Route("api/maps/getRouteInfo")]
        public async Task<IActionResult> GetRouteInfo(string origin, string destination)
        {
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
             {
                return BadRequest(new { message = "Origin and destination are required." });
            }

            try
            {
                var googleMapsService = HttpContext.RequestServices.GetService(typeof(GoogleMapsService)) as GoogleMapsService;
                if (googleMapsService == null)
                {
                    return StatusCode(500, new { message = "Google Maps service is not available." });
                }

                var directions = await googleMapsService.GetDirectionsAsync(origin, destination);
                var rideData = googleMapsService.ExtractRideData(directions);

                return Ok(new 
                {
                    totalDistance = rideData.TotalDistance,
                    totalDuration = rideData.TotalDuration
                });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while fetching route data." });
            }
}

}
}
