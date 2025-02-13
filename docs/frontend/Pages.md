# Home.razor

## Overview
The `Home` page serves as the **landing page** for the Maze Game Project.  
It provides:
✔ A **hero section** introducing the project.  
✔ A **latest news** section showcasing the most recent blog post.  
✔ A **game description** with a play button.

---

## Structure

| Section          | Description |
|-----------------|------------|
| **Hero Section**  | Displays the project title, description, and GitHub link. |
| **Latest News**   | Shows the most recent blog post using the `BlogCard` component. |
| **Game Description** | Briefly explains the maze game and provides a play button. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **Dynamic Blog Post** | Fetches and displays the latest blog post from the database. |
| **Media Support** | If the blog post contains media (image, video, etc.), it is displayed. |
| **Error Handling** | Logs errors if the blog post cannot be fetched. |

---

## Example Usage
The `Home` page is automatically used when visiting `/`.  
No additional setup is required.

---

## Notes
- The page **uses the `BlogService`** to fetch the latest blog post.
- The `BlogCard` component **dynamically renders** the latest blog post.
- **Error logging** helps diagnose issues when fetching data.

---


# Blog.razor

## Overview
The `Blog` page serves as a **centralized hub** for blog posts in the Maze Game Project.  
It provides:
✔ A **grid layout** for displaying multiple blog posts.  
✔ **Media previews** for images and videos.  
✔ A **modal window** to expand a blog post without navigating away.  

---

## Structure

| Section          | Description |
|-----------------|------------|
| **Blog Grid**  | Displays blog post previews in a card-based layout. |
| **Blog Preview** | Shows title, author, date, and media preview. |
| **Blog Modal** | Expands the selected post with full details. |

---

## Features

| Feature           | Description |
|------------------|------------|
| **Dynamic Blog Post Loading** | Fetches and displays all blog posts from the database. |
| **Media Handling** | Displays an image or video preview for each post. |
| **Modal View** | Clicking a post opens it in a **modal popup** instead of navigating away. |
| **Error Handling** | Prevents crashes if data is unavailable. |

---

## Example Usage
The `Blog` page is automatically used when visiting `/blog`.  
No additional setup is required.

---

## Notes
- The page **uses the `BlogService`** to fetch all blog posts.
- Clicking on a blog post **opens it in a modal** instead of navigating.
- The `BlogCard` component **renders the full blog post** inside the modal.
- **Optimized for performance** by reducing unnecessary navigation.

---

# CreateBlog.razor

## Overview
The `CreateBlog` page allows authenticated users to **create new blog posts**.  
It includes:
✔ **Media Upload** – Users can upload or select existing media.  
✔ **Blog Post Form** – Title and content fields with validation.  
✔ **Media Attachment** – Users can attach selected media to their blog post.  

---

## Structure

| Section            | Description |
|--------------------|------------|
| **Media Upload**   | Allows users to upload new media files (images, videos, etc.). |
| **Available Media** | Displays previously uploaded media for selection. |
| **Blog Post Form** | Provides input fields for the blog post's title and content. |
| **Selected Media** | Shows media files attached to the blog post. |
| **Submit Button**  | Saves the blog post and redirects to the blog page. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **Authentication** | Ensures only logged-in users can create blog posts. |
| **Media Upload** | Supports **file selection** and **uploads to the server**. |
| **Media Gallery** | Displays previously uploaded media for selection. |
| **Form Validation** | Enforces required fields and basic data validation. |
| **Post Submission** | Saves the blog post and **attaches selected media**. |

---

# Play.razor

## Overview
The `Play` page serves as the **game interface** for the Maze Game Project.  
It includes:
✔ A **game canvas** for rendering the maze.  
✔ **Start and restart buttons** for controlling gameplay.  
✔ **Game instructions** to guide players.  

---

## Structure

| Section          | Description |
|-----------------|------------|
| **Game Header**  | Displays the title and a short description of the game. |
| **Game Screen**  | Contains the `<canvas>` element where the maze is rendered. |
| **Game Controls** | Includes "Start" and "Restart" buttons for gameplay. |
| **Game Instructions** | Explains how to play the game. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **Maze Rendering** | The game is displayed inside a `<canvas>` element. |
| **Start Button** | Begins the game and initializes movement. |
| **Restart Button** | Resets the game back to the starting state. |
| **Overlay Messages** | Displays game status messages like "Ready?", "Game Over", or "You Win!". |

