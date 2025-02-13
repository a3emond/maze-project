# Standalone Components: `NavBar.razor` & `BlogCard.razor`

## Overview
This project includes several **standalone Blazor components** that provide UI functionality.  
This document covers:
1. **`NavBar.razor`** → Handles site navigation & authentication logic.
2. **`BlogCard.razor`** → Displays a blog post with media, likes, and comments.

---

## `NavBar.razor`

### **Purpose**
The `NavBar` component:
✔ Displays a **logo** and navigation links.  
✔ Supports **authentication** (showing different links for logged-in users).  
✔ Shows the **GitHub project link**.  
✔ Includes a **hamburger menu** for mobile users.

### **Key Features**
| Feature       | Description |
|--------------|------------|
| **Dynamic User Authentication** | Checks if the user is logged in and shows appropriate menu options. |
| **Role-Based Access** | Admins see an extra menu option (`Create Blog`). |
| **Logout Functionality** | Allows users to log out and be redirected. |
| **Hamburger Menu (Mobile)** | Toggles navigation visibility on smaller screens. |

### **Structure**
| Section  | Description |
|----------|------------|
| **Logo** | Displays the `logo.webp` image. |
| **Navigation Menu** | Links to Home, Play Game, Blog, and optionally Create Blog. |
| **User Profile** | Displays the username and a Logout button (if logged in). |
| **GitHub Link** | Provides a quick link to the project's GitHub repository. |

### **Example Usage**
The `NavBar` is included globally in the `MainLayout.razor`:
```razor
<NavBar />
```

# BlogCard.razor

## Purpose
The `BlogCard` component:
✔ Displays **blog post information** (title, content, author, date).  
✔ Supports **multiple media types** (images, videos, audio, documents).  
✔ Allows users to **like posts**.  
✔ Includes a **comment section** for user interaction.  

---

## Key Features

| Feature             | Description |
|---------------------|------------|
| **Media Handling**  | Supports images, videos, audio, and documents. |
| **Like Button**     | Allows users to like the post (updates in real-time). |
| **Comments**        | Displays existing comments and allows adding new ones. |
| **User Authentication** | Checks if the user is logged in to personalize the comment author field. |

---

## Media Type Handling
Depending on the `MediaType` property, the component renders:

- **Images** → `<img>` tag
- **Videos** → `<video>` tag
- **Audio** → `<audio>` tag
- **Documents** → `<a>` tag for external files

---

## Structure

| Section          | Description |
|-----------------|------------|
| **Media Preview**  | Displays an image, video, audio, or document link. |
| **Post Content**   | Shows title, author, creation date, and content. |
| **Like Button**    | Increases the like count when clicked. |
| **Comment Section** | Displays comments and allows adding new ones. |

---

## Example Usage
To display a blog post card:
```razor
<BlogCard Id="1"
          Title="First Post"
          Content="This is a sample blog post."
          Author="John Doe"
          CreatedAt="DateTime.Now"
          LikeCount="10"
          MediaType="MediaType.Image"
          MediaUrl="https://example.com/image.jpg" />



