// Theme Switcher
(function() {
    'use strict';

    const THEME_KEY = 'user-theme-preference';
    const DEFAULT_THEME = 'dark';

    // Get the current theme from localStorage or use default
    function getCurrentTheme() {
        return localStorage.getItem(THEME_KEY) || DEFAULT_THEME;
    }

    // Set the theme on the document
    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
        updateThemeButtons(theme);
    }

    // Update the active state of theme buttons
    function updateThemeButtons(activeTheme) {
        const buttons = document.querySelectorAll('.theme-btn');
        buttons.forEach(button => {
            const buttonTheme = button.getAttribute('data-theme');
            if (buttonTheme === activeTheme) {
                button.classList.add('active');
            } else {
                button.classList.remove('active');
            }
        });
    }

    // Initialize theme on page load
    function initTheme() {
        const currentTheme = getCurrentTheme();
        setTheme(currentTheme);
    }

    // Set up event listeners for theme buttons
    function setupThemeListeners() {
        const themeButtons = document.querySelectorAll('.theme-btn');
        themeButtons.forEach(button => {
            button.addEventListener('click', function() {
                const selectedTheme = this.getAttribute('data-theme');
                setTheme(selectedTheme);
            });
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            initTheme();
            setupThemeListeners();
        });
    } else {
        initTheme();
        setupThemeListeners();
    }
})();
