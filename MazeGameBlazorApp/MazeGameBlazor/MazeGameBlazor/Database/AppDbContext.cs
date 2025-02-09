using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MazeGameBlazor.Database
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Media> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity configuration

            // One-to-Many: User <-> BlogPost
            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.AuthorId);

            // One-to-Many: BlogPost <-> Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade); // Delete comments if a BlogPost is deleted

            // One-to-Many: BlogPost <-> Media
            modelBuilder.Entity<Media>(entity =>
            {
                // Store enum as a string in the database
                entity.Property(m => m.Type)
                      .HasConversion<string>();

                // Configure the one-to-many relationship with BlogPost
                entity.HasOne(m => m.BlogPost)
                      .WithMany(bp => bp.Media)
                      .HasForeignKey(m => m.BlogPostId);
            });

            // One-to-Many: BlogPost <-> Likes
            modelBuilder.Entity<Like>()
                .HasOne(l => l.BlogPost)
                .WithMany(bp => bp.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading delete to prevent conflicts

            // One-to-Many: User <-> Likes
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading delete to prevent conflicts
        }
    }
}
