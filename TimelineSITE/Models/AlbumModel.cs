using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelineSITE.Models
{
    public class AlbumModel
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(63, MinimumLength = 1)]
        [Display(Name = "Album Name")]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Journey Start")]
        public DateTime? DateStart { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Journery End")]
        public DateTime? DateEnd { get; set; }

        public ICollection<AlbumAccessModel> UsersAccess { get; set; } = new List<AlbumAccessModel>();
        public ICollection<ImageModel> Images { get; set; } = new List<ImageModel>();
    }
}
