using MazeGameBlazor.Components;
using MazeGameBlazor.Database;
using MazeGameBlazor.Database.Models;
using MazeGameBlazor.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using _Imports = MazeGameBlazor.Client._Imports;

namespace MazeGameBlazor;

public class Program
{
    public static void Main(string[] args)
    {
        //Console.WriteLine(Environment.GetEnvironmentVariable("AdminPassword"));

        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5000); // Allow external access on port 5000
        });

        // Register API controllers
        builder.Services.AddControllers();

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

        // Add HttpClientFactory
        builder.Services.AddHttpClient();

        // adjust limits for file upload
        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // Set to 100 MB or your desired size
        });
        builder.Services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });
        
        // Add SignalR
        builder.Services.AddSignalR();


        // CORS policy

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                policy =>
                {
                    policy.WithOrigins("https://maze.aedev.pro")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        //builder.Services.AddCors(options =>
        //{
        //    options.AddDefaultPolicy(builder =>
        //    {
        //        builder.AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader();
        //    });
        //});

        // Custom services
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<BlogService>();


        var app = builder.Build();

        app.UseWebSockets();
        app.UseCors();
        app.UseRouting();
        app.MapControllers();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new CompositeFileProvider(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")), // Default wwwroot
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                    "uploads")) // Uploads folder
            ),
            RequestPath = "/uploads",
            ServeUnknownFileTypes = true, // Allow serving video files
            DefaultContentType = "application/octet-stream", // Handle different formats
            OnPrepareResponse = ctx => { ctx.Context.Response.Headers.Append("Accept-Ranges", "bytes"); }
        });

        //// add a default user with guest as id
        //using (var scope = app.Services.CreateScope())
        //{
        //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        //    var user = await userManager.FindByIdAsync("guest");
        //    if (user == null)
        //    {
        //        user = new User
        //        {
        //            Id = "guest",
        //            UserName = "guest",
        //            Email = "guest@guest.ca"
        //        };
        //        await userManager.CreateAsync(user);
        //    }
        //}

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
            .AddAdditionalAssemblies(typeof(_Imports).Assembly);

        app.Run();
    }
}