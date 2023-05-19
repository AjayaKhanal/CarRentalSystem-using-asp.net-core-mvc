using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HajurkoCarRental.Areas.Identity.Data;
using HajurkoCarRental.Models;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Identity;
using HajurkoCarRental.Utilities;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;

namespace HajurkoCarRental.Controllers
{
    public class RentalRequestsController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly HajurkoCarRentalDataContext _context;

        public RentalRequestsController(HajurkoCarRentalDataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class RentalRequestInput
        {
            public int CarId { get; set; }

            [Required]

            public float Charge { get; set; }

            [Required]
            public DateTime RequestDate { get; set; }

            [Required]
            public bool Approved { get; set; }

            [Required]
            public bool Cancelled { get; set; }

            [Required]
            public DateTime ReturnDate { get; set; }


        }

        public class CustomerRentalModel
        {
            public string? FullName { get; set; }

            public byte[]? Picture { get; set; }

            public float TotalRental { get; set; }

            public string? Email { get; set; }

            public string? Address { get; set; }

            public string? PhoneNumber { get; set; }
        }

        // GET: RentalRequests
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<RentalRequest> requestData;
            if(currentUser.Role == "Customer")
            {
                requestData = _context.RentalRequest.Where(e => e.UserId == currentUser.Id).Include(r => r.Car).Include(u => u.User);

            } else
            {
                requestData = _context.RentalRequest.Include(r => r.Car).Include(u => u.User);
            }
            
            return View(requestData.OrderByDescending(e => e.Id).ToList());
        }


        public async Task<IActionResult> SalesReport()
        {
            string from = Request.Query["from"];
            string upto = Request.Query["upto"];

            DateTime start;
            DateTime end;


            float totalSales = 0;

            IEnumerable<RentalRequest> requestData;

            requestData = _context.RentalRequest.Where(e => e.Approved == true).Include(r => r.Car).Include(u => u.User).Include(u => u.AuthorizedBy);

            if (from != null && from != "")
            {
                start = DateTime.Parse(from);
                requestData = requestData.Where(b => b.RequestDate >= start);
            }
            if (upto != null && upto != "")
            {
                end = DateTime.Parse(upto);
                requestData = requestData.Where(b => b.RequestDate <= end);
            }

            foreach (var rentalRequest in requestData)
            {
                totalSales += rentalRequest.Charge;
            }
            ViewBag.Total = totalSales;
            ViewBag.From = from;
            ViewBag.Upto = upto;
            return View(requestData.OrderByDescending(e => e.RequestDate).ToList());
        }

        public async Task<IActionResult> CustomerRental(String id)
        {
            IEnumerable<RentalRequest> requestData;

            requestData = _context.RentalRequest.Where(e => e.UserId == id).Include(r => r.Car).Include(u => u.User).Include(u => u.AuthorizedBy);

            var user = _context.Users.Find(id);
            ViewData["UserDetail"] = user;

            return View(requestData.OrderByDescending(e => e.RequestDate).ToList());
        }

        public async Task<IActionResult> Status()
        {
            List<Car> AllCars =  _context.Car.ToList();

            List<Car> AvailableCars = new List<Car>();
            List<Car> FrequentCars = new List<Car>();
            List<Car> NotRentedCars = new List<Car>();
            List<Car> OnRentCars = new List<Car>();

            foreach (Car car in AllCars)
            {
                if (car.IsAvailable)
                {
                    // To get cars that are currently available
                    AvailableCars.Add(car);
                }
                else
                {
                    // To get cars that are on rent
                    OnRentCars.Add(car);
                }

                List<RentalRequest> Rentals = _context.RentalRequest.Where(x => x.CarId == car.Id).ToList();

                if (Rentals.Count() == 0)
                {
                    // To get cars that have not been rented before
                    NotRentedCars.Add(car);

                }

                // To get frequently rented cars, i.e. if it has been rented for more than 3 times
                if (Rentals.Where(x => x.Approved == true).Count() >= 3)
                {
                    FrequentCars.Add(car);
                }

            }

            ViewData["AvailableCars"] = AvailableCars;
            ViewData["FrequentCars"] = FrequentCars;
            ViewData["NotRentedCars"] = NotRentedCars;
            ViewData["OnRentCars"] = OnRentCars;
            return View();
        }

        public async Task<IActionResult> UserStatus()
        {
            List<User> AllUsers = _context.Users.ToList();

            List<CustomerRentalModel> FrequentCustomers = new List<CustomerRentalModel>();
            List<CustomerRentalModel> InactiveCustomers = new List<CustomerRentalModel>();

            foreach (User user in AllUsers)
            {
                List<RentalRequest> Rentals = _context.RentalRequest.Where(x => x.UserId == user.Id).ToList();

                var customer = new CustomerRentalModel();
                customer.PhoneNumber = user.PhoneNumber;
                customer.Address = user.Address;
                customer.FullName = user.FullName;
                customer.Email = user.Email;
                customer.Picture = user.Picture;
                customer.TotalRental = Rentals.Count();

                if(Rentals.Count > 0)
                {
                    FrequentCustomers.Add(customer);

                } else
                {
                    InactiveCustomers.Add(customer);
                }


            }

            ViewData["FrequentCustomers"] = FrequentCustomers.OrderByDescending(x => x.TotalRental).ToList();
            ViewData["InactiveCustomers"] = InactiveCustomers.OrderByDescending(x => x.TotalRental).ToList();
            return View();
        }



