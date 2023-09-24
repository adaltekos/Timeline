using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimelineSITE.Data;
using TimelineSITE.Models;
using TimelineSITE.Services;


namespace TimelineSITE.Areas.Albums.Pages
{
    [Authorize(Policy = "Users")]
    public class CreateAlbumModel : PageModel
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateAlbumModel(UsersDbContext userDbContext, BlobServiceClient blobServiceClient, UserManager<ApplicationUser> userManager)
        {
            _usersDbContext = userDbContext;
            _blobServiceClient = blobServiceClient;
            _userManager = userManager;
        }

        [BindProperty]
        public AlbumModel Album { get; set; } = new AlbumModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Model is not valid.");
                return Page();
            }

            using var transaction = _usersDbContext.Database.BeginTransaction();
            try
            {
                _usersDbContext.Albums.Add(Album);
                await _usersDbContext.SaveChangesAsync();

                try
                {
                    await _blobServiceClient.CreateBlobContainerAsync(Album.Id.ToString());
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred when creating container.");
                }

                var albumAccess = new AlbumAccessModel
                {
                    AlbumId = Album.Id,
                    UserId = (await _userManager.GetUserAsync(User)).Id,
                    ShareDate = DateTime.Now,
                    AccessLevel = AccessLevels.Owner
                };
                _usersDbContext.AlbumsAccesses.Add(albumAccess);
                await _usersDbContext.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            return RedirectToPage("./Index");
        }
    }
}
