# Blazor Routing and Navigation

## Overview
Blazor uses a **built-in routing system** to manage navigation between different components in the application.  
This system enables navigation without full-page reloads, making the application feel more like a **single-page app (SPA)**.

The `App.razor` file contains the **router configuration**, which:
✔ **Handles route resolution** and matches URLs to components.  
✔ **Provides authentication state globally** with `CascadingAuthenticationState`.  
✔ **Defines a "Page Not Found" message** for unmatched routes.  
✔ **Uses layouts to maintain a consistent structure across pages.**  

---

## **Blazor Router Explained**
The Blazor Router (`<Router>`) is responsible for matching URLs to the correct components.

### **Basic Structure of Blazor Routing**
```razor
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="typeof(MainLayout)">
            <h1>Page Not Found</h1>
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

# How Blazor Finds a Route

Blazor finds a route using the `@page` directive inside components.  
For example:

```razor
@page "/about"
<h1>About Us</h1>
<p>This is the about page.</p>
```

✔ When the user visits `/about`, Blazor **renders this component**.  
✔ If no matching route exists, the **NotFound** layout is shown instead.  

---

# Navigation in Blazor

Blazor provides multiple ways to navigate between pages.

## **Using `<NavLink>` for Navigation**
```razor
<NavLink href="/home">Go to Home</NavLink>
```
✔ Works like `<a>` but applies active styling when the route matches.  
✔ Automatically updates UI when navigation occurs.

---

## **Using `NavigationManager` for Programmatic Navigation**
```csharp
@inject NavigationManager NavManager
...
NavManager.NavigateTo("/dashboard");
```
✔ Redirects the user to `/dashboard` **without a full page reload**.  
✔ Can be used **inside event handlers** or authentication flows.  

---

# Handling "Page Not Found" Errors

If a user visits an unknown route, the `<NotFound>` section is displayed:

```razor
<NotFound>
    <LayoutView Layout="typeof(MainLayout)">
        <h1>Page Not Found</h1>
        <p>Sorry, there's nothing at this address.</p>
    </LayoutView>
</NotFound>
```

✔ Ensures **better user experience** instead of a blank page.  
✔ Can be customized with **helpful links or a search bar**.  

---

# Common Issues & Fixes

| **Issue**  | **Solution**  |
|------------|--------------|
| **"Page Not Found" error for existing pages** | Ensure the `@page` directive is present in the component. |
| **Navigation doesn't work** | Verify `<Router>` is correctly set up in `App.razor`. |
| **Layouts not applying** | Ensure `DefaultLayout="typeof(MainLayout)"` is included in `<RouteView>`. |

---
