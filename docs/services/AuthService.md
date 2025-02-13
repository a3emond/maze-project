# AuthService

## Overview
The `AuthService` class is responsible for handling user authentication, registration, and logout in the MazeGameBlazor application. It utilizes ASP.NET Core Identity to manage users and their authentication states.

## Dependencies
This service depends on the following ASP.NET Core Identity services:
- `UserManager<User>`: Handles user-related operations like creation and role assignment.
- `SignInManager<User>`: Manages user authentication and sign-in functionality.

## Constructor

```csharp
public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
```
- **Parameters:**
  - `userManager`: Instance of `UserManager<User>` for user management.
  - `signInManager`: Instance of `SignInManager<User>` for handling authentication.

## Methods

### RegisterUserAsync

```csharp
public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
```
- **Description**: Registers a new user asynchronously.
- **Parameters**:
  - `model`: An instance of `RegisterModel` containing user registration details.
- **Returns**: `IdentityResult` indicating the success or failure of the registration.

### LoginUserAsync

```csharp
public async Task<SignInResult> LoginUserAsync(LoginModel model)
```
- **Description**: Logs in a user asynchronously.
- **Parameters**:
  - `model`: An instance of `LoginModel` containing login credentials.
- **Returns**: `SignInResult` indicating success or failure.

### LogoutAsync

```csharp
public async Task LogoutAsync()
```
- **Description**: Logs out the currently authenticated user asynchronously.
- **Returns**: A `Task` representing the asynchronous operation.

## Example Usage

```csharp
var authService = new AuthService(userManager, signInManager);

// Register a user
var registerResult = await authService.RegisterUserAsync(new RegisterModel 
{
    UserName = "testuser",
    Email = "test@example.com",
    Password = "SecurePassword123!"
});

// Login a user
var loginResult = await authService.LoginUserAsync(new LoginModel 
{
    Email = "test@example.com",
    Password = "SecurePassword123!"
});

// Logout the user
await authService.LogoutAsync();
```

## Notes
- Email confirmation is automatically set to `true` upon registration. (Temporary)
- A newly registered user is assigned the "User" role by default.
- Authentication failures return `SignInResult.Failed`.

