# Documentation PDF Generation

This directory contains auto-generated PDF versions of the Event Video Playback documentation.

## 📄 Available PDFs

The following PDFs are automatically generated from the markdown documentation:

- **getting-started.pdf** - Installation and setup guide
- **plc-usage.pdf** - PLC library documentation
- **service-config.pdf** - Service configuration guide
- **hmi-usage.pdf** - HMI NuGet package guide
- **complete-documentation.pdf** - All documentation in one PDF

## 🔄 How PDF Generation Works

PDFs are automatically generated using **GitHub Actions** whenever documentation changes are pushed.

### Workflow Trigger

The PDF generation workflow (`.github/workflows/generate-docs-pdf.yml`) runs when:
- Changes are pushed to `gh_pages/_docs/` directory
- Changes are pushed to the PDF template
- Workflow is manually triggered via GitHub Actions UI

### Generation Process

1. **Pandoc** converts markdown files to PDF
2. Custom **LaTeX template** applies Event Video Playback branding (red/black theme)
3. PDFs include:
   - Professional formatting
   - Table of contents
   - Headers and footers
   - Color-coded links
   - Code syntax highlighting
   - Page numbers
4. Generated PDFs are committed back to this directory
5. PDFs become available for download on the website

## 🎨 PDF Styling

PDFs use a custom LaTeX template (`assets/pdf-template/template.tex`) that:
- Matches the website's red/black color scheme
- Uses primary red (#D32F2F) for headings and links
- Includes Beckhoff USA Community branding
- Provides professional headers and footers
- Formats code blocks with syntax highlighting

## 🚀 Manual Workflow Trigger

To manually regenerate PDFs:

1. Go to **GitHub Actions** tab
2. Select **"Generate Documentation PDFs"** workflow
3. Click **"Run workflow"**
4. Choose branch (main or v2.0.0)
5. Click **"Run workflow"** button

PDFs will be generated and committed within 2-3 minutes.

## 📥 Download Buttons

Each documentation page includes two download buttons:

1. **"Download as PDF"** - Downloads the current page as PDF
2. **"Download Complete Docs"** - Downloads all documentation combined

Buttons are styled with:
- White background with red border (current page)
- Red background with white text (complete docs)
- Hover animations
- Download icons from Feather Icons

## 🔧 Customizing PDF Output

### Modify Template

Edit `assets/pdf-template/template.tex` to customize:
- Colors and branding
- Page layout and margins
- Header/footer content
- Font sizes and styles
- Code block styling

### Modify Workflow

Edit `.github/workflows/generate-docs-pdf.yml` to:
- Change Pandoc options
- Add/remove PDFs
- Adjust metadata (title, author, date)
- Configure PDF settings

### Example: Change Margins

In the workflow, modify the `--variable geometry:margin=` option:

```yaml
--variable geometry:margin=1.5in  # Larger margins
--variable geometry:margin=0.75in # Smaller margins
```

### Example: Change Font Size

```yaml
--variable fontsize=12pt  # Larger text
--variable fontsize=10pt  # Smaller text
```

## 🐛 Troubleshooting

### PDFs Not Generating

1. Check GitHub Actions workflow status
2. Review workflow logs for errors
3. Ensure markdown syntax is valid
4. Verify LaTeX template syntax

### PDF Formatting Issues

- **Tables not displaying correctly**: Simplify table structure or use smaller font
- **Code blocks overflowing**: Use `breaklines=true` in listings settings
- **Links not working**: Ensure URLs are properly formatted in markdown

### Force Regeneration

If PDFs appear outdated:

1. Make a small change to any doc file (add a space)
2. Commit and push
3. Workflow will run automatically

Or trigger manually via GitHub Actions UI.

## 📦 Dependencies

The workflow requires:
- **Pandoc** - Document converter
- **LaTeX** (texlive) - PDF rendering engine
- Includes: texlive-latex-base, texlive-fonts-recommended, texlive-fonts-extra, texlive-latex-extra

These are automatically installed by the GitHub Actions runner.

## 📝 Notes

- PDFs are committed with `[skip ci]` message to avoid infinite workflow loops
- Generated PDFs are tracked in git for easy access
- File sizes: ~100-500KB per PDF (depending on content)
- Generation time: ~30-60 seconds per PDF

## 🔗 Links

- [GitHub Actions Workflows](../../.github/workflows/)
- [PDF Template](../pdf-template/template.tex)
- [Documentation Source](_docs/)
- [Live Website](https://beckhoff-usa-community.github.io/EventVideoPlayback/)
