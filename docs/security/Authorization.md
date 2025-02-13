# Authentication and Authorization in Blazor with Claims, UserManager, SignInManager, and AuthenticationStateProvider

## Overview
Blazor authentication and authorization are built on ASP.NET Core Identity, leveraging `UserManager`, `SignInManager`, and `AuthenticationStateProvider` to manage user authentication, claims, roles, and policies. Authorization can be implemented using role-based or policy-based approaches.

---

## 1. Setting Up Authentication in Blazor
To enable authentication in a Blazor Server or WebAssembly application, configure authentication in `Program.cs`:

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddIdentityCookies();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
});
```

In a **Blazor Server** app, also add:
```csharp
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
```

For **Blazor WebAssembly**, ensure `AddApiAuthorization()` is used.

---

## 2. UserManager and SignInManager
### UserManager
The `UserManager<TUser>` service manages user-related operations such as creating, deleting, updating users, and handling claims/roles.

#### Example: Retrieve User by Email
```csharp
var user = await _userManager.FindByEmailAsync("user@example.com");
```

#### Example: Add a Claim to a User
```csharp
await _userManager.AddClaimAsync(user, new Claim("Department", "IT"));
```

### SignInManager
The `SignInManager<TUser>` service manages authentication, including signing in/out and checking credentials.

#### Example: Sign In a User
```csharp
var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
if (result.Succeeded)
{
    // Redirect or perform actions after successful login
}
```

#### Example: Sign Out
```csharp
await _signInManager.SignOutAsync();
```

---

## 3. AuthenticationStateProvider
The `AuthenticationStateProvider` provides authentication state information to the Blazor app.

### Custom AuthenticationStateProvider
To extend authentication logic, create a custom `AuthenticationStateProvider`:

```csharp
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _httpContextAccessor.HttpContext.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var claimsPrincipal = new ClaimsPrincipal(user);
            return new AuthenticationState(claimsPrincipal);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
```

Register it in `Program.cs`:
```csharp
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
```

---

## 4. Checking User Claims in Blazor
Use `AuthenticationStateProvider` in a Blazor component to check user authentication and claims.

```razor
@inject AuthenticationStateProvider AuthenticationStateProvider
@code {
    private ClaimsPrincipal _user;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _user = authState.User;
    }
}

@if (_user.Identity.IsAuthenticated)
{
    <p>Welcome, @_user.Identity.Name!</p>
    <p>Your Role: @_user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value</p>
}
else
{
    <p>You are not logged in.</p>
}
```

---

## 5. Role-Based Authorization
To restrict access to users based on roles:

### Assigning a Role to a User
```csharp
await _userManager.AddToRoleAsync(user, "Admin");
```

### Checking User Role in Code
```csharp
if (await _userManager.IsInRoleAsync(user, "Admin"))
{
    // Perform admin-specific actions
}
```

---

## 6. Policy-Based Authorization
Authorization policies allow fine-grained control beyond roles.

### Defining an Authorization Policy
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
});
```

### Applying Policy to a Component
```razor
@attribute [Authorize(Policy = "RequireManagerRole")]
```

### Applying Policy in Code
```csharp
var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
var result = await authorizationService.AuthorizeAsync(user, "RequireManagerRole");
if (result.Succeeded)
{
    // Allow access
}
```

---

## 7. Enforcing Authorization in Blazor
To enforce authorization, use `AuthorizeView` or `[Authorize]`.

### Example: Restricting Component Access
```razor
<AuthorizeView>
    <Authorized>
        <p>Welcome, authenticated user!</p>
    </Authorized>
    <NotAuthorized>
        <p>Please log in.</p>
    </NotAuthorized>
</AuthorizeView>
```

### Example: Restricting Pages
Add `[Authorize]` to restrict access:
```razor
@page "/admin"
@attribute [Authorize(Roles = "Admin")]
<h3>Admin Dashboard</h3>
```

---

## Conclusion
Blazor authentication and authorization integrate `UserManager`, `SignInManager`, and `AuthenticationStateProvider` to manage users, roles, and policies effectively. Custom authentication logic can be implemented using a custom `AuthenticationStateProvider`, while authorization is controlled using roles, claims, and policies.
