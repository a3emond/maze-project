using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace MazeGameBlazor.Database.Models;

/// <summary>
///     Represents a user in the Identity system.
///     Inherits from IdentityUser to enable authentication and role management.
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    ///     Timestamp when the user account was created (default: current time).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    ///     Navigation property: Blog posts authored by this user.
    /// </summary>
    [AllowNull]
    public ICollection<BlogPost> BlogPosts { get; set; }

    /// <summary>
    ///     Navigation property: Likes made by this user on blog posts.
    /// </summary>
    [AllowNull]
    public ICollection<Like> Likes { get; set; }
}