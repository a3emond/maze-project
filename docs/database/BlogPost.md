# BlogPost.cs

## Overview
The `BlogPost` class represents a blog post entity in the **MazeGameBlazor** database.  
It stores information about the post, including title, content, author, media, comments, and likes.

## Table Structure

| Column Name  | Type             | Nullable | Description |
|-------------|----------------|----------|-------------|
| `Id`        | `int`           | ❌ No    | Primary Key (auto-incremented). |
| `Title`     | `string(100)`   | ❌ No    | Title of the blog post (Max 100 chars). |
| `Content`   | `string(1000)`  | ❌ No    | Main content of the blog post (Max 1000 chars). |
| `CreatedAt` | `DateTime`      | ❌ No    | Timestamp when the post was created (default: `DateTime.Now`). |
| `LikeCount` | `int`           | ❌ No    | Number of likes associated with this post (default: `0`). |
| `AuthorId`  | `string`        | ❌ No    | Foreign key linking to the `User` entity. |
| `MediaId`   | `int?`          | ✅ Yes   | Foreign key linking to `Media` (nullable). |

## Relationships

### **User ⇄ BlogPost** (One-to-Many)
- A user can author multiple blog posts.
- Deleting a user does **not** delete their posts (handled separately).

### **BlogPost ⇄ Media** (One-to-One, Optional)
- A blog post **can** have one attached media item, but it is **optional**.

### **BlogPost ⇄ Comment** (One-to-Many, Cascade Delete)
- A blog post **can** have multiple comments.
- Deleting a blog post **removes all associated comments**.

### **BlogPost ⇄ Like** (One-to-Many, Cascade Delete)
- A blog post **can** have multiple likes.
- Deleting a blog post **removes all associated likes**.

## Example Usage

### **Creating a New BlogPost**
```csharp
var newPost = new BlogPost
{
    Title = "My First Blog Post",
    Content = "This is the content of my first blog post!",
    AuthorId = user.Id,  // Assume `user` is an existing User entity
    CreatedAt = DateTime.UtcNow
};

dbContext.BlogPosts.Add(newPost);
dbContext.SaveChanges();
