# Event Video Playback - GitHub Pages Website

This directory contains the Jekyll-based GitHub Pages website for the Event Video Playback project.

## 🚀 Quick Start

### Local Development

1. **Install Ruby and Jekyll** (if not already installed):
   - Windows: Use [RubyInstaller](https://rubyinstaller.org/)
   - macOS: `brew install ruby`
   - Linux: `sudo apt-get install ruby-full`

2. **Install dependencies**:
   ```bash
   cd gh_pages
   bundle install
   ```

3. **Run the local server**:
   ```bash
   bundle exec jekyll serve
   ```

4. **View the site**:
   Open http://localhost:4000/EventVideoPlayback/ in your browser

### Automatic Deployment

The site automatically deploys to GitHub Pages when you push changes to the `main` or `v2.0.0` branches. The deployment is handled by the GitHub Actions workflow at `.github/workflows/pages.yml`.

**Live Site**: https://beckhoff-usa-community.github.io/EventVideoPlayback/

## 📁 Directory Structure

```
gh_pages/
├── _config.yml              # Jekyll configuration
├── Gemfile                  # Ruby dependencies
├── index.md                 # Landing page (hero section)
├── _layouts/                # Page layouts
│   ├── default.html         # Base layout with header/footer
│   ├── home.html            # Landing page layout
│   └── doc.html             # Documentation page layout
├── _includes/               # Reusable components
│   ├── header.html          # Site header with navigation
│   └── footer.html          # Site footer
├── _docs/                   # Documentation pages
│   ├── getting-started.md   # Installation guide
│   ├── plc-usage.md         # PLC library documentation
│   ├── service-config.md    # Service configuration guide
│   └── hmi-usage.md         # HMI NuGet package guide
├── _sass/                   # SCSS stylesheets
│   ├── _variables.scss      # Color scheme and design tokens
│   ├── _layout.scss         # Layout styles
│   └── _components.scss     # Component styles (hero, cards, etc.)
├── assets/
│   ├── css/
│   │   └── style.scss       # Main stylesheet (imports all SCSS)
│   ├── js/
│   │   └── main.js          # JavaScript for interactions
│   └── images/
│       └── Camera_Hero_NoBackground.png  # Hero image
└── README.md                # This file
```

## 🎨 Design System

### Color Palette

- **Primary Red**: `#D32F2F`
- **Dark Red**: `#C62828`
- **Accent Red**: `#FF5252`
- **Black**: `#1a1a1a`
- **Dark Gray**: `#212121`
- **Light Gray**: `#F5F5F5`
- **White**: `#FFFFFF`

### Typography

- **Font**: Inter (from Google Fonts)
- **Base Size**: 16px
- **Line Height**: 1.6

### Breakpoints

- **Mobile**: 480px
- **Tablet**: 768px
- **Desktop**: 1024px
- **Wide**: 1280px

## 📝 Updating Content

### Modifying the Landing Page

Edit `index.md` to change:
- Hero section text and buttons
- Feature cards
- Overview section
- System requirements
- Call-to-action sections

### Updating Documentation

Documentation pages are in the `_docs/` directory:

1. **Getting Started** (`_docs/getting-started.md`): Installation and setup
2. **PLC Library Usage** (`_docs/plc-usage.md`): Function blocks and examples
3. **Service Configuration** (`_docs/service-config.md`): Windows service settings
4. **HMI Usage** (`_docs/hmi-usage.md`): WPF control integration

Each documentation page uses the `doc` layout and includes:
```yaml
---
layout: doc
title: Page Title
description: Brief description
permalink: /docs/page-name/
---
```

### Adding New Pages

1. Create a new Markdown file in `_docs/`
2. Add front matter with layout, title, description, and permalink
3. Add the page to `_config.yml` under `header_pages`
4. Update navigation in `_includes/header.html` if needed

## 🎯 Customization

### Changing Colors

Edit `_sass/_variables.scss` to modify the color scheme:

```scss
$primary-red: #D32F2F;    // Your primary color
$dark-red: #C62828;       // Darker variant
$accent-red: #FF5252;     // Accent color
```

### Modifying Styles

- **Layout styles**: `_sass/_layout.scss`
- **Component styles**: `_sass/_components.scss`
- **Variables**: `_sass/_variables.scss`

### Adding JavaScript

Edit `assets/js/main.js` to add interactive features.

## 🔧 Configuration

Key settings in `_config.yml`:

```yaml
title: Event Video Playback
description: Transform TwinCAT Vision images into event-driven video recordings
baseurl: "/EventVideoPlayback"
url: "https://beckhoff-usa-community.github.io"
```

## 🚨 Troubleshooting

### Site not updating after push

1. Check GitHub Actions status in the **Actions** tab
2. Verify the workflow ran successfully
3. Wait 1-2 minutes for changes to propagate
4. Clear browser cache and refresh

### Styles not loading

1. Ensure `assets/css/style.scss` has front matter (`---` at the top)
2. Check that SCSS files in `_sass/` are properly imported
3. Verify file paths are correct

### Navigation links broken

1. Check `_config.yml` for correct `baseurl`
2. Use Liquid tags for links: `{{ '/docs/page/' | relative_url }}`
3. Ensure permalink in front matter matches navigation links

### Local build fails

```bash
# Clear Jekyll cache
bundle exec jekyll clean

# Reinstall dependencies
bundle install

# Try building again
bundle exec jekyll serve
```

## 📚 Resources

- [Jekyll Documentation](https://jekyllrb.com/docs/)
- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [Liquid Template Language](https://shopify.github.io/liquid/)
- [Markdown Guide](https://www.markdownguide.org/)

## 🤝 Contributing

When updating the website:

1. Test changes locally first with `bundle exec jekyll serve`
2. Check responsive design on mobile, tablet, and desktop
3. Verify all links work
4. Ensure images load correctly
5. Test navigation between pages
6. Push to a branch and create a pull request

## 📄 License

This website content is part of the Event Video Playback project and is licensed under the MIT License. See the [LICENSE](../LICENSE) file in the repository root.
