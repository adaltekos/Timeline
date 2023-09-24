#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TimelineSITE.Models;

namespace TimelineSITE.Areas.Manage.Pages.Accounts
{
    [Authorize(Policy = "Manage")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateUserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {

            [Required]
            [EmailAddress]
            public string Email { get; set; }


            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }


            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model is not valid");
                return Page();
            }

            ApplicationUser identityUser = new()
            {
                UserName = Input.Email,
                Email = Input.Email
            };

            IdentityResult result = await _userManager.CreateAsync(identityUser, Input.Password);

            if (result.Succeeded)
            {
                StatusMessage = $"Successfully created user {Input.Email}.";
                return Page();
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return Page();
           
        }
    }
}
