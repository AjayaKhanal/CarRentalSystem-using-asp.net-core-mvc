﻿using System;
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
using System.ComponentModel.DataAnnotations;

namespace HajurkoCarRental.Controllers
{
    public class CarDamaged
    {
        public int CarId { get; set; }
        public string CarBrand { get; set; }
    }

    public class DamagesController : Controller
    {
        private readonly HajurkoCarRentalDataContext _context;

        private readonly UserManager<User> _userManager;
        public DamagesController(HajurkoCarRentalDataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class DamageInput
        {

            [Display(Name = "Car")]
            public int CarId { get; set; }
            public string? Description { get; set; }

            public float? RepairCost { get; set; }

            public bool Paid { get; set; }
        }

        // GET: Damages
        public async Task<IActionResult> Index()
        {
            List<Damage> hajurkoCarRentalDataContext = await _context.Damage.Include(d => d.Car).Include(e => e.User).OrderByDescending(c => c.Id).ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currentUser.Role == "Customer")
                {
                    hajurkoCarRentalDataContext = hajurkoCarRentalDataContext.Where(x => x.UserId == currentUser.Id).ToList();

                }
            }
            return View(hajurkoCarRentalDataContext);
        }

        // GET: Damages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Damage == null)
            {
                return NotFound();
            }

            var damage = await _context.Damage
                .Include(d => d.Car).Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (damage == null)
            {
                return NotFound();
            }

            return View(damage);
        }

        // GET: Damages/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var DamagedCars = await _context.CarReturn.Where(i=>i.IsDamaged)
                .Join(_context.RentalRequest,
                carReturn => carReturn.RentalRequestId,
                rentalRequest => rentalRequest.Id,
                (carReturn, rentalRequest) => new CarDamaged
                {
                    CarId = rentalRequest.CarId,
                    CarBrand = rentalRequest.Car.Brand
                })
                .ToListAsync();

            //var DamagedCars = await _context.CarReturn.Where(i => i.IsDamaged == true).ToListAsync();

            //var Cars = await _context.Car.ToListAsync();

            //var DamagedList = new List<CarDamaged>();
            //var DamageCar = new List<Car>();

            //if (DamagedCars.Count > 0)
            //{
            //    foreach (var car in DamagedCars)
            //    {
            //        var RequestedCars = await _context.RentalRequest.Where(i => i.Id == car.RentalRequestId).ToListAsync();
            //        if (RequestedCars.Count > 0)
            //        {
            //            foreach (var carid in RequestedCars)
            //            {
            //                DamageCar = Cars.Where(c => c.Id == carid.CarId).ToList();

            //                //var DamageCar = Cars.Where(c => c.Id == car.RentalRequest.CarId).ToList();
            //            }
            //        }
            //        if (DamageCar.Count > 0)
            //        {
            //            foreach (var car1 in DamageCar)
            //            {
            //                var Damaged = new CarDamaged
            //                {
            //                    CarId = car1.Id,
            //                    CarBrand = car1.Brand
            //                };
            //                DamagedList.Add(Damaged);
            //            }
            //        }
            //    }

            //    ViewData["CarId"] = new SelectList(DamagedList, "CarId", "CarBrand");
            //}

            if(DamagedCars.Count > 0)
            {
                ViewData["CarId"] = new SelectList(DamagedCars, "CarId", "CarBrand");
            }
            else
            {
                // Create a SelectList with a single item for "No Damaged Cars"
                var noCarsList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No Damaged Cars", Value = "-1" }
                };
                ViewData["CarId"] = new SelectList(noCarsList, "Value", "Text");
            }

            return View();
        }

        // POST: Damages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DamageInput damage)
        {
            if (ModelState.IsValid)
            {
                //find damage car details
                var carDetails = await _context.Car.FirstOrDefaultAsync(m => m.Id == damage.CarId);

                var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                if (carDetails != null)
                {
                    Damage newDamage = new Damage();
                    newDamage.CarId = damage.CarId;
                    newDamage.Description = damage.Description;
                    newDamage.User = currentUser;
                    newDamage.UserId = currentUser.Id;
                    newDamage.Car = carDetails;
                    newDamage.Paid = false;
                    newDamage.RepairCost = null;


                    _context.Add(newDamage);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Your damage form was submitted & we will update the repair cost soon which you need to pay!";
                }
                else
                {
                    try
                    {
                        TempData["error"] = "No car found";
                    }
                    catch (Exception e)
                    {
                        TempData["error"] = e.Message;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Brand", damage.CarId);
            return View(damage);
        }

        // GET: Damages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Damage == null)
            {
                return NotFound();
            }

            var damage = await _context.Damage.FindAsync(id);
            if (damage == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Brand", damage.CarId);
            return View(damage);
        }

        // POST: Damages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DamageInput damage)
        {

            if (ModelState.IsValid)
            {
                var damageObj = await _context.Damage.FirstOrDefaultAsync(m => m.Id == id);
                damageObj.RepairCost = damage.RepairCost;
                damageObj.Paid = damage.Paid;

                _context.Update(damageObj);
                TempData["success"] = "Damage form was edited successfully!";

                if (damageObj.Paid == true)
                {
                    var carReturn = await _context.CarReturn.FirstOrDefaultAsync(cr => cr.RentalRequest.CarId == damageObj.CarId);
                    if(carReturn!=null) {
                        carReturn.IsDamaged = false;
                        _context.Update(carReturn);
                    };
                    
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(damage);
        }

        // GET: Damages/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Damage == null)
            {
                return NotFound();
            }

            var damage = await _context.Damage
                .Include(d => d.Car).Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (damage == null)
            {
                return NotFound();
            }

            return View(damage);
        }

        // POST: Damages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Damage == null)
            {
                return Problem("Entity set 'HajurkoCarRentalDataContext.Damage'  is null.");
            }
            var damage = await _context.Damage.FindAsync(id);
            if (damage != null)
            {
                _context.Damage.Remove(damage);
            }

            await _context.SaveChangesAsync();
            TempData["error"] = "Damage record was deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        //check damage exists or not
        private bool DamageExists(int id)
        {
            return (_context.Damage?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Damages/Payment/5
        public async Task<IActionResult> Payment(int? id)
        {
            var damage = await _context.Damage.FirstOrDefaultAsync(m => m.Id == id);
            damage.Paid = true;
            _context.Update(damage);
            TempData["success"] = "Payment done successfully, thank you!";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
