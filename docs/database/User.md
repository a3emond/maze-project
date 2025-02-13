# User.cs

## Overview
The `User` class represents **a registered user** in the Identity system.  
It **inherits from `IdentityUser`**, which provides built-in authentication and role-based access control.

## Table Structure (from IdentityUser)

| Column Name    | Type    | Nullable | Description |
|---------------|--------|----------|-------------|
| `Id`         | `string` | ❌ No   | Unique identifier for the user. |
| `UserName`   | `string` | ✅ Yes  | The username for login/authentication. |
| `Email`      | `string` | ✅ Yes  | The user's email address. |
| `PasswordHash` | `string` | ✅ Yes | Hashed password stored securely. |
| `CreatedAt`  | `DateTime` | ❌ No  | Timestamp of account creation (default: `DateTime.Now`). |

## Relationships

### **User ⇄ BlogPost** (One-to-Many)
- A user **can** create multiple blog posts.
- Deleting a user **does not delete** their blog posts.

### **User ⇄ Like** (One-to-Many)
- A user **can** like multiple blog posts.
- A user **cannot be deleted** if they have existing likes (`Restrict` delete behavior).

## Example Usage

### **Creating a New User (via Identity)**
```csharp
var newUser = new User
{
    UserName = "johndoe",
    Email = "johndoe@example.com",
    CreatedAt = DateTime.UtcNow
};

var result = await userManager.CreateAsync(newUser, "SecurePassword123!");
if (result.Succeeded)
{
    await userManager.AddToRoleAsync(newUser, "User");
}
