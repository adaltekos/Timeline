using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TimelineSITE.Areas.Albums.Services;
using TimelineSITE.Data;
using TimelineSITE.Models;

namespace TimelineSITE.Areas.Albums.Pages
{
    [Authorize(Policy = "Users")]
    public class ShareAlbumModel : PageModel
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AlbumPermissions _albumPermissions;

        public ShareAlbumModel(UsersDbContext usersDbContext, UserManager<ApplicationUser> userManager, AlbumPermissions albumPermissions)
        {
            _usersDbContext = usersDbContext;
            _userManager = userManager;
            _albumPermissions = albumPermissions;
        }

        [BindProperty]
        public AlbumModel Album { get; set; } = new AlbumModel();

        [BindProperty]
        public List<AlbumAccessModel> SharedUsersList { get; set; } = new List<AlbumAccessModel>();

        private async Task LoadDataAsync(AlbumModel album)
        {
            SharedUsersList = await _usersDbContext.AlbumsAccesses
           .Where(aa => aa.AlbumId == album.Id).Include(u => u.User)
           .ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(string AlbumId)
        {
            //Set who can have access
            var requiredAccessLevels = new List<AccessLevels> { AccessLevels.Owner };
            var accessIsGranted = await _albumPermissions.CheckAlbumAccess(AlbumId, User, requiredAccessLevels);
            if (!accessIsGranted)
            {
                return RedirectToPage("./Index");
            }

            Album = await _usersDbContext.Albums.FindAsync(new Guid(AlbumId));
            await LoadDataAsync(Album);
            return Page();
        }

        public async Task<IActionResult> OnPostShare(string email, AccessLevels accesslevel)
        {
            //Set who can have access
            var requiredAccessLevels = new List<AccessLevels> { AccessLevels.Owner };
            var accessIsGranted = await _albumPermissions.CheckAlbumAccess(Album.Id.ToString(), User, requiredAccessLevels);
            if (!accessIsGranted)
            {
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model is not valid");
                await LoadDataAsync(Album);
                return Page();
            }

            var userToShare = await _userManager.FindByEmailAsync(email);
            if (userToShare == null)
            {
                ModelState.AddModelError("", "Email does not exist in database");
                await LoadDataAsync(Album);
                return Page();
            }

            try
            {
                var newAlbumAccess = new AlbumAccessModel
                {
                    UserId = userToShare.Id,
                    AlbumId = Album.Id,
                    ShareDate = DateTime.Now,
                    AccessLevel = accesslevel
                };
                _usersDbContext.AlbumsAccesses.Add(newAlbumAccess);
                await _usersDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlException && sqlException.Number == 2601)
                {
                    ModelState.AddModelError(string.Empty, "The share with given email arleady exist in database");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the album access to database.");
                }
            }

            await LoadDataAsync(Album);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAccess(string albumaccessid)
        {
            //Set who can have access
            var requiredAccessLevels = new List<AccessLevels> { AccessLevels.Owner };
            var accessIsGranted = await _albumPermissions.CheckAlbumAccess(Album.Id.ToString(), User, requiredAccessLevels);
            if (!accessIsGranted)
            {
                return RedirectToPage("./Index");
            }
            var albumAccessToDelete = await _usersDbContext.AlbumsAccesses.FirstOrDefaultAsync(aa => aa.Id.ToString() == albumaccessid);
            if (albumAccessToDelete == null)
            {
                return RedirectToPage("./Index");
            }
            if ((await _userManager.GetUserAsync(User)).Id.ToString() == albumAccessToDelete.UserId)
            {
                ModelState.AddModelError("", "You cannot delete your own access");
                await LoadDataAsync(Album);
                return Page();
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model is not valid");
                await LoadDataAsync(Album);
                return Page();
            }

            try
            {
                _usersDbContext.AlbumsAccesses.Remove(albumAccessToDelete);
                await _usersDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the album access from database.");
            }

            await LoadDataAsync(Album);
            return Page();
        }
    }
}
