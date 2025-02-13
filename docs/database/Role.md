# Role.cs

## Overview
The `Role` class represents a **user role** in the Identity system.  
It **inherits from `IdentityRole`**, which provides built-in role-based authentication.

## Purpose
This class allows **customizing Identity roles** if needed in the future.  
Currently, it does not contain additional properties but can be extended.

## Table Structure (from IdentityRole)

| Column Name    | Type    | Nullable | Description |
|---------------|--------|----------|-------------|
| `Id`         | `string` | ❌ No   | Unique identifier for the role. |
| `Name`       | `string` | ✅ Yes  | Name of the role (e.g., "Admin", "User"). |
| `NormalizedName` | `string` | ✅ Yes  | Uppercase version of `Name` for searches. |
| `ConcurrencyStamp` | `string` | ✅ Yes  | Used for concurrency tracking. |

## Roles in the Application

| Role   | Description |
|--------|------------|
| **Admin** | Full control over the system, including managing users, blog posts, and media. |
| **User**  | Can interact with blog posts (comment, like, upload media), but has limited permissions. |

## Example Usage

### **Adding a New Role in Code**
```csharp
var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
await roleManager.CreateAsync(new Role { Name = "Admin" });
await roleManager.CreateAsync(new Role { Name = "User" });
