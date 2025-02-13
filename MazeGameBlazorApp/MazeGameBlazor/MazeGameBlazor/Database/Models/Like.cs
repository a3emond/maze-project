using System;

namespace MazeGameBlazor.Database.Models
{
    /// <summary>
    /// Represents a "Like" on a blog post by a user.
    /// </summary>
    public class Like
    {
        /// <summary>
        /// Primary Key: Unique identifier for the like entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign Key: ID of the associated blog post.
        /// </summary>
        public int BlogPostId { get; set; }

        /// <summary>
        /// Navigation property: The blog post that received the like.
        /// </summary>
        public BlogPost BlogPost { get; set; }

        /// <summary>
        /// Foreign Key: ID of the user who liked the post.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Navigation property: The user who liked the blog post.
        /// </summary>
        public User User { get; set; }
    }
}