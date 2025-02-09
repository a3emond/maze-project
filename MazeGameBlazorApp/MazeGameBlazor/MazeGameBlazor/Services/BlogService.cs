using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using Microsoft.EntityFrameworkCore;

public class BlogService
{
    private readonly AppDbContext _context;

    public BlogService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all blog posts with their associated media and author.
    /// </summary>
    public async Task<List<BlogPost>> GetAllBlogsAsync()
    {
        var blogPosts = await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media) // Fetch related media
            .ToListAsync();

        // Convert media type from string to enum
        foreach (var post in blogPosts)
        {
            foreach (var media in post.Media)
            {
                if (Enum.TryParse(typeof(MediaType), media.Type.ToString(), true, out var parsedType))
                {
                    media.Type = (MediaType)parsedType;
                }
                else
                {
                    media.Type = MediaType.Image; // Default fallback
                }
            }
        }

        return blogPosts;
    }

    /// <summary>
    /// Creates a new blog post and saves it to the database.
    /// </summary>
    public async Task CreateBlogAsync(BlogPostDto newBlog)
    {
        // Convert DTO to BlogPost entity
        var blogPost = new BlogPost
        {
            Title = newBlog.Title,
            Content = newBlog.Content,
            AuthorId = newBlog.AuthorId, // Ensure this comes from the authenticated user
            CreatedAt = DateTime.UtcNow,
            Media = new List<Media>
            {
                new Media
                {
                    Url = newBlog.MediaUrl,
                    ThumbnailUrl = newBlog.ThumbnailUrl ?? newBlog.MediaUrl, // Use the same URL if no thumbnail
                    Type = newBlog.MediaType
                }
            }
        };

        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Uploads media and returns its URL and type.
    /// </summary>
    public async Task<MediaUploadResult> UploadMediaAsync(Stream fileStream, string fileName, string contentType)
    {
        // Simulate file upload (replace this with actual storage logic)
        var filePath = Path.Combine("wwwroot/uploads", fileName);

        using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(file);
        }

        return new MediaUploadResult
        {
            Url = $"/uploads/{fileName}",
            Type = DetermineMediaType(contentType) // Infer media type based on content type
        };
    }

    /// <summary>
    /// Determines the media type based on content type.
    /// </summary>
    private MediaType DetermineMediaType(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" or "image/png" or "image/gif" => MediaType.Image,
            "video/mp4" or "video/webm" => MediaType.Video,
            "audio/mpeg" or "audio/wav" => MediaType.Audio,
            _ => MediaType.Document
        };
    }
}

/// <summary>
/// DTO for creating blog posts.
/// </summary>
public class BlogPostDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty; // To associate with the logged-in user
    public string MediaUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
}

/// <summary>
/// Represents the result of a media upload.
/// </summary>
public class MediaUploadResult
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
}
