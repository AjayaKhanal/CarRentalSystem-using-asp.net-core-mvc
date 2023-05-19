using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Areas.Identity.Data;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HajurkoCarRental.Controllers
{
    public class CarsController : Controller
    {
        private readonly HajurkoCarRentalDataContext _context;

        private readonly UserManager<User> _userManager;

        public CarsController(HajurkoCarRentalDataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager= userManager;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // Get price for car
        public async Task<IActionResult> GetData(int id)
        {
            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(User);
            List<Damage> damagePayment =  _context.Damage.Where(x => x.UserId == user.Id && x.Paid == false).ToList();
            bool pendingPayment = false;
            if (damagePayment.Any())
            {
                pendingPayment = true;
            }
            var data = new { rate = car.RentalRate, regularCustomer = user.RegularCustomer, available = car.IsAvailable, pendingPayment= pendingPayment, isStaff=user.Role!="Customer" };
            return Json(data);
        }

        // GET: Cars/Create
        //[Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (ModelState.IsValid)
            {
                // car image
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile? file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        car.Picture = dataStream.ToArray();
                        
                    }
                }
                car.IsAvailable = true;
                _context.Add(car);
                //save changes to the database
                await _context.SaveChangesAsync();

                TempData["success"] = "New car has been added successfully!!";
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }
            var existingCar = await _context.Car.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (ModelState.IsValid)
            {
                try
                {
                    if (car.Picture == null)
                    {
                        car.Picture = existingCar.Picture;
                    }
                    if (Request.Form.Files.Count > 0)
                    {
                        IFormFile? file = Request.Form.Files.FirstOrDefault();
                        using (var dataStream = new MemoryStream())
                        {
                            await file.CopyToAsync(dataStream);
                            car.Picture = dataStream.ToArray();

                        }
                    }
                    car.IsAvailable = true;
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["success"] = "Car details has been edited successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Car == null)
            {
                return Problem("Entity set 'HajurkoCarRentalDataContext.Car'  is null.");
            }
            var car = await _context.Car.FindAsync(id);
            if (car != null)
            {
                _context.Car.Remove(car);
            }
            
            await _context.SaveChangesAsync();

            TempData["error"] = "Category has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        //check car exists or not
        private bool CarExists(int id)
        {
          return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
