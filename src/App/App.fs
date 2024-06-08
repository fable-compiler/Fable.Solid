module App

open Browser

Solid.render (Routing.Routing, document.getElementById ("app-container"))

// Keeping this entry file simple so that it does not get changed .
// This helps to avoid an issue where components are rendered as duplicates while watching and reloading with Vite
