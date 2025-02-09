using MazeGameBlazor.Components;
using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using MazeGameBlazor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MazeGameBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Environment.GetEnvironmentVariable("AdminPassword"));

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            // Add db context // connection string will be checked in environment variables and appsettings.json as fallback
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            // Add Identity
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Add authentication /authorization
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            // Custom services
            builder.Services.AddScoped<AuthService>();




            var app = builder.Build();


            // Add Default Roles and Admin User
            //using (var scope = app.Services.CreateScope())
            //{
            //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            //    // Add default roles
            //    if (!await roleManager.RoleExistsAsync("Admin"))
            //    {
            //        await roleManager.CreateAsync(new Role { Name = "Admin" });
            //    }
            //    if (!await roleManager.RoleExistsAsync("User"))
            //    {
            //        await roleManager.CreateAsync(new Role { Name = "User" });
            //    }

            //    // Add default admin user
            //    var adminUser = await userManager.FindByEmailAsync("a3emond@gmail.com");
            //    if (adminUser == null)
            //    {
            //        adminUser = new User
            //        {
            //            UserName = "admin",
            //            Email = "a3emond@gmail.com",
            //            EmailConfirmed = true
            //        };

            //        // Get password from environment variables
            //        var password = Environment.GetEnvironmentVariable("AdminPassword") ?? throw new Exception("Admin password not set in environment variables.");
            //        var result = await userManager.CreateAsync(adminUser, password);
            //        if (result.Succeeded)
            //        {
            //            await userManager.AddToRoleAsync(adminUser, "Admin");
            //        }
            //    }

            //}

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }

        
        
    }
}