---

## Example Usage
The `Play` page is automatically used when visiting `/play`.  
No additional setup is required.

---

## Notes
- **The game logic is not yet implemented** but will be integrated inside the `@code { }` block.
- The **maze will be drawn on a `<canvas>` element** with JavaScript or Blazor game logic.
- The game **responds to keyboard inputs** (arrow keys) for movement.
- **Future improvements** will include:
  - Collision detection with walls and traps.
  - Timer-based score tracking.
  - Additional levels and maze complexity.

---

# Login.razor

## Overview
The `Login` page provides user authentication functionality.  
It includes:
✔ **A login form** with email and password fields.  
✔ **Validation and error handling** for incorrect credentials.  
✔ **A registration link** for users without an account.  
✔ **A login success message with auto-hide functionality**.  

---

## Structure

| Section             | Description |
|---------------------|------------|
| **Login Form**      | Users enter email and password to log in. |
| **Validation Summary** | Displays errors if login fails. |
| **Register Section** | Provides a link to the registration page. |
| **Login Message**   | Shows success or failure messages. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **Authentication** | Validates credentials via the `AuthService`. |
| **Form Validation** | Ensures fields are correctly filled. |
| **Error Handling** | Displays messages for login failures. |
| **Auto-Hide Form on Success** | Hides the form if login succeeds. |

---

To navigate programmatically:
```csharp
NavManager.NavigateTo("/login");
```

# Logout.razor

## Overview
The `Logout` page is responsible for **logging users out of the system**.  
It includes:
✔ **Automatic logout execution** when the page loads.  
✔ **A message indicating the logout status**.  
✔ **A button to return to the home page** after logout is complete.  

---

## Structure

| Section             | Description |
|---------------------|------------|
| **Logout Message**  | Displays the current logout status. |
| **Home Link**       | Redirects users back to the homepage after logout. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **Automatic Logout** | Triggers logout as soon as the page loads. |
| **Authentication Check** | Verifies if the user is logged in before attempting logout. |
| **Error Handling** | Displays an error message if logout fails. |
| **Redirect Option** | Shows a "Return to Home" button once logout is complete. |

---

# MyAccount.razor

## Overview
The `MyAccount` page allows users to **manage their profile information**.  
It includes:
✔ **A user profile form** for updating the username and email.  
✔ **Password update option** (optional).  
✔ **A logout button** for signing out of the system.  

---

## Structure

| Section             | Description |
|---------------------|------------|
| **Profile Form**    | Allows users to update their username and email. |
| **Password Field**  | Allows users to enter a new password (optional). |
| **Logout Button**   | Logs the user out of the system. |
| **Status Message**  | Displays confirmation messages for profile updates. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **Profile Editing** | Users can update their username and email. |
| **Password Change Support** | Users can update their password (optional). |
| **Form Validation** | Ensures data is entered correctly before submission. |
| **Direct SQL Update** | Uses a raw SQL query to update user details. |
| **Session Refresh** | Ensures changes take effect immediately. |

---

To navigate programmatically:
```csharp
NavManager.NavigateTo("/my-account");
```

# Register.razor

## Overview
The `Register` page allows new users to **create an account**.  
It includes:
✔ **A registration form** for entering a username, email, and password.  
✔ **Validation for passwords and email format.**  
✔ **A confirmation message upon successful registration.**  
✔ **A redirect link to the login page.**  

---

## Structure

| Section             | Description |
|---------------------|------------|
| **Registration Form** | Fields for username, email, password, and password confirmation. |
| **Validation Summary** | Displays error messages for incorrect input. |
| **Success Message** | Informs users if registration was successful. |
| **Login Redirect** | Provides a link to the login page after successful registration. |

---

## Features

| Feature          | Description |
|-----------------|------------|
| **User Registration** | Allows new users to create an account. |
| **Password Confirmation** | Ensures users enter matching passwords. |
| **Validation Handling** | Displays messages for incorrect inputs. |
| **Error Handling** | Shows relevant errors if registration fails. |

---

To navigate programmatically:
```csharp
NavManager.NavigateTo("/register");
```
