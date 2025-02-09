using System.ComponentModel.DataAnnotations;

namespace MazeGameBlazor.Database.Models
{
    public class Comment
    {
        public int Id { get; set; } // Primary Key

        [Required]
        [MaxLength(800)]
        public string Content { get; set; } = String.Empty;

        [Required]
        [MaxLength(50)]
        public string Author { get; set; } = String.Empty;


        [Required]
        public int BlogPostId { get; set; } // Foreign Key
        [Required]
        public BlogPost BlogPost { get; set; }
    }
}
