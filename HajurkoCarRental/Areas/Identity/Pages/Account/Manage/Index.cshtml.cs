// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HajurkoCarRental.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HajurkoCarRental.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            [Display(Name = "User Picture")]
            public byte[] Picture { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            [StringLength(10, MinimumLength = 10)]
            public string PhoneNumber { get; set; }


            [Display(Name = "Diving License or Citizenship Document")]
            public IFormFile VerificationFile { get; set; }

            public String VerificationFileName { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var fullName = user.FullName;
            var Picture = user.Picture;
            var Address = user.Address;
            var VerificationFileName = user.VerificationFileName;
            

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = fullName,
                Picture = Picture,
                Address = Address,
                VerificationFileName = VerificationFileName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var fullName = user.FullName;
            if (Input.FullName != fullName)
            {
                user.FullName = Input.FullName;
            }

            var address = user.Address;
            if (Input.Address != address)
            {
                user.Address = Input.Address;
                
            }

            

            if (Input.VerificationFile != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");

                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                FileInfo fileInfo = new FileInfo(Input.VerificationFile.FileName);
                
                string extension = fileInfo.Extension;
                fileInfo.Refresh();
                long fileSize = Input.VerificationFile.Length;

                if (extension.ToLower() == ".png" || extension.ToLower() == ".pdf")
                {
                    
                } else
                {
                    StatusMessage = "Only png & pdf files are allowed to upload.";
                    return RedirectToPage();
                }

                if (fileSize > 1572864)
                {
                    StatusMessage = "File size shouldn't exceed 1.5MB";
                    return RedirectToPage();
                }

                string fileNameWithPath = Path.Combine(path, Input.VerificationFile.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    Input.VerificationFile.CopyTo(stream);
                }

                user.VerificationFileName = Input.VerificationFile.FileName;
                await _userManager.UpdateAsync(user);
            }

            

            if (Request.Form.Files.Count > 0)
            {
                Console.WriteLine(Request.Form.Files);
                IFormFile file;
                if (Input.VerificationFile == null)
                {
                    file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        user.Picture = dataStream.ToArray();
                    }
                    await _userManager.UpdateAsync(user);
                } else
                {
                    if (Request.Form.Files.Count == 2)
                    {
                        file = Request.Form.Files.LastOrDefault();
                        using (var dataStream = new MemoryStream())
                        {
                            await file.CopyToAsync(dataStream);
                            user.Picture = dataStream.ToArray();
                        }
                        await _userManager.UpdateAsync(user);
                    }
                }

                
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
