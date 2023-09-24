#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TimelineSITE.Models;

namespace TimelineSITE.Areas.Manage.Pages.Accounts
{
    public class ManageUserRolesModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUserRolesModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public string UserEmail { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<ManageUserRolesViewModel> Input { get; set; } = new List<ManageUserRolesViewModel>();

        public class ManageUserRolesViewModel
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
            public bool Selected { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User with Id = {userId} cannot be found");
            }
            UserEmail = user.Email;
            UserId = user.Id;
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                Input.Add(userRolesViewModel);
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model is not valid");
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ModelState.AddModelError("", $"User {UserEmail} doesn't exist.");
                return Page();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return Page();
            }

            result = await _userManager.AddToRolesAsync(user, Input.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                result = await _userManager.AddToRolesAsync(user, roles);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot recreate roles from backup");
                }
                return Page();
            }

            var listSelected = Input.Where(x => x.Selected).Select(y => y.RoleName).ToList();
            var listAdded = listSelected.Except(roles);
            var listRemoved = roles.Except(listSelected);

            StatusMessage = $"{(listAdded.Any() ? "Role " + string.Join(", ", listAdded) + " added. " : string.Empty)}" +
                $"{(listRemoved.Any() ? "Role " + string.Join(", ", listRemoved) + " removed." : string.Empty)}";

            return Page();
        }

    }
}
