using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Areas.Identity.Data;
using HajurkoCarRental.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HajurkoCarRental.Controllers
{
    //car controller
    public class CarReturnsController : Controller
    {
        private readonly HajurkoCarRentalDataContext _context;

        public CarReturnsController(HajurkoCarRentalDataContext context)
        {
            _context = context;
        }

        public class CarReturnInput
        {

            public int RentalRequestId { get; set; }


            [Display(Name = "Fuel Level")]
            public float FuelLevel { get; set; }
            public bool IsDamaged { get; set; }

            [Required]
            [Display(Name = "Return Date")]
            public DateTime ReturnDate { get; set; }
        }

        // GET: CarReturns
        public async Task<IActionResult> Index()
        {
            var hajurkoCarRentalDataContext = _context.CarReturn.OrderByDescending(x => x.ReturnDate).Include(c => c.RentalRequest);
            return View(await hajurkoCarRentalDataContext.ToListAsync());
        }

        // GET: CarReturns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarReturn == null)
            {
                return NotFound();
            }

            var carReturn = await _context.CarReturn
                .Include(c => c.RentalRequest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carReturn == null)
            {
                return NotFound();
            }

            return View(carReturn);
        }

        // GET: CarReturns/Create
        public IActionResult Create()
        {
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequest, "Id", "Id");
            return View();
        }

        // POST: CarReturns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarReturnInput carReturn)
        {
            bool returnExists = _context.CarReturn.Any(m => m.RentalRequestId == carReturn.RentalRequestId);
            if (returnExists)
            {
                ModelState.AddModelError("RentalRequestId", "Return for this request id is already recorded!");
            }

            if (ModelState.IsValid)
            {
                var rentalRequest = await _context.RentalRequest.FirstOrDefaultAsync(m => m.Id == carReturn.RentalRequestId);
                var newCarReturn = new CarReturn();

                newCarReturn.RentalRequest = rentalRequest;
                newCarReturn.RentalRequestId = carReturn.RentalRequestId;
                newCarReturn.FuelLevel = carReturn.FuelLevel;
                newCarReturn.ReturnDate = carReturn.ReturnDate;
                newCarReturn.IsDamaged = carReturn.IsDamaged;

                _context.Add(newCarReturn);
                //Save changes to the database
                await _context.SaveChangesAsync();

                var carDetails = await _context.Car.FirstOrDefaultAsync(m => m.Id == rentalRequest.CarId);
                carDetails.IsAvailable = true;
                _context.Update(carDetails);
                await _context.SaveChangesAsync();

                //success message
                TempData["success"] = "New Rental record was created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequest, "Id", "Id", carReturn.RentalRequestId);
            return View(carReturn);
        }

        // GET: CarReturns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarReturn == null)
            {
                return NotFound();
            }

            // fiind car return
            var carReturn = await _context.CarReturn.FindAsync(id);
            if (carReturn == null)
            {
                return NotFound();
            }

            var rentalRequest = _context.RentalRequest.Where(rr => rr.Approved == true);
            ViewData["RentalRequestId"] = new SelectList(rentalRequest, "Id", "Id", carReturn.RentalRequestId);
            return View(carReturn);
        }

        // POST: CarReturns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarReturnInput carReturn)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCarReturn = await _context.CarReturn.FindAsync(id);
                    existingCarReturn.FuelLevel = carReturn.FuelLevel;
                    existingCarReturn.ReturnDate = carReturn.ReturnDate;
                    existingCarReturn.IsDamaged = carReturn.IsDamaged;
                    TempData["success"] = "Return rental has been edited successfully!";
                    _context.Update(existingCarReturn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarReturnExists(id))
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
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequest, "Id", "Id", carReturn.RentalRequestId);
            return View(carReturn);
        }

        // GET: CarReturns/Delete/5
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarReturn == null)
            {
                return NotFound();
            }

            var carReturn = await _context.CarReturn
                .Include(c => c.RentalRequest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carReturn == null)
            {
                return NotFound();
            }

            return View(carReturn);
        }

        // POST: CarReturns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarReturn == null)
            {
                return Problem("Entity set 'HajurkoCarRentalDataContext.CarReturn'  is null.");
            }
            var carReturn = await _context.CarReturn.FindAsync(id);
            if (carReturn != null)
            {
                _context.CarReturn.Remove(carReturn);
            }
            TempData["error"] = "Return rental has been deleted successfully!";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarReturnExists(int id)
        {
          return (_context.CarReturn?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
