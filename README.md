
# AppDbContext.cs 
(Database -- Entity Framework -- Code First)

## Overview
`AppDbContext` is the main database context for the MazeGameBlazor application. It manages the Entity Framework Core (EF Core) configurations, entity relationships, and authentication setup.

## Inheritance
This class inherits from `IdentityDbContext<User, Role, string>` to integrate ASP.NET Identity for authentication and role management.

## Tables
| Table Name | Description |
|------------|------------|
| `BlogPosts` | Stores blog posts created by users. |
| `Comments` | Stores comments made on blog posts. |
| `Likes` | Stores likes associated with blog posts. |
| `Media` | Stores media files linked to blog posts. |

## Relationships
- **User ⇄ BlogPost** (One-to-Many)  
  - A user can author multiple blog posts.
  - Deleting a user does not delete their blog posts.
  
- **BlogPost ⇄ Comment** (One-to-Many, Cascade Delete)  
  - A blog post can have multiple comments.
  - When a blog post is deleted, its comments are also deleted.

- **BlogPost ⇄ Likes** (One-to-Many, Cascade Delete)  
  - A blog post can have multiple likes.
  - When a blog post is deleted, its likes are also deleted.

- **User ⇄ Likes** (One-to-Many, Restrict Deletion)  
  - A user can like multiple blog posts.
  - Users cannot be deleted if they have existing likes.



# MazeGameBlazorApp

## Overview
MazeGameBlazorApp is a Blazor WebAssembly-based project that includes a blog system with authentication, role-based access control, and media handling.

---

## Database Models

Below are the core models used in the database:

- [BlogPost.cs](MazeGameBlazorApp/MazeGameBlazor/MazeGameBlazor/Database/Models/BlogPost.cs) - Represents a blog post entity.
- [Comment.cs](MazeGameBlazorApp/MazeGameBlazor/MazeGameBlazor/Database/Models/Comment.cs) - Represents a comment linked to a blog post.
- [Like.cs](MazeGameBlazorApp/MazeGameBlazor/MazeGameBlazor/Database/Models/Like.cs) - Represents a like interaction on a blog post.
- [Media.cs](MazeGameBlazorApp/MazeGameBlazor/MazeGameBlazor/Database/Models/Media.cs) - Stores media files associated with blog posts.
- [Role.cs](MazeGameBlazorApp/MazeGameBlazor/MazeGameBlazor/Database/Models/Role.cs) - Defines user roles for authentication.
- [User.cs](MazeGameBlazorApp/MazeGameBlazor/MazeGameBlazor/Database/Models/User.cs) - Represents a registered user in the system.

---

## Identity & Role-Based Access Control

This project integrates **ASP.NET Core Identity**, which provides built-in authentication and role-based authorization.

### **What Identity Includes**
ASP.NET Core Identity is used for:
- User authentication (login, registration, password hashing).
- Role-based access control (Admin & User).
- Token management (if extended for APIs).
- Secure user data storage with `IdentityDbContext`.

The `User` model inherits from `IdentityUser`, allowing easy integration with Identity features.

### **Roles in the Application**
The project defines two main roles:

| Role   | Description |
|--------|------------|
| **Admin** | Full control over the system, including managing users, blog posts, and media. |
| **User**  | Can comment, like, and update their account informations, but has limited permissions. |

### **Role-Based Authorization**
Role-based access is enforced using `[Authorize]` attributes in Blazor components and API controllers.

#### **Restricting Access in Razor Components**
To restrict page access based on roles, use:

```razor
@attribute [Authorize(Roles = "Admin")]


## Configuration
- **Enum Storage:** `MediaType` enums are stored as `string` values in the database.
- **Identity Framework:** The `base.OnModelCreating(modelBuilder);` call ensures ASP.NET Identity settings are correctly applied.

## Setup and Usage
To register the database context in `Program.cs`, use:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
