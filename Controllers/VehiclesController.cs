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
using AutoMapper;

namespace RideSharing.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public VehiclesController(RideSharingContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
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

            var userId = user.Id;
            var vehicles = await _context.Vehicles
                .Where(v => v.DriverId == userId)
                .ToListAsync();

            return View(vehicles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            try
            {
                var model = new VehicleCreateViewModel
                {
                    Make = form["Make"],
                    Model = form["Model"],
                    Year = int.TryParse(form["Year"], out var year) ? year : 0,
                    LicensePlate = form["LicensePlate"],
                    Color = form["Color"],
                    Capacity = int.TryParse(form["Capacity"], out var capacity) ? capacity : 0
                };

                if (!TryValidateModel(model))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    Console.WriteLine("Errors in ModelState: " + string.Join(", ", errors));
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    Console.WriteLine("User is not logged in.");
                    return Unauthorized();
                }

                var vehicle = new Vehicle
                {
                    Make = model.Make,
                    Model = model.Model,
                    Year = model.Year,
                    LicensePlate = model.LicensePlate,
                    Color = model.Color,
                    Capacity = model.Capacity,
                    DriverId = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                Console.WriteLine("Vehicle data for saving:");
                Console.WriteLine($"DriverId: {vehicle.DriverId}, Make: {vehicle.Make}, Model: {vehicle.Model}, Year: {vehicle.Year}");

                _context.Vehicles.Add(vehicle);

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Vehicle successfully saved.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving vehicle: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the vehicle.");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(new VehicleCreateViewModel());
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<VehicleEditViewModel>(vehicle);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _mapper.Map(model, vehicle);
            vehicle.UpdatedAt = DateTime.Now;

            try
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                Console.WriteLine("Vehicle successfully updated.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(vehicle.Id))
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

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }

        [HttpGet]
        public JsonResult GetMaxSeats(int vehicleId)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle != null)
            {
                return Json(new { maxSeats = vehicle.Capacity });
            }
            return Json(new { maxSeats = 1 });
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

             var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

    }
}
