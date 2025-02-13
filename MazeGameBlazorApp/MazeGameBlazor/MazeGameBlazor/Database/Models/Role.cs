using Microsoft.AspNetCore.Identity;

namespace MazeGameBlazor.Database.Models
{
    /// <summary>
    /// Represents a user role in the Identity system.
    /// Inherits from IdentityRole to enable role-based access control.
    /// </summary>
    public class Role : IdentityRole
    {
        // This class is for extending IdentityRole in case additional role-related properties are needed.
    }
}