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
        return await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media) // Fetch related media
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a single blog post by ID with associated media and author.
    /// </summary>
    public async Task<BlogPost?> GetBlogByIdAsync(int id)
    {
        return await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    /// Creates a new blog post without media (media is attached separately).
    /// </summary>
    public async Task<BlogPost> CreateBlogAsync(BlogPostDto newBlog)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(newBlog.Title))
            throw new ArgumentException("Blog post title cannot be empty.");
        if (string.IsNullOrWhiteSpace(newBlog.Content))
            throw new ArgumentException("Blog post content cannot be empty.");

        var blogPost = new BlogPost
        {
            Title = newBlog.Title,
            Content = newBlog.Content,
            AuthorId = newBlog.AuthorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();

        return blogPost;
    }

    /// <summary>
    /// Retrieves all available media that is not attached to any blog post.
    /// </summary>
    public async Task<List<Media>> GetAvailableMediaAsync()  // TODO: use as a fallback with http request
    {
        return await _context.Media
            .ToListAsync();
    }

    /// <summary>
    /// Uploads media independently and returns its details.
    /// </summary>
    /// **** not used in the current implementation **** ---> http request in mediaController
    public async Task<MediaUploadResult> UploadMediaAsync(Stream fileStream, string fileName, string contentType) // TODO: use as a fallback with http request
    {
        var uploadFolder = Path.Combine("wwwroot", "uploads");

        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadFolder, uniqueFileName);

        await using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(file);
        }

        var mediaType = DetermineMediaType(contentType);
        var media = new Media
        {
            Url = $"/uploads/{uniqueFileName}",
            Type = mediaType
        };

        _context.Media.Add(media);
        await _context.SaveChangesAsync();

        return new MediaUploadResult
        {
            Id = media.Id,
            Url = media.Url,
            Type = media.Type
        };
    }

    /// <summary>
    /// Attaches selected media to an existing blog post.
    /// </summary>
    public async Task AttachMediaToBlogPostAsync(int blogPostId, List<int> mediaIds)
    {
        var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
        if (blogPost == null)
            throw new KeyNotFoundException($"Blog post with ID {blogPostId} not found.");

        var mediaList = await _context.Media
            .Where(m => mediaIds.Contains(m.Id))
            .ToListAsync();

        if (!mediaList.Any())
            throw new ArgumentException("No valid media found.");


        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a blog post and detaches its media.
    /// </summary>
    public async Task DeleteBlogAsync(int blogPostId)
    {
        var blogPost = await _context.BlogPosts
            .Include(b => b.Media)
            .FirstOrDefaultAsync(b => b.Id == blogPostId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog post with ID {blogPostId} not found.");

        // Keep media but detach from the blog post

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes orphaned media (not linked to any blog post).
    /// </summary>
    public async Task<int> CleanupOrphanedMediaAsync()
    {
        var orphanedMedia = await _context.Media
            .ToListAsync();

        if (!orphanedMedia.Any())
            return 0;

        _context.Media.RemoveRange(orphanedMedia);
        await _context.SaveChangesAsync();

        return orphanedMedia.Count;
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

// DTO for creating a new blog post
public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string AuthorId { get; set; } = "";
}

// DTO for uploading media
public class MediaUploadResult
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
}
