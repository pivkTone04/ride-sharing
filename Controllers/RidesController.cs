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

namespace RideSharing.Controllers
{
    [Authorize]
    public class RidesController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RidesController(RideSharingContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var currentDateTime = DateTime.Now;
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
            ride.CreatedAt = DateTime.Now;
            ride.UpdatedAt = DateTime.Now;

            _context.Add(ride);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Napaka pri shranjevanju vožnje: " + ex.Message);
                ModelState.AddModelError("", "Prišlo je do napake pri shranjevanju vožnje.");
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
            ride.UpdatedAt = DateTime.Now;

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

    var model = new RideDetailsViewModel
    {
        Id = ride.Id,
        DriverEmail = ride.Driver.Email,
        VehicleName = ride.Vehicle.Model,
        Origin = ride.Origin,
        Destination = ride.Destination,
        RideDateTime = ride.RideDateTime,
        AvailableSeats = ride.AvailableSeats,
        PricePerSeat = ride.PricePerSeat,
        PickupLocation = ride.PickupLocation,
        RideDescription = ride.RideDescription
    };

    return View(model);
}


        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.Id == id);
        }
    }
}
