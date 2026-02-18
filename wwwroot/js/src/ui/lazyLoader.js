export function initLazyImages() {
    const lazyImages = document.querySelectorAll('img.lazy');

    if (!lazyImages.length) return;

    const lazyObserver = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                lazyObserver.unobserve(img);
            }
        });
    });

    lazyImages.forEach(img => lazyObserver.observe(img));
}
