# GitHub Pages Deployment Guide

This guide explains how to deploy the Event Video Playback website to GitHub Pages.

## Prerequisites

Before deploying, ensure you have:
- Push access to the repository
- GitHub Pages enabled in repository settings

## Deployment Methods

### Method 1: Automatic Deployment (Recommended)

The site automatically deploys when you push changes to the `main` or `v2.0.0` branches.

1. **Make your changes** in the `gh_pages/` directory
2. **Commit your changes**:
   ```bash
   git add gh_pages/
   git commit -m "Update website content"
   ```
3. **Push to GitHub**:
   ```bash
   git push origin main
   # or
   git push origin v2.0.0
   ```
4. **Monitor deployment**:
   - Go to the repository **Actions** tab
   - Watch the "Deploy Jekyll site to GitHub Pages" workflow
   - Deployment typically takes 1-2 minutes

5. **View the live site**:
   - https://beckhoff-usa-community.github.io/EventVideoPlayback/

### Method 2: Manual Workflow Trigger

You can manually trigger a deployment from the GitHub Actions interface:

1. Go to the repository **Actions** tab
2. Select "Deploy Jekyll site to GitHub Pages"
3. Click "Run workflow"
4. Choose the branch (main or v2.0.0)
5. Click "Run workflow" button

## First-Time Setup

If this is the first deployment, you need to configure GitHub Pages settings:

### Step 1: Enable GitHub Pages

1. Go to repository **Settings** > **Pages**
2. Under "Build and deployment":
   - **Source**: Deploy from a branch (this will be changed automatically)
   - Or select: **GitHub Actions** (recommended)

### Step 2: Set Permissions

1. Go to repository **Settings** > **Actions** > **General**
2. Scroll to "Workflow permissions"
3. Select: **Read and write permissions**
4. Check: **Allow GitHub Actions to create and approve pull requests**
5. Click **Save**

### Step 3: Configure Environment

1. Go to repository **Settings** > **Environments**
2. You should see a "github-pages" environment
3. If not, it will be created automatically on first deployment

### Step 4: First Deployment

1. Push any change to trigger deployment:
   ```bash
   git add gh_pages/
   git commit -m "Initial website deployment"
   git push origin main
   ```

2. Go to **Actions** tab and watch the workflow
3. Wait for deployment to complete
4. The site URL will appear in the workflow output

## Verifying Deployment

### Check Workflow Status

1. **Actions Tab**: View real-time build logs
2. **Green checkmark**: Deployment successful
3. **Red X**: Deployment failed (check logs for errors)

### Test the Live Site

Visit: https://beckhoff-usa-community.github.io/EventVideoPlayback/

**Test checklist:**
- [ ] Landing page loads correctly
- [ ] Hero image displays
- [ ] Navigation menu works
- [ ] All documentation pages load
- [ ] Internal links work
- [ ] Styles and colors are correct
- [ ] Mobile responsive design works
- [ ] JavaScript animations work

## Troubleshooting

### Deployment Fails

**Error: "Process completed with exit code 16"**
- Jekyll build failed
- Check syntax in Markdown files
- Verify YAML front matter is correct
- Review GitHub Actions logs for specific errors

**Fix:**
```bash
# Test build locally
cd gh_pages
bundle exec jekyll build
# Fix any errors shown
```

**Error: "No uploaded artifact was found"**
- Build completed but artifact upload failed
- Usually temporary; retry deployment

**Fix:**
- Re-run the workflow from Actions tab
- Check GitHub status page for outages

### Site Not Updating

**Changes not visible after deployment:**

1. **Wait**: CDN propagation can take 1-2 minutes
2. **Clear cache**:
   - Hard refresh: `Ctrl+Shift+R` (Windows) or `Cmd+Shift+R` (Mac)
   - Open in private/incognito window
3. **Verify deployment**:
   - Check Actions tab shows successful deployment
   - Verify commit SHA matches latest push

### 404 Errors on Navigation

**Internal links showing 404:**

