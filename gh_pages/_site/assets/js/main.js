// Mobile Menu Toggle
document.addEventListener('DOMContentLoaded', function() {
  const mobileMenuToggle = document.querySelector('.mobile-menu-toggle');
  const siteNav = document.querySelector('.site-nav');

  if (mobileMenuToggle) {
    mobileMenuToggle.addEventListener('click', function() {
      siteNav.classList.toggle('active');
      this.innerHTML = siteNav.classList.contains('active') ? '✕' : '☰';
    });

    // Close menu when clicking a link
    const navLinks = document.querySelectorAll('.site-nav a');
    navLinks.forEach(link => {
      link.addEventListener('click', function() {
        siteNav.classList.remove('active');
        mobileMenuToggle.innerHTML = '☰';
      });
    });

    // Close menu when clicking outside
    document.addEventListener('click', function(event) {
      if (!siteNav.contains(event.target) && !mobileMenuToggle.contains(event.target)) {
        siteNav.classList.remove('active');
        mobileMenuToggle.innerHTML = '☰';
      }
    });
  }

  // Smooth scroll for anchor links
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
      const href = this.getAttribute('href');
      if (href !== '#') {
        e.preventDefault();
        const target = document.querySelector(href);
        if (target) {
          const offsetTop = target.offsetTop - 70; // Account for fixed header
          window.scrollTo({
            top: offsetTop,
            behavior: 'smooth'
          });
        }
      }
    });
  });

  // Header shadow on scroll
  const header = document.querySelector('.site-header');
  let lastScrollTop = 0;

  window.addEventListener('scroll', function() {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    if (scrollTop > 50) {
      header.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.3)';
    } else {
      header.style.boxShadow = '0 2px 8px rgba(0, 0, 0, 0.15)';
    }

    lastScrollTop = scrollTop;
  });

  // Fade in elements on scroll
  const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
  };

  const observer = new IntersectionObserver(function(entries) {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.style.opacity = '1';
        entry.target.style.transform = 'translateY(0)';
      }
    });
  }, observerOptions);

  // Observe feature cards and other elements
  const animatedElements = document.querySelectorAll('.feature-card, .overview, .requirement-item');
  animatedElements.forEach(el => {
    el.style.opacity = '0';
    el.style.transform = 'translateY(30px)';
    el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
    observer.observe(el);
  });

  // Add loading animation complete
  document.body.classList.add('loaded');
});

// Copy code button for code blocks
document.querySelectorAll('pre code').forEach((block) => {
  const button = document.createElement('button');
  button.className = 'copy-code-btn';
  button.textContent = 'Copy';
  button.addEventListener('click', () => {
    navigator.clipboard.writeText(block.textContent).then(() => {
      button.textContent = 'Copied!';
      setTimeout(() => {
        button.textContent = 'Copy';
      }, 2000);
    });
  });
  block.parentElement.style.position = 'relative';
  block.parentElement.appendChild(button);
});
