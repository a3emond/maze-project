using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using MazeGameBlazor.Database.Models;
using System.Threading.Tasks;

namespace MazeGameBlazor.Components.Pages
{
    public class CreateBlogBase : ComponentBase
    {
        [Inject] protected NavigationManager Navigation { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] protected BlogService BlogService { get; set; }

        protected BlogPostDto NewBlogPost { get; set; } = new BlogPostDto();
        protected bool IsAdmin { get; set; } = false;
        protected string UploadedFileName { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Check if the user is in the "Admin" role
            IsAdmin = user.IsInRole("Admin");
        }

        protected async Task HandleValidSubmit()
        {
            if (!IsAdmin)
            {
                return;
            }

            // Handle blog creation logic
            await BlogService.CreateBlogAsync(NewBlogPost);

            // Redirect or notify user
            Navigation.NavigateTo("/blog");
        }

        protected async Task HandleFileChange(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file != null)
            {
                // Upload file to the server and get the URL
                UploadedFileName = file.Name;
                var fileStream = file.OpenReadStream();
                var result = await BlogService.UploadMediaAsync(fileStream, file.Name,file.ContentType);

                // Attach uploaded file URL to the blog post media
                NewBlogPost.MediaUrl = result.Url;
                NewBlogPost.MediaType = result.Type; // e.g., MediaType.Image, MediaType.Video
            }
        }
    }
}

