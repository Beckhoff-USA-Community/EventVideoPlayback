#!/usr/bin/env python3
"""Convert markdown documentation to PDF using Python libraries"""

import sys
import os
from pathlib import Path

# Force UTF-8 encoding for console output on Windows
if sys.platform == 'win32':
    sys.stdout.reconfigure(encoding='utf-8')
    sys.stderr.reconfigure(encoding='utf-8')

def check_dependencies():
    """Check if required packages are installed"""
    try:
        import markdown
    except ImportError:
        print("Installing required package: markdown...")
        import subprocess
        subprocess.check_call([sys.executable, '-m', 'pip', 'install', 'markdown'])
        import markdown

    try:
        import xhtml2pdf
    except ImportError:
        print("Installing required package: xhtml2pdf...")
        import subprocess
        subprocess.check_call([sys.executable, '-m', 'pip', 'install', 'xhtml2pdf'])
        import xhtml2pdf

    return True

def markdown_to_html(md_content, title="Documentation"):
    """Convert markdown to HTML with styling"""
    import markdown

    # Convert markdown to HTML
    md = markdown.Markdown(extensions=[
        'extra',
        'codehilite',
        'toc',
        'tables',
        'fenced_code'
    ])
    html_content = md.convert(md_content)

    # Create full HTML document with CSS
    html = f"""
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>{title}</title>
    <style>
        @page {{
            size: letter;
            margin: 1in;
        }}
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 100%;
        }}
        h1, h2, h3, h4, h5, h6 {{
            color: #d32f2f;
            margin-top: 1.5em;
            margin-bottom: 0.5em;
        }}
        h1 {{
            font-size: 2em;
            border-bottom: 2px solid #d32f2f;
            padding-bottom: 0.3em;
        }}
        h2 {{
            font-size: 1.5em;
        }}
        code {{
            background-color: #f5f5f5;
            padding: 2px 4px;
            border-radius: 3px;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
        }}
        pre {{
            background-color: #f5f5f5;
            padding: 10px;
            border-left: 3px solid #d32f2f;
            overflow-x: auto;
            border-radius: 3px;
        }}
        pre code {{
            background-color: transparent;
            padding: 0;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 1em 0;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #d32f2f;
            color: white;
        }}
        tr:nth-child(even) {{
            background-color: #f9f9f9;
        }}
        a {{
            color: #d32f2f;
            text-decoration: none;
        }}
        a:hover {{
            text-decoration: underline;
        }}
        blockquote {{
            border-left: 4px solid #d32f2f;
            padding-left: 1em;
            margin-left: 0;
            color: #666;
        }}
        .toc {{
            background-color: #f9f9f9;
            border: 1px solid #ddd;
            padding: 1em;
            margin-bottom: 2em;
        }}
    </style>
</head>
<body>
    {html_content}
</body>
</html>
"""
    return html

def strip_yaml_frontmatter(content):
    """Remove YAML front matter from markdown content"""
    # Check if content starts with YAML front matter
    if content.startswith('---'):
        # Find the closing '---'
        parts = content.split('---', 2)
        if len(parts) >= 3:
            # Return content after the second '---'
            return parts[2].strip()
    return content

def convert_md_to_pdf(md_file, output_file):
    """Convert a single markdown file to PDF"""
    from xhtml2pdf import pisa

    # Read markdown file
    with open(md_file, 'r', encoding='utf-8') as f:
        md_content = f.read()

    # Strip YAML front matter if present
    md_content = strip_yaml_frontmatter(md_content)

    # Get title from filename
    title = md_file.stem.replace('-', ' ').replace('_', ' ').title()
    title = f"Event Video Playback - {title}"

    # Convert to HTML
    html = markdown_to_html(md_content, title)

    # Convert HTML to PDF
    with open(output_file, 'wb') as pdf_file:
        pisa_status = pisa.CreatePDF(html, dest=pdf_file)

    if pisa_status.err:
        raise Exception(f"PDF generation failed with {pisa_status.err} errors")

def main():
    """Main function"""
    # Check dependencies
    print("Checking dependencies...")
    if not check_dependencies():
        return 1

    print("✓ All dependencies installed\n")

    # Set paths
    script_dir = Path(__file__).parent
    project_root = script_dir.parent
    docs_dir = project_root / 'gh_pages' / '_docs'
    output_dir = script_dir / 'packages' / 'EventvideoPlayback.Documentation' / 'bin'

    # Verify input directory exists
    if not docs_dir.exists():
        print(f"✗ Documentation directory not found: {docs_dir}")
        return 1

    # Create output directory
    output_dir.mkdir(parents=True, exist_ok=True)

    # Get all markdown files
    md_files = sorted(docs_dir.glob('*.md'))

    if not md_files:
        print(f"✗ No markdown files found in {docs_dir}")
        return 1

    print(f"Found {len(md_files)} markdown files")
    print(f"Output directory: {output_dir}\n")

    success = 0
    failed = 0

    # Convert each file
    for md_file in md_files:
        pdf_name = md_file.stem + '.pdf'
        pdf_path = output_dir / pdf_name

        print(f"Converting {md_file.name}...", end=' ')

        try:
            convert_md_to_pdf(md_file, pdf_path)
            print("✓")
            success += 1
        except Exception as e:
            print(f"✗ {e}")
            failed += 1

    # Create combined PDF
    print(f"\nCreating combined PDF...", end=' ')
    try:
        from xhtml2pdf import pisa

        # Combine all markdown content
        combined_content = []
        for md_file in md_files:
            with open(md_file, 'r', encoding='utf-8') as f:
                content = f.read()
                # Strip YAML front matter if present
                content = strip_yaml_frontmatter(content)
                # Add a page break between documents
                combined_content.append(content)
                combined_content.append("\n\n---\n\n")

        # Convert to HTML
        html = markdown_to_html(''.join(combined_content), "Event Video Playback - Complete Documentation")

        # Convert to PDF
        combined_path = output_dir / 'complete-documentation.pdf'
        with open(combined_path, 'wb') as pdf_file:
            pisa_status = pisa.CreatePDF(html, dest=pdf_file)

        if pisa_status.err:
            raise Exception(f"PDF generation failed with {pisa_status.err} errors")

        print("✓")
        success += 1
    except Exception as e:
        print(f"✗ {e}")
        failed += 1

    # Summary
    print(f"\n{'='*40}")
    print(f"Success: {success}, Failed: {failed}")
    print(f"{'='*40}")

    if failed == 0:
        print("\n✓ All PDFs generated successfully!")
        print(f"Output: {output_dir}")

    return 0 if failed == 0 else 1

if __name__ == '__main__':
    sys.exit(main())
