using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace MazeGameBlazor.Database.Models
{
    public class User : IdentityUser
    {
        // Additional custom properties
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default value

        // Navigation properties
        [AllowNull]
        public ICollection<BlogPost> BlogPosts { get; set; }
        [AllowNull]
        public ICollection<Like> Likes { get; set; }
    }

}
