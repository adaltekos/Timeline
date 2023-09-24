using Microsoft.AspNetCore.Identity;

namespace TimelineSITE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<AlbumAccessModel> AlbumsAccess { get; set; } = new List<AlbumAccessModel>();
    }
}