        // GET: RentalRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RentalRequest == null)
            {
                return NotFound();
            }

            var rentalRequest = await _context.RentalRequest
                .Include(r => r.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rentalRequest == null)
            {
                return NotFound();
            }

            return View(rentalRequest);
        }
        

        // GET: RentalRequests/Create
        public async Task<IActionResult> Create(int carId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Please login to continue!";
                return Redirect("/Identity/Account/Login");
            }
            if (user.VerificationFileName == null)
            {
                TempData["error"] = "Please upload your driving license or citizenship to continue!";
                return Redirect("/Identity/Account/Manage");
            }
            //ViewData["CarId"] = new SelectList(_context.Car, "Id", "Brand");
            ViewData["CarId"] = carId;
            return View();
        }

        // POST: RentalRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarId,Charge,RequestDate,Approved,Cancelled,ReturnDate")] RentalRequestInput rentalRequest)
        {
            if (rentalRequest.RequestDate > rentalRequest.ReturnDate)
            {
                ModelState.AddModelError("ReturnDate", "Rental Return Date should be after the request date.");

            }
            
            if (ModelState.IsValid)
            {
                var newRentalRequest = new RentalRequest();
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var car = await _context.Car.FirstOrDefaultAsync(m => m.Id == rentalRequest.CarId);
                newRentalRequest.Approved = rentalRequest.Approved;
                newRentalRequest.RequestDate = rentalRequest.RequestDate;
                newRentalRequest.ReturnDate = rentalRequest.ReturnDate;
                newRentalRequest.Cancelled = rentalRequest.Cancelled;
                newRentalRequest.Charge = rentalRequest.Charge;
                newRentalRequest.User = currentUser;
                newRentalRequest.UserId = currentUser.Id;
                newRentalRequest.Car = car;
                newRentalRequest.CarId = rentalRequest.CarId;

                _context.Add(newRentalRequest);

                TempData["success"] = "Rental request has been created successfully!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Brand", rentalRequest.CarId);
            return View(rentalRequest);
        }

        // GET: RentalRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RentalRequest == null)
            {
                return NotFound();
            }

            var rentalRequest = await _context.RentalRequest.FindAsync(id);
            if (rentalRequest == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Brand", rentalRequest.CarId);
            return View(rentalRequest);
        }

        // POST: RentalRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RentalRequestInput rentalRequest)
        {
            if (rentalRequest.Cancelled)
            {
                if (DateTime.Now > rentalRequest.RequestDate)
                {
                    TempData["error"] = "You can only cancel the rental rquest before the request date!";
                   return RedirectToAction("Edit");
                }
            }
            
            var RentalRequest = await _context.RentalRequest.FindAsync(id);
            RentalRequest.Approved = rentalRequest.Approved;
            RentalRequest.Cancelled = rentalRequest.Cancelled;
            RentalRequest.Charge = rentalRequest.Charge;
            try
            {

                TempData["success"] = "Rental Request has been edited successfully!";
                _context.Update(RentalRequest);
                await _context.SaveChangesAsync();

                if (rentalRequest.Approved)
                {
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    RentalRequest.AuthorizedBy = currentUser;
                    RentalRequest.AuthorizedById = currentUser.Id;
                    _context.Update(RentalRequest);
                    await _context.SaveChangesAsync();


                    var car = await _context.Car.FindAsync(RentalRequest.CarId);
                    car.IsAvailable = false;
                    _context.Update(car);
                    await _context.SaveChangesAsync();

                    var userDetail = await _context.Users.FindAsync(RentalRequest.UserId);
                    List<string> userEmail = new List<string>();
                    userEmail.Add(userDetail.Email);
                    EmailHelper.SendEmail("Rental Request Status of " + car.Brand, "Congratulations, your rental request for " + car.Brand + " was approved successfully!", userEmail);
                }

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["CarId"] = new SelectList(_context.Car, "Id", "Brand", rentalRequest.CarId);
            return View(rentalRequest);
        }

        // GET: RentalRequests/Delete/5
        //[Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RentalRequest == null)
            {
                return NotFound();
            }

            var rentalRequest = await _context.RentalRequest
                .Include(r => r.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rentalRequest == null)
            {
                return NotFound();
            }

            return View(rentalRequest);
        }

        // POST: RentalRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RentalRequest == null)
            {
                return Problem("Entity set 'HajurkoCarRentalDataContext.RentalRequest'  is null.");
            }
            var rentalRequest = await _context.RentalRequest.FindAsync(id);
            if (rentalRequest != null)
            {
                _context.RentalRequest.Remove(rentalRequest);
            }

            TempData["error"] = "Rental Request has been deleted successfully!";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalRequestExists(int id)
        {
          return (_context.RentalRequest?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
