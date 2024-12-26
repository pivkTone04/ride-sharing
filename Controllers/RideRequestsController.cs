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
            var rideRequests = _context.RideRequests
                .Include(rr => rr.Ride)
                .Include(rr => rr.Passenger)
                .Where(rr => rr.PassengerId == user.Id || rr.Ride.DriverId == user.Id);
            return View(await rideRequests.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .Include(rr => rr.Passenger)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rideRequest == null)
            {
                return NotFound();
            }

            return View(rideRequest);
        }

        public IActionResult Create()
        {
            ViewData["RideId"] = new SelectList(_context.Rides, "Id", "Origin");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RideId")] RideRequest rideRequest)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                rideRequest.PassengerId = user.Id;
                rideRequest.RequestedAt = DateTime.UtcNow;
                _context.Add(rideRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RideId"] = new SelectList(_context.Rides, "Id", "Origin", rideRequest.RideId);
            return View(rideRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rideRequest = await _context.RideRequests.FindAsync(id);
            if (rideRequest == null)
            {
                return NotFound();
            }
            ViewData["RideId"] = new SelectList(_context.Rides, "Id", "Origin", rideRequest.RideId);
            return View(rideRequest);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RideId,Status")] RideRequest rideRequest)
        {
            if (id != rideRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRequest = await _context.RideRequests.FindAsync(id);
                    if (existingRequest == null)
                    {
                        return NotFound();
                    }

                    existingRequest.Status = rideRequest.Status;
                    _context.Update(existingRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RideRequestExists(rideRequest.Id))
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
            ViewData["RideId"] = new SelectList(_context.Rides, "Id", "Origin", rideRequest.RideId);
            return View(rideRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rideRequest = await _context.RideRequests
                .Include(rr => rr.Ride)
                .Include(rr => rr.Passenger)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rideRequest == null)
            {
                return NotFound();
            }

            return View(rideRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rideRequest = await _context.RideRequests.FindAsync(id);
            if (rideRequest != null)
            {
                _context.RideRequests.Remove(rideRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RideRequestExists(int id)
        {
            return _context.RideRequests.Any(e => e.Id == id);
        }
    }
}
