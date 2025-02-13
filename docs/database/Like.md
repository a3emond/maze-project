# Like.cs

## Overview
The `Like` class represents a **like** interaction in the MazeGameBlazor application.  
A user can "like" a blog post, and each like is stored in this table.

## Table Structure

| Column Name  | Type    | Nullable | Description |
|-------------|--------|----------|-------------|
| `Id`        | `int`  | ❌ No    | Primary Key (auto-incremented). |
| `BlogPostId`| `int`  | ❌ No    | Foreign Key linking to `BlogPost`. |
| `UserId`    | `string` | ❌ No   | Foreign Key linking to `User`. |

## Relationships

### **BlogPost ⇄ Like** (One-to-Many, Cascade Delete)
- A blog post **can** have multiple likes.
- When a blog post is deleted, **all associated likes are also deleted**.

### **User ⇄ Like** (One-to-Many, Restrict Deletion)
- A user **can** like multiple blog posts.
- A user **cannot** be deleted if they have existing likes (`Restrict` delete behavior).

## Example Usage

### **Creating a Like**
```csharp
var newLike = new Like
{
    BlogPostId = 1, // Assuming BlogPost with ID=1 exists
    UserId = user.Id // Assuming `user` is an existing User entity
};

dbContext.Likes.Add(newLike);
dbContext.SaveChanges();
