
using HajurkoCarRental.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HajurkoCarRental.Controllers
{
    public class UserController : Controller
    {
        private readonly HajurkoCarRentalDataContext _db;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        public UserController(HajurkoCarRentalDataContext db,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }

        public class UserModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            public string? Id { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string? FullName { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string? Address { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Role")]
            [RegularExpression("^(Admin|Staff|Customer)$", ErrorMessage = "Only valid choices are Admin, Staff & Customer (Case Sensitive)")]
            public string? Role { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "Password must be Minimum eight characters, at least one letter, one number and one special character.")]
            [Display(Name = "Password")]
            public string? Password { get; set; }

            [Display(Name ="Regular")]
            public bool RegularCustomer { get; set; }

        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public IActionResult Index()
        {
            // var staffs = _db.Users.Where(user => user.Role == "Admin" || user.Role == "Staff").ToList();
            var staffs = _db.Users.OrderByDescending(b => b.FullName).ToList();
            return View(staffs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel userObj)
        {

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FullName = userObj.FullName;
                user.Address = userObj.Address;
                user.PhoneNumber = userObj.PhoneNumber;
                user.Role = userObj.Role;
                user.RegularCustomer = userObj.RegularCustomer;
                
                user.EmailConfirmed = true;

                await _userStore.SetUserNameAsync(user, userObj.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, userObj.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, userObj.Password);

                if (result.Succeeded)
                {

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                TempData["success"] = "New user has been created successfully!";
                return RedirectToAction("Index");
            } else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }


            return View(userObj);
        }


        //Editing User
        // GET
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = _db.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User obj)
        {

            if (ModelState.IsValid)
            {
               
                var user = _db.Users.Find(obj.Id);
                user.FullName = obj.FullName;
                user.PhoneNumber = obj.PhoneNumber;
                user.Address = obj.Address;
                user.Role = obj.Role;
                user.RegularCustomer = obj.RegularCustomer;
                
                await _db.SaveChangesAsync();

                TempData["success"] = "User details has been edited!";
                return RedirectToAction("Index");

            }
            return View(obj);
        }

        //Changing User Password
        // GET
        public IActionResult ChangePassword(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = _db.Users.Find(id);
            var userModel = new UserModel();
            userModel.Email = user.Email;
            userModel.Id = user.Id;
            userModel.FullName = user.FullName;
            userModel.Address = user.Address;
            userModel.PhoneNumber = user.PhoneNumber;
            userModel.Role = user.Role;

            if (user == null)
            {
                return NotFound();
            }
            return View(userModel);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserModel obj)
        {
            Console.WriteLine(ModelState);

                var user = _db.Users.Find(obj.Id);
                
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, obj.Password);
                

                TempData["success"] = "User password has been changed!";
                return RedirectToAction("Index");

        }

    }
}
