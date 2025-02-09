using System.ComponentModel.DataAnnotations;
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

        
        
        [AllowNull]
        public ICollection<Comment> Comments { get; set; } // nullable
        [AllowNull]
        public ICollection<Like> Likes { get; set; }
        [AllowNull]
        public ICollection<Media> Media { get; set; }
    }
}
