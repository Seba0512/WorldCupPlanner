---
name: Elite Tournament Intelligence
colors:
  surface: '#141314'
  surface-dim: '#141314'
  surface-bright: '#3a3939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1c1b1c'
  surface-container: '#201f20'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353435'
  on-surface: '#e5e2e1'
  on-surface-variant: '#c6c6cb'
  inverse-surface: '#e5e2e1'
  inverse-on-surface: '#313030'
  outline: '#909095'
  outline-variant: '#45474b'
  surface-tint: '#c6c6cc'
  primary: '#c6c6cc'
  on-primary: '#2f3035'
  primary-container: '#0f1115'
  on-primary-container: '#7b7c82'
  inverse-primary: '#5d5e63'
  secondary: '#c4c6ce'
  on-secondary: '#2d3037'
  secondary-container: '#464950'
  on-secondary-container: '#b6b8c0'
  tertiary: '#d1c4bb'
  on-tertiary: '#372f29'
  tertiary-container: '#16100b'
  on-tertiary-container: '#857b73'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#e2e2e8'
  primary-fixed-dim: '#c6c6cc'
  on-primary-fixed: '#1a1c20'
  on-primary-fixed-variant: '#45474b'
  secondary-fixed: '#e1e2ea'
  secondary-fixed-dim: '#c4c6ce'
  on-secondary-fixed: '#191c22'
  on-secondary-fixed-variant: '#44474d'
  tertiary-fixed: '#eee0d7'
  tertiary-fixed-dim: '#d1c4bb'
  on-tertiary-fixed: '#211a15'
  on-tertiary-fixed-variant: '#4e453f'
  background: '#141314'
  on-background: '#e5e2e1'
  surface-variant: '#353435'
typography:
  display-lg:
    fontFamily: Montserrat
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Montserrat
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
  headline-md:
    fontFamily: Montserrat
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  headline-sm:
    fontFamily: Montserrat
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-md:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  container-padding: 24px
  gutter: 16px
  section-gap: 32px
---

## Brand & Style

This design system is engineered for high-stakes sports management, blending the intensity of the World Cup with the precision of premium SaaS analytics. The aesthetic is defined as **Atmospheric Minimalism**—a high-contrast dark mode environment that utilizes glassmorphism to create a sense of depth and focus. 

The target audience consists of tournament directors and logistics coordinators who require immediate clarity amidst complex data. The UI evokes a "Command Center" feel: authoritative, high-end, and technologically advanced. Visual depth is achieved through layered translucency rather than heavy decorative elements, ensuring the "Pitch Green" and "Gold" accents signify importance and success without cluttering the workspace.

## Colors

The palette is anchored in a deep obsidian base to maximize the "stadium-at-night" effect. 

- **Foundation:** The primary background (#0F1115) provides a void-like canvas, while the surface color (#1A1D23) distinguishes cards and navigation elements.
- **Accents:** 
    - **Blue Accent:** Used for primary actions, selection states, and data visualization highlights.
    - **Gold Accent:** Reserved for "Premium" features, trophy-related data, and champion-tier status.
    - **Pitch Green:** Strictly for positive status indicators, pitch-side analytics, and successful completion.
    - **Red:** Used for critical alerts, match cancellations, or high-priority warnings.
- **Overlays:** Semi-transparent versions of the surface color (at 60-80% opacity) are used for glassmorphic effects to maintain legibility.

## Typography

This design system utilizes a dual-font approach to balance athletic energy with administrative clarity. 

**Montserrat** is used for all headings to provide a geometric, bold, and modern sports feel. For larger display sizes, a slight negative letter-spacing is applied to mimic professional broadcast graphics.

**Inter** handles all functional data and body copy. It is chosen for its exceptional legibility in dense data tables and dashboard widgets. Labels and secondary metadata utilize a medium-weight Inter with increased tracking and uppercase styling to ensure they remain distinct from body text.

## Layout & Spacing

The system employs a **12-column fluid grid** for desktop, transitioning to a **4-column grid** for mobile. 

- **Rhythm:** A 4px baseline grid ensures vertical consistency.
- **Margins:** Standard page margins are 24px on desktop and 16px on mobile.
- **Reflow:** On tablet and mobile, sidebar navigation collapses into a bottom bar or a hamburger menu to prioritize the "Scoreboard" view of management data.
- **Dashboard Density:** Components use a "comfortable" spacing model (16px gutters) to avoid visual fatigue, as users are often monitoring the interface for long durations.

## Elevation & Depth

Hierarchy is established through **Backdrop Blurs** and **Subtle Tonal Gradients** rather than traditional elevation.

- **Level 1 (Base):** #0F1115 (Background).
- **Level 2 (Surface):** #1A1D23 (Cards/Sections) with a 1px subtle stroke (#FFFFFF at 5% opacity).
- **Level 3 (Interactive):** Glassmorphism effect (Backdrop-filter: blur(12px)) with a 10% white tint.
- **Shadows:** Soft, expansive shadows are used sparingly. When used, they carry a slight blue tint (rgba(0, 209, 255, 0.15)) to make elements appear as if they are floating above a digital pitch.

## Shapes

The shape language is consistently **Rounded (12px)**. This radius is applied to all primary containers, buttons, and input fields to soften the high-contrast dark environment.

- **Cards/Containers:** 12px (Standard).
- **Large Sections/Modals:** 16px (Rounded-lg).
- **Status Tags/Chips:** Full pill-shape (Rounded-xl) to distinguish them from interactive buttons.
- **Visual Cues:** All iconography should follow a 2px stroke weight with slightly rounded terminals to match the font geometry.

## Components

- **Buttons:** Primary buttons use a linear gradient from #00D1FF to #00A3FF with white text. Secondary buttons are "Ghost" style with a 12px radius and a 1px border.
- **Glass Cards:** Used for real-time stats. Feature a 12px blur, a 1px top-border highlight, and a background of #FFFFFF at 0.03 alpha.
- **Inputs:** Darker than the surface (#0F1115), with 12px rounded corners. The focus state uses a 1px glow of #00D1FF.
- **Match Status Chips:** Small, pill-shaped indicators. "Live" matches use the Pitch Green with a subtle pulse animation.
- **Scoreboard Widgets:** Feature large Montserrat numbers and condensed Inter labels. Use the Gold accent for "Winner" states.
- **Navigation:** Vertical sidebar with active states indicated by a left-aligned 4px vertical bar in Blue Accent.