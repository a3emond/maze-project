using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MazeGameBlazor.Database
{
    /// <summary>
    /// Application Database Context for Entity Framework Core.
    /// Inherits from IdentityDbContext to support authentication and role management.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Database tables
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Media> Media { get; set; }

        /// <summary>
        /// Configures the database model relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensures Identity framework configuration is applied.

            // ✅ One-to-Many: User <-> BlogPost (A user can have many blog posts)
            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.AuthorId)
                .IsRequired(); // A blog post must have an author

            // ✅ One-to-Many: BlogPost <-> Comment (Cascade Delete)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade); // Delete all comments when a BlogPost is deleted

            // ✅ One-to-Many: BlogPost <-> Likes (Cascade Delete)
            modelBuilder.Entity<Like>()
                .HasOne(l => l.BlogPost)
                .WithMany(bp => bp.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade); // Delete likes when a BlogPost is deleted

            // ✅ One-to-Many: User <-> Likes (Restrict Deletion)
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting users with existing likes

            // ✅ Store MediaType Enum as String in the Database
            modelBuilder.Entity<Media>()
                .Property(m => m.Type)
                .HasConversion<string>()
                .IsRequired(); // Prevents NULL values
        }
    }
}
