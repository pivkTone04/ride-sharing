using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharing.Data;
using RideSharing.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RideSharing.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly RideSharingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehiclesController(RideSharingContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
public async Task<IActionResult> Create([Bind("Make,Model,Year,LicensePlate,Color,Capacity")] Vehicle vehicle)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        Console.WriteLine("Napake v ModelState: " + string.Join(", ", errors));
        return View(vehicle);
    }

    var user = await _userManager.GetUserAsync(User); 
    if (user == null)
    {
        Console.WriteLine("Uporabnik ni prijavljen.");
        return Unauthorized();
    }

    Console.WriteLine("Prijavljen uporabnik: ");
    Console.WriteLine("UserId: " + user.Id);
    Console.WriteLine("UserName: " + user.UserName);

    vehicle.DriverId = user.Id;
    vehicle.CreatedAt = DateTime.UtcNow;
    vehicle.UpdatedAt = DateTime.UtcNow;

    Console.WriteLine("Podatki vozila pred shranjevanjem: ");
    Console.WriteLine($"DriverId: {vehicle.DriverId}, Make: {vehicle.Make}, Model: {vehicle.Model}");

    _context.Add(vehicle);

    try
    {
        await _context.SaveChangesAsync();
        Console.WriteLine("Vozilo uspešno shranjeno.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Napaka pri shranjevanju vozila: " + ex.Message);
    }

    return RedirectToAction(nameof(Index));
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
            return View(vehicle);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Make,Model,Year,LicensePlate,Color,Capacity")] Vehicle vehicle)
{
    if (id != vehicle.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
            {
                return NotFound();
            }

            Console.WriteLine("Obstoječe vozilo: ");
            Console.WriteLine($"Make: {existingVehicle.Make}, Model: {existingVehicle.Model}");

            existingVehicle.Make = vehicle.Make;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.Year = vehicle.Year;
            existingVehicle.LicensePlate = vehicle.LicensePlate;
            existingVehicle.Color = vehicle.Color;
            existingVehicle.Capacity = vehicle.Capacity;
            existingVehicle.UpdatedAt = DateTime.UtcNow;

            _context.Update(existingVehicle);
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

    return View(vehicle);
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
