// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Theme Management
function setTheme(theme) {
    const root = document.documentElement;
    
    // Remove all theme classes
    root.classList.remove('theme-pink', 'theme-green', 'theme-blue', 'theme-purple', 'theme-yellow');
    
    // Add the selected theme class
    if (theme === 'green') {
        root.classList.add('theme-green');
    } else if (theme === 'blue') {
        root.classList.add('theme-blue');
    } else if (theme === 'purple') {
        root.classList.add('theme-purple');
    } else if (theme === 'yellow') {
        root.classList.add('theme-yellow');
    }
    // Pink is default, no class needed
    
    // Save preference
    localStorage.setItem('crochet-theme', theme);
    
    // Update button states
    document.querySelectorAll('.theme-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    
    const activeBtn = theme === 'pink' 
        ? document.querySelector('.theme-btn-pink')
        : document.querySelector(`.theme-btn-${theme}`);
    
    if (activeBtn) {
        activeBtn.classList.add('active');
    }
}

// Load saved theme on page load
document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = localStorage.getItem('crochet-theme') || 'pink';
    setTheme(savedTheme);
});

