using System.ComponentModel.DataAnnotations;

namespace MazeGameBlazor.Database.Models
{
    public class Media
    {
        public int Id { get; set; } // Primary Key
        [Required]
        [MaxLength(200)]
        public string Url { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string ThumbnailUrl { get; set; } = string.Empty;
        public MediaType Type { get; set; }

        public int BlogPostId { get; set; } // Foreign Key
        public BlogPost BlogPost { get; set; }
    }
    public enum MediaType
    {
        Image,
        Video,
        Audio,
        Document
    }
}
