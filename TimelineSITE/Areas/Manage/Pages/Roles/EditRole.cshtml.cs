#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace TimelineSITE.Areas.Manage.Pages.Roles
{
    [Authorize(Policy = "Manage")]
    public class EditRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditRoleModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Required]
        [BindProperty]
        public string RoleId { get; set; }

        [Required]
        [BindProperty]
        public string RoleName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                StatusMessage = $"User with Id = {roleId} cannot be found";
                return Page();
            }
            RoleId = role.Id;
            RoleName = role.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model is not valid");
                return Page();
            }

            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                ModelState.AddModelError("", $"Role doesn't exist.");
                return Page();
            }

            var roleName = await _roleManager.GetRoleNameAsync(role);
            if (RoleName != roleName)
            {
                var setRoleNameResult = await _roleManager.SetRoleNameAsync(role, RoleName);
                if (setRoleNameResult.Succeeded)
                {
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        StatusMessage = $"Role changed name from {roleName} to {RoleName}.";
                        return Page();
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                foreach (var error in setRoleNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
