using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

public class BlogService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly AppDbContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BlogService" /> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="authStateProvider">The authentication state provider.</param>
    public BlogService(AppDbContext context, AuthenticationStateProvider authStateProvider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
    }

    /// <summary>
    ///     Retrieves all blog posts with their associated media and author.
    /// </summary>
    /// <returns>A list of all blog posts.</returns>
    public async Task<List<BlogPost>> GetAllBlogsAsync()
    {
        return await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    ///     Retrieves a single blog post by ID with associated media and author.
    /// </summary>
    /// <param name="id">The ID of the blog post.</param>
    /// <returns>The corresponding <see cref="BlogPost" /> or null if not found.</returns>
    public async Task<BlogPost?> GetBlogByIdAsync(int id)
    {
        return await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    ///     Gets the latest blog post.
    /// </summary>
    /// <returns>The most recent <see cref="BlogPost" />, or null if no posts exist.</returns>
    public async Task<BlogPost?> GetLatestBlogPostAsync()
    {
        return await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Media)
            .OrderByDescending(b => b.CreatedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    ///     Creates a new blog post.
    /// </summary>
    /// <param name="newBlog">The blog post details.</param>
    /// <returns>The created <see cref="BlogPost" />.</returns>
    public async Task<BlogPost> CreateBlogAsync(BlogPostDto newBlog)
    {
        if (newBlog == null) throw new ArgumentNullException(nameof(newBlog));
        if (string.IsNullOrWhiteSpace(newBlog.Title))
            throw new ArgumentException("Title cannot be empty.", nameof(newBlog.Title));
        if (string.IsNullOrWhiteSpace(newBlog.Content))
            throw new ArgumentException("Content cannot be empty.", nameof(newBlog.Content));

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
    ///     Likes a blog post.
    /// </summary>
    /// <param name="blogPostId">The ID of the blog post to like.</param>
    /// <returns>The updated like count.</returns>
    public async Task<int> LikePostAsync(int blogPostId)
    {
        var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
        if (blogPost == null)
            throw new KeyNotFoundException($"Blog post with ID {blogPostId} not found.");

        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User.Identity?.Name ?? "guest";

        blogPost.LikeCount++;
        await _context.SaveChangesAsync();

        return blogPost.LikeCount;
    }

    /// <summary>
    ///     Retrieves all comments for a given blog post.
    /// </summary>
    /// <param name="blogPostId">The blog post ID.</param>
    /// <returns>A list of comments for the specified post.</returns>
    public async Task<List<Comment>> GetCommentsAsync(int blogPostId)
    {
        return await _context.Comments
            .Where(c => c.BlogPostId == blogPostId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    /// <summary>
    ///     Adds a comment to a blog post.
    /// </summary>
    /// <param name="blogPostId">The blog post ID.</param>
    /// <param name="author">The author of the comment.</param>
    /// <param name="content">The comment content.</param>
    /// <returns>The newly created comment.</returns>
    public async Task<Comment> AddCommentAsync(int blogPostId, string author, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty.", nameof(content));

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

/// <summary>
///     DTO for creating a new blog post.
/// </summary>
public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
}

/// <summary>
///     DTO for media upload results.
/// </summary>
public class MediaUploadResult
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
}