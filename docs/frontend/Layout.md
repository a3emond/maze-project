# Blazor Layouts: MainLayout & MinimalLayout

## Overview
Blazor **layouts** define the common structure of the application,  
including headers, navigation, footers, and main content areas.

This project uses **two layouts**:
1. `MainLayout.razor` → Standard layout with a **header, navbar, footer**.
2. `MinimalLayout.razor` → A lightweight layout with **only the page content**.

---

## `MainLayout.razor`

### **Purpose**
The `MainLayout` is the **default layout** for the application.  
It includes:
✔ A **header** with the project title and course info.  
✔ A **navigation bar** (`<NavBar />` component).  
✔ A **main content area** (`@Body`).  
✔ A **footer** with copyright information.

### **Structure**
| Section  | Description |
|----------|------------|
| **Header** | Displays the project name and tagline. |
| **NavBar** | Includes the navigation menu. |
| **Main Content** | The `@Body` section renders the page content dynamically. |
| **Footer** | Displays copyright and author information. |

### **Usage**
Blazor automatically applies `MainLayout.razor` as the **default layout**.

To explicitly set it:
```razor
@layout MainLayout
