# Comment.cs

## Overview
The `Comment` class represents a user comment on a **BlogPost** in the MazeGameBlazor application.

## Table Structure

| Column Name  | Type            | Nullable | Description |
|-------------|----------------|----------|-------------|
| `Id`        | `int`           | ❌ No    | Primary Key (auto-incremented). |
| `Content`   | `string(800)`   | ❌ No    | Comment text (Max 800 chars). |
| `Author`    | `string(50)`    | ❌ No    | Name of the comment author (Max 50 chars). |
| `BlogPostId`| `int`           | ❌ No    | Foreign Key linking to `BlogPost`. |

## Relationships

### **BlogPost ⇄ Comment** (One-to-Many, Cascade Delete)
- A blog post **can** have multiple comments.
- When a blog post is deleted, **all associated comments are also deleted** (cascade delete).
- The `BlogPostId` is required for referential integrity.

## Example Usage

### **Creating a New Comment**
```csharp
var newComment = new Comment
{
    Content = "This is my first comment!",
    Author = "John Doe",
    BlogPostId = 1 // Assuming BlogPost with ID=1 exists
};

dbContext.Comments.Add(newComment);
dbContext.SaveChanges();
