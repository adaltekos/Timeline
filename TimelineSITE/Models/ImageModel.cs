using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimelineSITE.Models
{
    public class ImageModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] ThumbnailImage { get; set; }
        [Required]
        [ForeignKey("Albums")]
        public Guid AlbumId { get; set; }
        public virtual AlbumModel? Album { get; set; }
    }
}
