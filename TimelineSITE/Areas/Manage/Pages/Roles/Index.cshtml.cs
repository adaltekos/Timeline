#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace TimelineSITE.Areas.Manage.Pages.Roles
{
    [Authorize(Policy = "Manage")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public class RoleModel
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
        }

        [BindProperty]
        public List<RoleModel> RolesList { get; set; } = new List<RoleModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            foreach (IdentityRole role in roles)
            {
                var thisRoleModel = new RoleModel();
                thisRoleModel.RoleId = role.Id;
                thisRoleModel.RoleName = role.Name;
                RolesList.Add(thisRoleModel);
            }
            return Page();
        }
    }
}
