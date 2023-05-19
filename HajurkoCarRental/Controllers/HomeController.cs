using HajurkoCarRental.Areas.Identity.Data;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HajurkoCarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HajurkoCarRentalDataContext _context;


        public HomeController(ILogger<HomeController> logger, HajurkoCarRentalDataContext context)
        {
            _logger = logger;
            _context = context;
        }

        // view car with search
        public async Task<IActionResult> Index()
        {
            string search = Request.Query["search"];

            var carsList = _context.Car.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                carsList = carsList.Where(c => c.Brand.Contains(search) || c.Model.Contains(search) || c.PlateNumber.Contains(search));
            }
            List<Car> newCarsList = carsList.OrderByDescending(x=>x.Id).ToList();
            ViewBag.Search = search;
            return View(newCarsList);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}