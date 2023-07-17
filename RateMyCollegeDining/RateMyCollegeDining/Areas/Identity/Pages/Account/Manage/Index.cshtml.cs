﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RateMyCollegeDining.Areas.Identity.Data;

namespace RateMyCollegeDining.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        [Display(Name = "Date Joined")]
        [DataType(DataType.DateTime)]
        public DateTime DateJoined { get; set; }

        public string name { get; set; }

        public string university { get; set; }

        public string bio { get; set; }

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

            [DataType(DataType.Text)]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [DataType(DataType.MultilineText)]
            [Display(Name = "Bio")]
            public string Bio { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "University")]
            public string University { get; set; }

            [DataType(DataType.ImageUrl)]
            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var dateUser = await _userManager.GetUserAsync(User);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            DateJoined = dateUser.Date_Joined;
            university = dateUser.University;
            name = dateUser.Name;
            bio = "No Bio";
            if(dateUser.Bio != null)
            {
                bio = dateUser.Bio;
            }

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = name,
                University = university,
                Bio = bio
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
            if(Input.Name != user.Name)
            {
                if(Input.Name != null)
                {
                    user.Name = Input.Name;
                }
            }

            if(Input.University != user.University)
            {
                if(Input.University != null)
                {
                    user.University = Input.University;
                }
            }

            if(Input.Bio != user.Bio)
            {
                if(Input.Bio != null) 
                {
                    user.Bio = Input.Bio;
                }
            }

            if(Request.Form.Files.Count > 0) 
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream()) 
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
