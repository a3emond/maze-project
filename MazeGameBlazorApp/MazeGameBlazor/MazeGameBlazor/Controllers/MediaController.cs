using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _dbContext;

    public MediaController(IWebHostEnvironment environment, AppDbContext dbContext)
    {
        _environment = environment;
        _dbContext = dbContext;
    }

    //  Step 1: Upload Media Independently (No BlogPostId Required)
    [HttpPost("upload")]
    public async Task<IActionResult> UploadMedia(IFormFile file)
    {
        // Validate file
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            // Ensure uploads folder exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate a unique file name to prevent overwriting
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the uploaded file to the server
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Determine the media type
            var mediaType = DetermineMediaType(file.ContentType);

            // Save media details to database
            var media = new Media
            {
                Url = $"/uploads/{uniqueFileName}",
                Type = mediaType
            };

            _dbContext.Media.Add(media);
            await _dbContext.SaveChangesAsync();

            // Return uploaded media details
            return Ok(new
            {
                media.Id,
                media.Url,
                media.Type
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    //  Step 2: Retrieve All Available Media (For Media Selection)
    [HttpGet]
    public async Task<IActionResult> GetAvailableMedia()
    {
        var mediaList = await _dbContext.Media
            .ToListAsync();

        return Ok(mediaList);
    }

    //  Step 3: Attach Media to Blog Post (After Blog Post is Created)
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

    //  Helper: Determine Media Type
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

//  Model for Media Attachment Request
public class AttachMediaRequest
{
    public int BlogPostId { get; set; }
    public List<int> MediaIds { get; set; }
}
