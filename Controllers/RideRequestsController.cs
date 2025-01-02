using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;
using RideSharing.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace RideSharing.Controllers
{
    [Authorize]
    public class RideRequestsController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RideRequestsController(RideSharingContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
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

            var rideRequests = await _context.RideRequests
                .Where(rr => rr.PassengerId == user.Id)
                .Include(rr => rr.Ride)
                .ThenInclude(r => r.Vehicle)
                .ToListAsync();

            return View(rideRequests);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var availableRides = await _context.Rides
                .Where(r => r.DriverId != userId)
                .ToListAsync();

            ViewBag.Rides = new SelectList(availableRides, "Id", "Origin");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RideRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var availableRides = await _context.Rides
                    .Where(r => r.DriverId != userId)
                    .ToListAsync();
                ViewBag.Rides = new SelectList(availableRides, "Id", "Origin", model.RideId);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("Uporabnik ni prijavljen.");
                return Unauthorized();
            }

            var rideRequest = _mapper.Map<RideRequest>(model);
            rideRequest.PassengerId = user.Id;
            rideRequest.RequestedAt = DateTime.UtcNow;

            _context.Add(rideRequest);

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Zahteva za vožnjo uspešno shranjena.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Napaka pri shranjevanju zahteve za vožnjo: " + ex.Message);
                ModelState.AddModelError("", "Prišlo je do napake pri shranjevanju zahteve za vožnjo.");
                var availableRides = await _context.Rides
                    .Where(r => r.DriverId != user.Id)
                    .ToListAsync();
                ViewBag.Rides = new SelectList(availableRides, "Id", "Origin", model.RideId);
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

            var rideRequest = await _context.RideRequests.FindAsync(id);
            if (rideRequest == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<RideRequestEditViewModel>(rideRequest);

            var availableRides = await _context.Rides
                .Where(r => r.DriverId != _userManager.GetUserId(User))
                .ToListAsync();
            ViewBag.Rides = new SelectList(availableRides, "Id", "Origin", rideRequest.RideId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RideRequestEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var availableRides = await _context.Rides
                    .Where(r => r.DriverId != _userManager.GetUserId(User))
                    .ToListAsync();
                ViewBag.Rides = new SelectList(availableRides, "Id", "Origin", model.RideId);
                return View(model);
            }

            var rideRequest = await _context.RideRequests.FindAsync(id);
            if (rideRequest == null)
            {
                return NotFound();
            }

            _mapper.Map(model, rideRequest);
            rideRequest.RequestedAt = DateTime.UtcNow;

            try
            {
                _context.Update(rideRequest);
                await _context.SaveChangesAsync();
                Console.WriteLine("Zahteva za vožnjo uspešno posodobljena.");
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
