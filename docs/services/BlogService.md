# BlogService

## Overview
The `BlogService` class provides functionality for managing blog posts, including retrieving, creating, liking, and commenting on posts. It integrates with the database using Entity Framework Core and supports media attachments.

## Dependencies
This service depends on:
- `AppDbContext`: The database context for accessing blog-related tables.
- `AuthenticationStateProvider`: Used to manage authentication (optional).
- `Microsoft.EntityFrameworkCore`: For database operations.

## Constructor

```csharp
public BlogService(AppDbContext context, AuthenticationStateProvider authStateProvider)
public BlogService(AppDbContext context)
public BlogService()
```
- **Parameters:**
  - `context`: Instance of `AppDbContext` for database access.
  - `authStateProvider` (optional): Authentication provider for managing user authentication.

## Methods

### GetAllBlogsAsync

```csharp
public async Task<List<BlogPost>> GetAllBlogsAsync()
```
- **Description**: Retrieves all blog posts with their associated media and author.
- **Returns**: A list of `BlogPost` objects, ordered by `CreatedAt` in descending order.

### GetBlogByIdAsync

```csharp
public async Task<BlogPost?> GetBlogByIdAsync(int id)
```
- **Description**: Retrieves a single blog post by its ID, including associated media and author.
- **Parameters**:
  - `id`: The ID of the blog post.
- **Returns**: A `BlogPost` object if found, otherwise `null`.

### GetLatestBlogPostAsync

```csharp
public async Task<BlogPost?> GetLatestBlogPostAsync()
```
- **Description**: Fetches the latest blog post based on the `CreatedAt` timestamp.
- **Returns**: The latest `BlogPost` object or `null` if no posts exist.

### CreateBlogAsync

```csharp
public async Task<BlogPost> CreateBlogAsync(BlogPostDto newBlog)
```
- **Description**: Creates a new blog post (without media).
- **Parameters**:
  - `newBlog`: A `BlogPostDto` containing title, content, and author ID.
- **Returns**: The newly created `BlogPost` object.
- **Exceptions**:
  - Throws `ArgumentException` if the title or content is empty.

### LikePostAsync

```csharp
public async Task<int> LikePostAsync(int blogPostId)
```
- **Description**: Increments the like count of a blog post.
- **Parameters**:
  - `blogPostId`: The ID of the blog post to be liked.
- **Returns**: The updated like count of the blog post.
- **Exceptions**:
  - Throws `KeyNotFoundException` if the blog post does not exist.

### GetCommentsAsync

```csharp
public async Task<List<Comment>> GetCommentsAsync(int blogPostId)
```
- **Description**: Retrieves all comments for a given blog post.
- **Parameters**:
  - `blogPostId`: The ID of the blog post.
- **Returns**: A list of `Comment` objects, ordered by ID in descending order.

### AddCommentAsync

```csharp
public async Task<Comment> AddCommentAsync(int blogPostId, string author, string content)
```
- **Description**: Adds a new comment to a blog post.
- **Parameters**:
  - `blogPostId`: The ID of the blog post to comment on.
  - `author`: The name of the comment author.
  - `content`: The comment text.
- **Returns**: The newly added `Comment` object.
- **Exceptions**:
  - Throws `ArgumentException` if the comment content is empty.

## Data Transfer Objects (DTOs)

### BlogPostDto

```csharp
public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string AuthorId { get; set; } = "";
}
```
- **Purpose**: Used when creating a new blog post.

### MediaUploadResult

```csharp
public class MediaUploadResult
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
}
```
- **Purpose**: Represents the result of a media upload, including URL and type.

## Example Usage

```csharp
var blogService = new BlogService(context);

// Create a new blog post
var newBlog = await blogService.CreateBlogAsync(new BlogPostDto 
{
    Title = "First Blog",
    Content = "This is my first blog post.",
    AuthorId = "user123"
});

// Retrieve all blogs
var blogs = await blogService.GetAllBlogsAsync();

// Like a blog post
var updatedLikes = await blogService.LikePostAsync(newBlog.Id);

// Add a comment
var comment = await blogService.AddCommentAsync(newBlog.Id, "User1", "Great post!");
```

## Notes
- **Media Handling**: Blog posts do not contain media directly. Media is stored separately and linked.
- **Authentication**: Currently, liking a post defaults to `"guest"`. Authentication handling will be implemented.
- **Performance**: Uses `Include` for eager loading to minimize database queries.

## License
This project follows the standard licensing agreement defined for MazeGameBlazor.
