using System;
using System.ComponentModel.DataAnnotations;

namespace MazeGameBlazor.Database.Models
{
    /// <summary>
    /// Represents a comment on a blog post.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Primary Key: Unique identifier for the comment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The content of the comment (Max: 800 characters).
        /// </summary>
        [Required]
        [MaxLength(800)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// The name of the comment's author (Max: 50 characters).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Foreign Key: ID of the associated blog post.
        /// </summary>
        [Required]
        public int BlogPostId { get; set; }

        /// <summary>
        /// Navigation property: The blog post to which this comment belongs.
        /// </summary>
        [Required]
        public BlogPost BlogPost { get; set; }
    }
}