# GitHub Pages Website Build Instructions

## Project Overview
Build a modern, impressive Jekyll-based GitHub Pages website for the Event Video Playback project. The site should feature a stunning landing page with documentation sections for PLC library usage, service configuration, and HMI integration.

**Repository:** https://github.com/Beckhoff-USA-Community/EventVideoPlayback
**Hero Image:** `Camera_Hero_NoBackground.png` (red/black color scheme)
**Target Audience:** TwinCAT developers and automation engineers

## Design Requirements

### Visual Style
- **Modern & Impressive**: Users should say "wow" when they see it
- **Color Scheme**: Red (#D32F2F or similar) and black/dark gray, inspired by the hero image
- **Typography**: Clean, technical, professional fonts (e.g., Inter, Roboto, or Source Sans Pro)
- **Layout**: Responsive design that works on desktop, tablet, and mobile
- **Animations**: Subtle, professional animations (fade-ins, smooth scrolls)

### Landing Page Structure
1. **Hero Section**
   - Large hero image (`Camera_Hero_NoBackground.png`) prominently displayed
   - Project title: "Event Video Playback"
   - Tagline: "Transform TwinCAT Vision images into event-driven video recordings"
   - Primary CTA button: "Get Started" (links to documentation)
   - Secondary CTA button: "View on GitHub" (links to repo)

2. **Features Section**
   - 3-4 feature cards highlighting key capabilities:
     - Event-driven video capture from TwinCAT Vision
     - Automatic video logging with Event Logger integration
     - HMI playback control for reviewing events
     - Easy-to-use PLC function blocks

3. **Quick Overview Section**
   - Brief description of what the project does
   - Use case example (machine down occurrence scenario)
   - Visual diagram or icon representation

4. **System Requirements Section**
   - Quick list of requirements
   - Link to detailed installation guide

5. **Footer**
   - Links to documentation sections
   - GitHub repository link
   - Beckhoff USA Community branding
   - License information

### Documentation Pages

The site needs the following documentation pages with placeholder content (content can be filled in later):

1. **Getting Started / Installation**
   - Installation overview
   - System requirements
   - Quick start guide
   - *Content can be populated from existing markdown files later*

2. **PLC Library Usage**
   - PLC library overview
   - Basic usage examples
   - Function block reference
   - *Content can be populated from existing markdown files later*

3. **Service Configuration**
   - Service settings overview
   - Configuration parameters
   - Troubleshooting
   - *Content can be populated from existing markdown files later*

4. **HMI NuGet Package Usage**
   - HMI control installation
   - Configuration steps
   - Usage examples
   - *Content can be populated from existing markdown files later*

**PRIORITY**: Focus on creating the page structure, navigation, and visual design. The actual content text is secondary and can be added later by the user.

## Technical Implementation

### Jekyll Setup

1. **Initialize Jekyll in the `gh_pages` directory**
   ```bash
   cd gh_pages
   jekyll new . --force --skip-bundle
   ```

2. **Gemfile Configuration**
   - Use `github-pages` gem for compatibility
   - Add necessary plugins:
     ```ruby
     gem "github-pages", group: :jekyll_plugins
     gem "webrick" # Required for Ruby 3.0+

     group :jekyll_plugins do
       gem "jekyll-seo-tag"
       gem "jekyll-sitemap"
     end
     ```

3. **_config.yml Settings**
   ```yaml
   title: Event Video Playback
   description: Transform TwinCAT Vision images into event-driven video recordings
   baseurl: "/EventVideoPlayback"
   url: "https://beckhoff-usa-community.github.io"

   # Build settings
   theme: minima
   plugins:
     - jekyll-seo-tag
     - jekyll-sitemap

   # Collections for documentation
   collections:
     docs:
       output: true
       permalink: /docs/:path/

   # Navigation
   header_pages:
     - docs/getting-started.md
     - docs/plc-usage.md
     - docs/service-config.md
     - docs/hmi-usage.md
   ```

### Directory Structure

Create the following structure in `gh_pages`:
```
gh_pages/
├── _config.yml
├── Gemfile
├── index.md (landing page)
├── _docs/
│   ├── getting-started.md
│   ├── plc-usage.md
│   ├── service-config.md
│   └── hmi-usage.md
├── _layouts/
│   ├── default.html
│   ├── home.html
│   └── doc.html
├── _includes/
│   ├── header.html
│   ├── footer.html
│   └── navigation.html
├── assets/
│   ├── css/
│   │   └── style.scss
│   ├── js/
│   │   └── main.js
│   └── images/
│       └── Camera_Hero_NoBackground.png
└── _sass/
    ├── _variables.scss
    ├── _layout.scss
    └── _components.scss
```

### Custom Styling

Create a custom theme override in `assets/css/style.scss`:

**Color Palette:**
- Primary Red: `#D32F2F` or `#C62828`
- Dark Gray/Black: `#212121` or `#1a1a1a`
- Accent Red: `#FF5252`
- Light Gray: `#F5F5F5`
- White: `#FFFFFF`

**Key CSS Features:**
- Hero section with gradient overlay
- Feature cards with hover effects
- Smooth scroll behavior
- Responsive navigation menu
- Code syntax highlighting
- Sticky header on scroll
- Button hover animations

### Content & Assets Setup

1. **Hero Image**
   - Copy `Camera_Hero_NoBackground.png` to `assets/images/`
   - Optimize for web display

2. **Placeholder Documentation**
   - Create documentation pages with basic structure
   - Use lorem ipsum or brief placeholder text
   - Focus on demonstrating the layout and navigation flow
   - User will populate actual content later

3. **Navigation Links**
   - Set up working navigation between all pages
   - Include GitHub repo links
   - Add "Back to Home" links from doc pages
   - Ensure all internal links use Jekyll's permalink structure

### GitHub Pages Deployment

1. **Configure GitHub Repository Settings**
   - Go to repository Settings > Pages
   - Set source to: Deploy from a branch
   - Branch: `gh-pages` or `main` (depending on your workflow)
   - Folder: `/gh_pages` or `/` (root)

2. **Create GitHub Actions Workflow** (optional but recommended)
   - Create `.github/workflows/pages.yml`
   - Automate Jekyll build and deployment
   ```yaml
   name: Deploy Jekyll site to Pages

   on:
     push:
       branches: ["main", "v2.0.0"]

   jobs:
     build:
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v3
         - uses: actions/configure-pages@v3
         - uses: actions/jekyll-build-pages@v1
           with:
             source: ./gh_pages
         - uses: actions/upload-pages-artifact@v2

     deploy:
       needs: build
       runs-on: ubuntu-latest
       permissions:
         pages: write
         id-token: write
       environment:
         name: github-pages
       steps:
         - uses: actions/deploy-pages@v2
   ```

## Step-by-Step Execution Plan

### Phase 1: Jekyll Foundation
1. Initialize Jekyll project in `gh_pages` directory
2. Configure `_config.yml` with project details
3. Set up `Gemfile` with required gems
4. Create directory structure for layouts, includes, and assets

### Phase 2: Landing Page Design
1. Create custom `home.html` layout with hero section
2. Design feature cards section
3. Add overview and quick start sections
4. Implement responsive navigation
5. Create footer with links

### Phase 3: Custom Styling
1. Set up SCSS structure with variables
2. Implement red/black color scheme
3. Create component styles (buttons, cards, hero)
4. Add animations and transitions
5. Ensure mobile responsiveness

### Phase 4: Documentation Pages
1. Create `_docs` collection
2. Create getting-started.md with placeholder content and proper structure
3. Create plc-usage.md with placeholder content and proper structure
4. Create service-config.md with placeholder content and proper structure
5. Create hmi-usage.md with placeholder content and proper structure
6. Add front matter to all docs
7. Set up proper navigation between pages
8. **Note**: Focus on layout and navigation structure, not final content

### Phase 5: Assets & Images
1. Copy hero image to assets
2. Copy documentation images (if available)
3. Optimize images for web
4. Create fallback placeholders if images are missing

### Phase 6: Testing & Deployment
1. Test locally with `bundle exec jekyll serve`
2. Verify all links work
3. Test responsive design on multiple screen sizes
4. Check syntax highlighting in code blocks
5. Configure GitHub Pages settings
6. Push to repository and verify deployment
7. Test live site at https://beckhoff-usa-community.github.io/EventVideoPlayback/

## Important Notes

- **Design Focus**: Prioritize impressive visual design, smooth navigation, and professional layout over content accuracy
- **Placeholder Content**: It's acceptable to use placeholder text for documentation pages - the structure and navigation are what matter most
- **Hero Image**: The `Camera_Hero_NoBackground.png` should be the star of the landing page
- **Navigation**: Make it easy to jump between sections with a sticky nav or sidebar
- **Mobile First**: Ensure excellent mobile experience with responsive navigation
- **Performance**: Site should load quickly and feel snappy
- **GitHub Pages Compatibility**: Test thoroughly that everything works on GitHub Pages deployment

## Success Criteria

The completed website should:
- **Make users say "WOW"** with modern, impressive design
- Load quickly (under 3 seconds)
- Be fully responsive on mobile, tablet, and desktop
- Feature the hero image prominently with professional red/black styling
- Have intuitive, smooth navigation between all sections
- Include working navigation menu with clear structure
- Work perfectly when deployed to GitHub Pages
- Have placeholder documentation pages with proper layout structure
- Include functional "Get Started" and "View on GitHub" buttons
- Feature smooth scrolling and subtle animations
- Be easily editable for content updates later

**Priority Order:**
1. Visual design and "wow" factor
2. Navigation structure and usability
3. GitHub Pages deployment functionality
4. Content (lowest priority - can be added later)

## Resources

- Jekyll Documentation: https://jekyllrb.com/docs/
- GitHub Pages Documentation: https://docs.github.com/en/pages
- Markdown Guide: https://www.markdownguide.org/
- CSS Grid & Flexbox for layouts
- Font options: Google Fonts (Inter, Roboto, Source Sans Pro)
