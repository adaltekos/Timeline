using Azure.Storage.Blobs;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TimelineSITE.Data;
using TimelineSITE.Models;
using Microsoft.AspNetCore.Authorization;
using TimelineSITE.Areas.Albums.Services;

namespace TimelineSITE.Areas.Albums.Pages
{
    [Authorize(Policy = "Users")]
    public class DeleteAlbumModel : PageModel
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AlbumPermissions _albumPermissions;

        public DeleteAlbumModel(UsersDbContext usersDbContext, BlobServiceClient blobServiceClient, UserManager<ApplicationUser> userManager, AlbumPermissions albumPermissions)
        {
            _usersDbContext = usersDbContext;
            _blobServiceClient = blobServiceClient;
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

            try
            {
                await _blobServiceClient.DeleteBlobContainerAsync(Album.Id.ToString());
                using var transaction = _usersDbContext.Database.BeginTransaction();
                try
                {
                    _usersDbContext.Albums.Remove(Album);
                    await _usersDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError(string.Empty, "An error occurred while deleting the album from database.");
                    return Page();
                }
            }
            catch (RequestFailedException)
            {
                ModelState.AddModelError(string.Empty, "An error occurred on request when deleting container in azure.");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred when deleting container in azure.");
            }

            return RedirectToPage("./Index");
        }
    }
}
