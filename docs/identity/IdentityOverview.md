# Identity Overview

## What is ASP.NET Core Identity?
ASP.NET Core Identity is a membership system that provides authentication and authorization functionality. It allows applications to:
✔ Manage users, passwords, roles, and claims.
✔ Handle authentication with cookies or external providers (Google, Facebook, etc.).
✔ Implement **role-based access control (RBAC)**.
✔ Secure user data using password hashing and token management.

---

## **Identity Integration in the Project**
This project integrates Identity with `IdentityDbContext` to provide authentication and role-based authorization.

```csharp
public class AppDbContext : IdentityDbContext<User, Role, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
```
✔ **`IdentityDbContext<User, Role, string>`** extends Entity Framework to store user and role information.  
✔ `User` represents registered users, and `Role` represents different permission levels.

---

## **User & Role Models**
### **User Model (`User.cs`)**
```csharp
public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```
✔ Inherits from `IdentityUser`, which includes fields like `Email`, `PasswordHash`, and `UserName`.  
✔ **Custom properties** like `CreatedAt` can be added.

### **Role Model (`Role.cs`)**
```csharp
public class Role : IdentityRole
{
    // This class allows extending roles in the future if needed
}
```
✔ Inherits from `IdentityRole`, which contains role names and related permissions.  
✔ Can be extended with additional properties if needed.

---

## **Role-Based Access Control (RBAC)**
This project defines two main roles:

| Role   | Description |
|--------|------------|
| **Admin** | Full control over users, posts, and media. |
| **User**  | Can interact with content but has limited permissions. |

**Restricting Page Access with `[Authorize]`**
```razor
@attribute [Authorize(Roles = "Admin")]
```
✔ Ensures that only **admins** can access the page or API endpoint.

---

## **User Authentication Process**
1. **User Registration:**
   - A new user is created and assigned a role (default: `User`).
2. **Login & Authentication:**
   - The system verifies credentials and issues an authentication cookie.
3. **Role-Based Authorization:**
   - Users can access only the pages allowed by their role.

---

## **Identity Configuration in `Program.cs`**
To register Identity services, update `Program.cs`:

```csharp
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
```
✔ Registers **Identity services** and links them to `AppDbContext`.  
✔ Enables **token-based security features** (e.g., email confirmation, password reset).

---

## **Common Issues & Fixes**
| **Issue**  | **Solution**  |
|------------|--------------|
| **User cannot log in** | Ensure passwords are properly hashed and stored in the database. |
| **Role-based access not working** | Verify `[Authorize(Roles = "Admin")]` is correctly applied. |
| **User roles not persisting** | Ensure roles are properly seeded in the database. |

---

## **Summary**
✔ **ASP.NET Core Identity** provides authentication and authorization management.  
✔ **Users & Roles** are managed using `IdentityUser` and `IdentityRole`.  
✔ **Role-based access control (RBAC)** ensures secure navigation and access restrictions.  
✔ **Identity services** are configured in `Program.cs` to enable security features.

---
