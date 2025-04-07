using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MazeGameBlazor.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaController" /> class.
    /// </summary>
    /// <param name="environment">Web hosting environment.</param>
    /// <param name="dbContext">Database context.</param>
    public MediaController(IWebHostEnvironment environment, AppDbContext dbContext)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    ///     Streams a media file for playback.
    /// </summary>
    /// <param name="filename">The name of the media file.</param>
    /// <returns>The media file stream or a not found result.</returns>
    [HttpGet("stream/{filename}")]
    public async Task<IActionResult> StreamMedia(string filename)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "uploads", filename);
        if (!System.IO.File.Exists(filePath))
            return NotFound("Media file not found.");

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(stream, "video/mp4", true); // Enables seeking
    }

    /// <summary>
    ///     Uploads a media file to the server and stores metadata in the database.
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    /// <returns>Details of the uploaded media.</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMedia(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var mediaType = DetermineMediaType(file.ContentType);

            var media = new Media
            {
                Url = $"/uploads/{uniqueFileName}",
                Type = mediaType
            };

            _dbContext.Media.Add(media);
            await _dbContext.SaveChangesAsync();

            return Ok(new { media.Id, media.Url, media.Type });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    ///     Retrieves all media files available in the database.
    /// </summary>
    /// <returns>A list of media objects.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAvailableMedia()
    {
        var mediaList = await _dbContext.Media.ToListAsync();
        return Ok(mediaList);
    }

    /// <summary>
    ///     Attaches media files to a blog post.
    /// </summary>
    /// <param name="request">The request containing the blog post ID and media IDs.</param>
    /// <returns>A success message if media is attached.</returns>
    [HttpPost("attach")]
    public async Task<IActionResult> AttachMediaToBlogPost([FromBody] AttachMediaRequest request)
    {
        var blogPost = await _dbContext.BlogPosts.FindAsync(request.BlogPostId);
        if (blogPost == null)
            return NotFound("Blog post not found.");

        var mediaList = await _dbContext.Media
            .Where(m => request.MediaIds.Contains(m.Id))
            .ToListAsync();

        if (!mediaList.Any())
            return NotFound("No valid media found.");

        await _dbContext.SaveChangesAsync();
        return Ok("Media successfully attached to blog post.");
    }

    /// <summary>
    ///     Determines the type of media based on the content type.
    /// </summary>
    /// <param name="contentType">The MIME type of the file.</param>
    /// <returns>The corresponding media type.</returns>
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
///     Model representing a request to attach media to a blog post.
/// </summary>
public class AttachMediaRequest
{
    public int BlogPostId { get; set; }
    public List<int> MediaIds { get; set; } = new();
}