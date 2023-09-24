using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimelineSITE.Data;
using TimelineSITE.Models;

namespace TimelineSITE.Areas.Albums.Services
{
    public class AlbumPermissions
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlbumPermissions(UsersDbContext usersDbContext, UserManager<ApplicationUser> userManager)
        {
            _usersDbContext = usersDbContext;
            _userManager = userManager;
        }
        public async Task<bool> CheckAlbumAccess(string albumId, ClaimsPrincipal User, List<AccessLevels> accessLevels)
        {
            var album = await _usersDbContext.Albums.FindAsync(new Guid(albumId));
            if (album == null)
            {
                return false;
            }
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return false;
            }
            var albumAccess = await _usersDbContext.AlbumsAccesses.FirstOrDefaultAsync(aa => aa.UserId == currentUser.Id && aa.AlbumId == album.Id);
            if (albumAccess == null || !accessLevels.Contains(albumAccess.AccessLevel))
            {
                return false;
            }
            return true;
        }
    }
}
