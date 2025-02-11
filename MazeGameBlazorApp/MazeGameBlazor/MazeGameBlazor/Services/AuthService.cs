using System.Security.Claims;
using MazeGameBlazor.Database.Models;
using Microsoft.AspNetCore.Identity;
using static MazeGameBlazor.Components.Pages.Register;
using static MazeGameBlazor.Components.Pages.Login;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace MazeGameBlazor.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;



        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };

            await _userManager.CreateAsync(user, model.Password);
            // Add user to default role User
            await _userManager.AddToRoleAsync(user, "User");
            return IdentityResult.Success;
        }

        public async Task<SignInResult> LoginUserAsync(LoginModel model)
        {
            //display model.Email
            Console.WriteLine(model.Email);
            //display model.Password
            Console.WriteLine(model.Password);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                Console.WriteLine("\n\n\nuser does not exist\n\n\n");
                return SignInResult.Failed; // User does not exist
            }

            return await _signInManager.PasswordSignInAsync(
                user.UserName, // Use UserName instead of Email
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}