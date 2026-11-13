// motion.js — one motion controller for the whole app.
// Tokens come from tokens.css (--motion-*). Reduced-motion disables transforms globally in CSS.
document.documentElement.classList.add('js');

const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)');
const apply = () => document.documentElement.toggleAttribute('data-reduced-motion', prefersReduced.matches);
apply();
prefersReduced.addEventListener?.('change', apply);

// Auto-reveal: key blocks rise in on first view (opt out with data-reveal="off").
const autoSelectors = [
    '.page-head', '.home-band', '.cards', '.site-index', '.gallery-grid',
    '.footage-grid', '.auth-cols', '.detail-cols'
];
if (!prefersReduced.matches) {
    document.querySelectorAll(autoSelectors.join(','))
        .forEach((el, i) => {
            if (el.getAttribute('data-reveal') === 'off') return;
            el.setAttribute('data-reveal', '');
            el.style.transitionDelay = `${Math.min(i * 45, 270)}ms`;
        });
}

// Reveal-on-first-view hook (atoms opt in with data-reveal). No-op when reduced motion is on.
const revealObserver = ('IntersectionObserver' in window)
    ? new IntersectionObserver((entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                entry.target.setAttribute('data-revealed', '');
                revealObserver.unobserve(entry.target);
            }
        }
    }, { threshold: 0.12 })
    : null;

document.querySelectorAll('[data-reveal]').forEach((el) => {
    if (revealObserver && !prefersReduced.matches) {
        revealObserver.observe(el);
    } else {
        el.setAttribute('data-revealed', '');
    }
});
