using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;

public class BlogService
{
    private readonly AppDbContext _context;
    private readonly AuthenticationStateProvider _authStateProvider;

    public BlogService(AppDbContext context, AuthenticationStateProvider authStateProvider)
    {
        _context = context;
        _authStateProvider = authStateProvider;
    }

    public BlogService(AppDbContext context)
    {
        _context = context;
    }

    public BlogService()
    {
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

    // get latest blogpost
    public async Task<BlogPost?> GetLatestBlogPostAsync()
    {
        return await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync();
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
    /// like a blog post.
    /// </summary>
    public async Task<int> LikePostAsync(int blogPostId)
    {
        // TODO: handle Authentication and get the user ID

        var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
        if (blogPost == null)
            throw new KeyNotFoundException($"Blog post with ID {blogPostId} not found.");

        string userId = "guest"; // Default value for guest users

        var like = new Like
        {
            BlogPostId = blogPostId,
            UserId = userId
        };

        blogPost.LikeCount++; // Increment like count
        await _context.SaveChangesAsync();

        return blogPost.LikeCount;
    }



    /// <summary>
    ///  Gets all comments for a blog post.
    /// </summary>
    public async Task<List<Comment>> GetCommentsAsync(int blogPostId)
    {
        return await _context.Comments
            .Where(c => c.BlogPostId == blogPostId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    /// <summary>
    ///  Adds a new comment to a blog post.
    /// </summary>
    public async Task<Comment> AddCommentAsync(int blogPostId, string author, string content)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty.");

        var comment = new Comment
        {
            BlogPostId = blogPostId,
            Author = author,
            Content = content
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
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
