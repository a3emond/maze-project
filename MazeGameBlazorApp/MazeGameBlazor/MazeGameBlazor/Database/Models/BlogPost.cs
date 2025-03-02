using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MazeGameBlazor.Database.Models;

/// <summary>
///     Represents a blog post in the system.
/// </summary>
public class BlogPost
{
    /// <summary>
    ///     Primary Key: Unique identifier for the blog post.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Title of the blog post (Max: 100 characters).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = "";

    /// <summary>
    ///     Content/body of the blog post (Max: 1000 characters).
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = "";

    /// <summary>
    ///     Timestamp when the post was created (defaults to the current time).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    ///     The number of likes associated with this blog post.
    /// </summary>
    public int LikeCount { get; set; } = 0;

    /// <summary>
    ///     Foreign Key: ID of the author (User).
    /// </summary>
    [Required]
    public string AuthorId { get; set; }

    /// <summary>
    ///     Navigation property: The author of this blog post.
    /// </summary>
    [Required]
    public User Author { get; set; }

    /// <summary>
    ///     Foreign Key: Media associated with this post (optional).
    /// </summary>
    public int? MediaId { get; set; }

    /// <summary>
    ///     Navigation property: The media attached to this blog post.
    /// </summary>
    [ForeignKey("MediaId")]
    public Media? Media { get; set; }

    /// <summary>
    ///     Navigation property: List of comments associated with this blog post.
    /// </summary>
    [AllowNull]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    ///     Navigation property: List of likes associated with this blog post.
    /// </summary>
    [AllowNull]
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}