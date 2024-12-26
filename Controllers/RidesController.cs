
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;

namespace RideSharing.Controllers
{
    [Authorize]
    public class RidesController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RidesController(RideSharingContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var rides = _context.Rides.Include(r => r.Driver).Include(r => r.Vehicle);
            return View(await rides.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ride == null)
            {
                return NotFound();
            }

            return View(ride);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var vehicles = _context.Vehicles.Where(v => v.DriverId == user.Id).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles, "Id", "Make");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Origin,Destination,DateTime,VehicleId")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                ride.DriverId = user.Id;
                _context.Add(ride);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var vehicles = _context.Vehicles.Where(v => v.DriverId == _userManager.GetUserId(User)).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles, "Id", "Make", ride.VehicleId);
            return View(ride);
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
            var vehicles = _context.Vehicles.Where(v => v.DriverId == _userManager.GetUserId(User)).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles, "Id", "Make", ride.VehicleId);
            return View(ride);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Origin,Destination,DateTime,VehicleId")] Ride ride)
        {
            if (id != ride.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRide = await _context.Rides.FindAsync(id);
                    if (existingRide == null)
                    {
                        return NotFound();
                    }

                    existingRide.Origin = ride.Origin;
                    existingRide.Destination = ride.Destination;
                    existingRide.DateTime = ride.DateTime;
                    existingRide.VehicleId = ride.VehicleId;

                    _context.Update(existingRide);
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
            var vehicles = _context.Vehicles.Where(v => v.DriverId == _userManager.GetUserId(User)).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles, "Id", "Make", ride.VehicleId);
            return View(ride);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ride = await _context.Rides
                .Include(r => r.Driver)
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

        private bool RideExists(int id)
        {
            return _context.Rides.Any(e => e.Id == id);
        }
    }
}
