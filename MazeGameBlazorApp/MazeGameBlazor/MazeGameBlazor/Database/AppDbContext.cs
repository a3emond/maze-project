using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MazeGameBlazor.Database
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Media> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity configuration

            // ✅ One-to-Many: User <-> BlogPost
            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.AuthorId)
                .IsRequired(); // Ensures every blog post has an author

            // ✅ One-to-Many: BlogPost <-> Comment (Cascade Delete)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade); // Delete comments if a BlogPost is deleted

            // ✅ One-to-Many: BlogPost <-> Likes (Cascade Delete)
            modelBuilder.Entity<Like>()
                .HasOne(l => l.BlogPost)
                .WithMany(bp => bp.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade); // Delete likes if a BlogPost is deleted

            // ✅ One-to-Many: User <-> Likes (Restrict Deletion)
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting users with likes

            // ✅ Store MediaType Enum as String
            modelBuilder.Entity<Media>()
                .Property(m => m.Type)
                .HasConversion<string>()
                .IsRequired(); // Prevents NULL values
        }
    }
}