1. Check `_config.yml` baseurl is correct:
   ```yaml
   baseurl: "/EventVideoPlayback"
   ```

2. Verify links use `relative_url` filter:
   ```liquid
   {{ '/docs/getting-started/' | relative_url }}
   ```

3. Check permalinks in front matter match navigation links

### Styles Not Loading

**CSS not applied:**

1. Check `assets/css/style.scss` has front matter:
   ```yaml
   ---
   ---
   ```

2. Verify SCSS files are imported:
   ```scss
   @import "variables";
   @import "layout";
   @import "components";
   ```

3. Clear browser cache and try again

### Images Not Loading

**Hero image or other images broken:**

1. Verify image path:
   ```liquid
   {{ '/assets/images/Camera_Hero_NoBackground.png' | relative_url }}
   ```

2. Check image exists in `assets/images/`
3. Verify image filename matches (case-sensitive)

## Workflow Details

The deployment workflow (`.github/workflows/pages.yml`) performs these steps:

1. **Checkout**: Gets the repository code
2. **Setup Ruby**: Installs Ruby 3.1 and dependencies
3. **Setup Pages**: Configures GitHub Pages
4. **Build Jekyll**: Compiles the site with production settings
5. **Upload Artifact**: Packages the built site
6. **Deploy**: Publishes to GitHub Pages

### Workflow Triggers

The workflow runs when:
- Pushing to `main` or `v2.0.0` branches
- Only if files in `gh_pages/` or workflow file change
- Manually triggered via Actions tab

### Build Time

Typical build and deployment: **1-2 minutes**

Breakdown:
- Checkout: 5-10 seconds
- Ruby setup: 20-30 seconds
- Jekyll build: 15-30 seconds
- Deploy: 30-60 seconds

## Making Changes Safely

### Best Practices

1. **Test Locally First**:
   ```bash
   cd gh_pages
   bundle exec jekyll serve
   # Test at http://localhost:4000/EventVideoPlayback/
   ```

2. **Use Feature Branches**:
   ```bash
   git checkout -b update-documentation
   # Make changes
   git commit -m "Update documentation"
   git push origin update-documentation
   # Create pull request for review
   ```

3. **Review Before Merge**:
   - Check pull request preview
   - Verify changes in GitHub's file view
   - Review with team if needed

4. **Monitor Deployment**:
   - Watch Actions tab after merge
   - Test live site immediately after deployment
   - Be ready to rollback if issues occur

### Emergency Rollback

If a deployment breaks the site:

1. **Revert the commit**:
   ```bash
   git revert HEAD
   git push origin main
   ```

2. **Or reset to previous version**:
   ```bash
   git reset --hard <previous-commit-sha>
   git push origin main --force
   ```

3. **Or checkout previous version**:
   ```bash
   git checkout <previous-commit-sha> gh_pages/
   git commit -m "Rollback to previous version"
   git push origin main
   ```

## Performance Optimization

### Image Optimization

Before adding images:
```bash
# Optimize PNG
optipng -o7 assets/images/image.png

# Or use ImageMagick
convert input.png -strip -quality 85 output.png
```

### Build Optimization

The workflow uses:
- Bundler cache for faster Ruby setup
- Incremental builds (when possible)
- Production environment for optimized output

## Monitoring

### GitHub Pages Status

Check: https://www.githubstatus.com/

Look for issues with:
- GitHub Pages
- GitHub Actions
- Git Operations

### Site Analytics

Consider adding:
- Google Analytics
- GitHub Traffic Stats (Settings > Insights > Traffic)

## Support

**Issues with deployment?**

1. Check [GitHub Actions logs](../../actions)
2. Review [GitHub Pages documentation](https://docs.github.com/en/pages)
3. Check [Jekyll documentation](https://jekyllrb.com/docs/)
4. Open an issue in the repository

## Additional Resources

- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [Jekyll Documentation](https://jekyllrb.com/docs/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Repository Settings](../../settings)
- [View Deployments](../../deployments/github-pages)
