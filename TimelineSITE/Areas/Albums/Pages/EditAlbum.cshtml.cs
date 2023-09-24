using Azure.Storage.Blobs;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimelineSITE.Data;
using System.ComponentModel.DataAnnotations;
using TimelineSITE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TimelineSITE.Areas.Albums.Services;

namespace TimelineSITE.Areas.Albums.Pages
{
    [Authorize(Policy = "Users")]
    public class EditAlbumModel : PageModel
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AlbumPermissions _albumPermissions;

        public EditAlbumModel(UsersDbContext usersDbContext, UserManager<ApplicationUser> userManager, AlbumPermissions albumPermissions)
        {
            _usersDbContext = usersDbContext;
            _userManager = userManager;
            _albumPermissions = albumPermissions;
        }

        [BindProperty]
        public AlbumModel Album { get; set; } = new AlbumModel();

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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
                return Page();
            }

            using var transaction = _usersDbContext.Database.BeginTransaction();
            try
            {
                var albumToEdit = await _usersDbContext.Albums.FindAsync(Album.Id);
                albumToEdit.Name = Album.Name;
                albumToEdit.DateStart = Album.DateStart;
                albumToEdit.DateEnd = Album.DateEnd;
                await _usersDbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the album from database.");
                return Page();
            }

            return RedirectToPage("./PhotoAlbum", new { AlbumId = Album.Id });
        }
    }
}