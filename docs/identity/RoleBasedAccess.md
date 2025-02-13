# Role-Based Access Control (RBAC)

## **Overview**
Role-Based Access Control (RBAC) is a security model that restricts system access based on user roles. This ensures that only authorized users can access specific functionalities.

✔ **Users are assigned roles (e.g., Admin, User, etc.)**
✔ **Permissions are granted based on roles, not individual users.**
✔ **Enhances security by enforcing access restrictions.**

---

## **Roles in the Application**
This project defines two primary roles:

| Role   | Description |
|--------|------------|
| **Admin** | Full control over users, blog posts, and media. |
| **User**  | Can interact with content but has limited permissions. |

Additional roles can be added by extending the `Role` model.

---

## **Defining Roles in Identity**
Roles are managed using `IdentityRole` in ASP.NET Core Identity:

```csharp
public class Role : IdentityRole
{
    // Extend this class to add additional role properties if needed.
}
```
✔ The `Role` model inherits from `IdentityRole`, allowing for **role-based authentication.**

---

## **Enforcing Role-Based Access in Blazor**
Blazor uses the `[Authorize]` attribute to restrict access to specific components or pages.

### **Restricting Access to Admins Only**
```razor
@attribute [Authorize(Roles = "Admin")]
```
✔ Only users assigned the **Admin** role can access this page.

### **Restricting Sections Inside a Page**
```razor
@if (user.IsInRole("Admin"))
{
    <button class="admin-button">Manage Users</button>
}
```
✔ Conditionally renders content **only for Admins.**

---

## **Assigning Roles to Users**
Roles can be assigned programmatically when creating a new user:

```csharp
var user = new User { UserName = "johndoe", Email = "johndoe@example.com" };
await userManager.CreateAsync(user, "SecurePassword123!");
await userManager.AddToRoleAsync(user, "Admin");
```
✔ The **Admin** role is assigned to the newly created user.

---

## **Retrieving User Roles in Blazor**
You can check a user's role dynamically using authentication state.

```razor
@inject AuthenticationStateProvider AuthStateProvider

@code {
    private bool isAdmin;
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        isAdmin = user.IsInRole("Admin");
    }
}
```
✔ Checks if the current user **has the Admin role.**

---

## **Role-Based Authorization in API Controllers**
If securing API endpoints, use `[Authorize]` in controllers:

```csharp
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    // Only accessible by Admins
    [HttpGet("dashboard")]
    public IActionResult GetAdminDashboard()
    {
        return Ok("Welcome, Admin!");
    }
}
```
✔ Ensures that only **authenticated Admins** can access the API route.

---

## **Common Issues & Fixes**

| **Issue**  | **Solution**  |
|------------|--------------|
| **Users cannot access restricted pages** | Ensure `[Authorize(Roles = "Admin")]` is correctly applied. |
| **Roles are not persisting** | Verify roles are assigned correctly using `AddToRoleAsync`. |
| **User roles are not refreshing** | Ensure users **log out and back in** to refresh their roles. |

---

## **Summary**
✔ **RBAC enhances security** by restricting access based on roles.  
✔ **Roles are managed using IdentityRole and enforced via `[Authorize]` attributes.**  
✔ **Blazor components dynamically check roles to show/hide UI elements.**  
✔ **Secure API controllers** by restricting access using `[Authorize(Roles = "Admin")]`.  

---
