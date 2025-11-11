---
layout: home
title: Event Video Playback
description: Transform TwinCAT Vision images into event-driven video recordings
---

<section class="hero">
  <div class="hero-content">
    <img src="{{ '/assets/images/Camera_Hero_NoBackground.png' | relative_url }}" alt="Event Video Playback" class="hero-image">
    <h1 class="hero-title">Event Video Playback</h1>
    <p class="hero-tagline">Transform TwinCAT Vision images into event-driven video recordings</p>
    <div class="hero-buttons">
      <a href="{{ '/docs/getting-started/' | relative_url }}" class="btn btn-primary">Get Started</a>
      <a href="https://github.com/Beckhoff-USA-Community/EventVideoPlayback" target="_blank" class="btn btn-secondary">View on GitHub</a>
    </div>
  </div>
</section>

<section class="section features">
  <div class="container">
    <div class="section-header">
      <h2 class="section-title">Powerful Features for Industrial Automation</h2>
      <p class="section-subtitle">Everything you need to capture, log, and review event-driven video from TwinCAT Vision</p>
    </div>
    <div class="features-grid">
      <div class="feature-card">
        <span class="feature-icon">📹</span>
        <h3 class="feature-title">Event-Driven Capture</h3>
        <p class="feature-description">Automatically capture and compile videos from TwinCAT Vision images when critical events occur. No manual intervention required.</p>
      </div>
      <div class="feature-card">
        <span class="feature-icon">📊</span>
        <h3 class="feature-title">Event Logger Integration</h3>
        <p class="feature-description">Seamlessly integrate with TwinCAT Event Logger for automatic video logging and correlation with system events.</p>
      </div>
      <div class="feature-card">
        <span class="feature-icon">🎮</span>
        <h3 class="feature-title">HMI Playback Control</h3>
        <p class="feature-description">Review and analyze recorded events directly from your HMI with intuitive playback controls and timeline navigation.</p>
      </div>
      <div class="feature-card">
        <span class="feature-icon">🔧</span>
        <h3 class="feature-title">Easy PLC Integration</h3>
        <p class="feature-description">Simple-to-use PLC function blocks make integration into your existing TwinCAT projects quick and straightforward.</p>
      </div>
    </div>
  </div>
</section>

<section class="section section-dark">
  <div class="container">
    <div class="overview">
      <div class="overview-content">
        <h2>Why Event Video Playback?</h2>
        <p>In industrial automation, understanding what happened during critical events is crucial. Event Video Playback bridges the gap between TwinCAT Vision image capture and meaningful video documentation.</p>
        <p><strong>Real-World Example:</strong> When a machine down occurrence is triggered, the system automatically compiles recent images into a video, logs it with Event Logger, and makes it available for review on the HMI. Operators and engineers can quickly diagnose issues without manually searching through thousands of individual images.</p>
        <a href="{{ '/docs/getting-started/' | relative_url }}" class="btn btn-primary" style="margin-top: 1rem;">Learn More</a>
      </div>
      <div class="overview-image">
        <img src="{{ '/assets/images/Camera_Hero_NoBackground.png' | relative_url }}" alt="Event Video Playback Overview">
      </div>
    </div>
  </div>
</section>

<section class="section section-gray">
  <div class="container">
    <div class="section-header">
      <h2 class="section-title">System Requirements</h2>
      <p class="section-subtitle">Everything you need to get started with Event Video Playback</p>
    </div>
    <div class="requirements-list">
      <div class="requirement-item">
        <span class="requirement-icon">✓</span>
        <span class="requirement-text">TwinCAT 3.1 Build 4024 or higher</span>
      </div>
      <div class="requirement-item">
        <span class="requirement-icon">✓</span>
        <span class="requirement-text">TwinCAT Vision 4.0 or higher</span>
      </div>
      <div class="requirement-item">
        <span class="requirement-icon">✓</span>
        <span class="requirement-text">Windows 10/11 or Windows Server 2016+</span>
      </div>
      <div class="requirement-item">
        <span class="requirement-icon">✓</span>
        <span class="requirement-text">.NET 8 Runtime</span>
      </div>
      <div class="requirement-item">
        <span class="requirement-icon">✓</span>
        <span class="requirement-text">FFmpeg (included with service)</span>
      </div>
      <div class="requirement-item">
        <span class="requirement-icon">✓</span>
        <span class="requirement-text">TwinCAT Event Logger (optional)</span>
      </div>
    </div>
    <div style="text-align: center; margin-top: 3rem;">
      <a href="{{ '/docs/getting-started/' | relative_url }}" class="btn btn-primary">View Installation Guide</a>
    </div>
  </div>
</section>

<section class="section">
  <div class="container">
    <div class="section-header">
      <h2 class="section-title">Ready to Get Started?</h2>
      <p class="section-subtitle">Choose your path based on your role</p>
    </div>
    <div class="features-grid">
      <div class="feature-card">
        <span class="feature-icon">📖</span>
        <h3 class="feature-title">PLC Developers</h3>
        <p class="feature-description">Learn how to integrate Event Video Playback function blocks into your TwinCAT PLC projects.</p>
        <a href="{{ '/docs/plc-usage/' | relative_url }}" class="btn btn-ghost" style="margin-top: 1rem;">PLC Library Guide →</a>
      </div>
      <div class="feature-card">
        <span class="feature-icon">⚙️</span>
        <h3 class="feature-title">System Integrators</h3>
        <p class="feature-description">Configure the Windows service, set up video parameters, and troubleshoot installations.</p>
        <a href="{{ '/docs/service-config/' | relative_url }}" class="btn btn-ghost" style="margin-top: 1rem;">Service Config →</a>
      </div>
      <div class="feature-card">
        <span class="feature-icon">🖥️</span>
        <h3 class="feature-title">HMI Developers</h3>
        <p class="feature-description">Add video playback controls to your HMI using the NuGet package for WPF applications.</p>
        <a href="{{ '/docs/hmi-usage/' | relative_url }}" class="btn btn-ghost" style="margin-top: 1rem;">HMI Package Guide →</a>
      </div>
    </div>
  </div>
</section>
