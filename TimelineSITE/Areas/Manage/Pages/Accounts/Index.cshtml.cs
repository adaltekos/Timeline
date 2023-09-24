#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimelineSITE.Models;

namespace TimelineSITE.Areas.Manage.Pages.Accounts
{
    [Authorize(Policy = "Manage")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserModel
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public bool EmailConfirmed { get; set; }
            public DateTimeOffset? LockoutEnd { get; set; }
            public bool LockoutEnabled { get; set; }
            public int AccessFaildCount { get; set; }
            public IEnumerable<string> Roles { get; set; }
        }

        [BindProperty]
        public List<UserModel> IdentityUserList { get; set; } = new List<UserModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (ApplicationUser user in users)
            {
                var thisUserModel = new UserModel();
                thisUserModel.UserId = user.Id;
                thisUserModel.Email = user.Email;
                thisUserModel.EmailConfirmed = user.EmailConfirmed;
                thisUserModel.LockoutEnd = user.LockoutEnd;
                thisUserModel.LockoutEnabled = user.LockoutEnabled;
                thisUserModel.AccessFaildCount = user.AccessFailedCount;
                thisUserModel.Roles = await GetUserRoles(user);
                IdentityUserList.Add(thisUserModel);
            }
            return Page();
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}
