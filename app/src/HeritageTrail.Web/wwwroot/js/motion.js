// motion.js — one motion controller for the whole app.
// Tokens come from tokens.css (--motion-*). Reduced-motion disables transforms globally in CSS.
document.documentElement.classList.add('js');

const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)');
const apply = () => document.documentElement.toggleAttribute('data-reduced-motion', prefersReduced.matches);
apply();
prefersReduced.addEventListener?.('change', apply);

// Reveal-on-first-view hook (atoms opt in with data-reveal). No-op when reduced motion is on.
const revealObserver = ('IntersectionObserver' in window)
    ? new IntersectionObserver((entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                entry.target.setAttribute('data-revealed', '');
                revealObserver.unobserve(entry.target);
            }
        }
    }, { threshold: 0.2 })
    : null;

document.querySelectorAll('[data-reveal]').forEach((el) => {
    if (revealObserver && !prefersReduced.matches) {
        revealObserver.observe(el);
    } else {
        el.setAttribute('data-revealed', '');
    }
});
