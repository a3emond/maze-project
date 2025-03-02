using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Identity;
using static MazeGameBlazor.Components.Pages.Login;
using static MazeGameBlazor.Components.Pages.Register;

namespace MazeGameBlazor.Services;

/// <summary>
///     Service responsible for handling user authentication, registration, and logout.
/// </summary>
public class AuthService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthService" /> class.
    /// </summary>
    /// <param name="userManager">User manager service for handling user-related operations.</param>
    /// <param name="signInManager">Sign-in manager service for handling authentication.</param>
    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    ///     Registers a new user asynchronously.
    /// </summary>
    /// <param name="model">User registration details.</param>
    /// <returns>IdentityResult indicating success or failure.</returns>
    public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
    {
        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded) await _userManager.AddToRoleAsync(user, "User");
        return result;
    }

    /// <summary>
    ///     Logs in a user asynchronously.
    /// </summary>
    /// <param name="model">User login details.</param>
    /// <returns>SignInResult indicating success or failure.</returns>
    public async Task<SignInResult> LoginUserAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return SignInResult.Failed; // User does not exist

        return await _signInManager.PasswordSignInAsync(
            user.UserName,
            model.Password,
            false,
            false
        );
    }

    /// <summary>
    ///     Logs out the current user asynchronously.
    /// </summary>
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}