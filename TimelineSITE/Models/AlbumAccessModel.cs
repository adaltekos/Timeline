using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelineSITE.Models
{
    public enum AccessLevels
    {
        Owner = 1,
        Guest = 2,
        ViewOnly = 3
    }
    public class AlbumAccessModel
    {
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Albums")]
        public Guid AlbumId { get; set; }
        public virtual AlbumModel? Album { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public DateTime ShareDate { get; set; }

        public AccessLevels AccessLevel { get; set; }
    }
}
