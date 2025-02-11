using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MazeGameBlazor.Database.Models
{
    public class BlogPost
    {
        public int Id { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = ""; // Default value

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = ""; // Default value
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default value
        public int LikeCount { get; set; } = 0; // Default value

        [Required]
        public string AuthorId { get; set; } // Foreign Key
        [Required]
        public User Author { get; set; }

        // ✅ Keep only one Media per BlogPost
        public int? MediaId { get; set; }  // Nullable in case a post has no media
        [ForeignKey("MediaId")]
        public Media? Media { get; set; }

        // Other relationships
        [AllowNull]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        [AllowNull]
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}