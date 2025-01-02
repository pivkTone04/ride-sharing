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
            Console.WriteLine("Napake v ModelState: " + string.Join(", ", errors));
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Console.WriteLine("Uporabnik ni prijavljen.");
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Console.WriteLine("Podatki vozila za shranjevanje:");
        Console.WriteLine($"DriverId: {vehicle.DriverId}, Make: {vehicle.Make}, Model: {vehicle.Model}, Year: {vehicle.Year}");

        _context.Vehicles.Add(vehicle);

        try
        {
            await _context.SaveChangesAsync();
            Console.WriteLine("Vozilo uspešno shranjeno.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Napaka pri shranjevanju vozila: {ex.Message}");
            ModelState.AddModelError("", "Prišlo je do napake pri shranjevanju vozila.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Napaka pri obdelavi zahteve: {ex.Message}");
        ModelState.AddModelError("", "Prišlo je do nepričakovane napake.");
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
            vehicle.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                Console.WriteLine("Vozilo uspešno posodobljeno.");
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
    }
}
