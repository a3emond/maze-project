// js/site.js

window.toggleNavbar = function () {
    const navMenu = document.getElementById("navMenu");
    if (!navMenu) return;

    const isOpen = navMenu.classList.toggle("open");
    console.log("[toggleNavbar] triggered. State:", isOpen);

    // Attach auto-close to all links inside the nav
    if (isOpen) {
        const links = navMenu.querySelectorAll("a, button");
        links.forEach(link => {
            link.addEventListener("click", window._closeNavbarOnce);
        });
    }
};

window._closeNavbarOnce = function () {
    const navMenu = document.getElementById("navMenu");
    if (!navMenu) return;
    navMenu.classList.remove("open");

    // Clean up: remove listener from all elements
    const links = navMenu.querySelectorAll("a, button");
    links.forEach(link => {
        link.removeEventListener("click", window._closeNavbarOnce);
    });
};