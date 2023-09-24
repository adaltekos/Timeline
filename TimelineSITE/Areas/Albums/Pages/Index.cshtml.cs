using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static TimelineSITE.Areas.Manage.Pages.Roles.IndexModel;
using TimelineSITE.Models;
using TimelineSITE.Data;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static TimelineSITE.Areas.Albums.Pages.ShareAlbumModel;
using Microsoft.AspNetCore.Authorization;

namespace TimelineSITE.Areas.Albums.Pages
{
    [Authorize(Policy = "Users")]
    public class IndexModel : PageModel
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel( UsersDbContext usersDbContext, UserManager<ApplicationUser> userManager)
        {
            _usersDbContext = usersDbContext;
            _userManager = userManager;
        }
        
        [BindProperty]
        public List<AlbumAccessModel> AlbumsAccessList { get; private set; } = new List<AlbumAccessModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            AlbumsAccessList = await _usersDbContext.AlbumsAccesses.Where(u=>u.UserId==user.Id).Include(aa => aa.Album).ToListAsync();
            
            return Page();
        }
    }
}
