import {GalleryView} from "./gallery.view.js";

export function initGalleryController() {
    GalleryView.init();

    document.addEventListener('click', e => {
        if (!e.target.classList.contains('post-image')) return;

        const post = e.target.closest('.publication');
        
        const images = Array.from(post.querySelectorAll('.post-image'))
            .map(img => img.dataset.full);
        
        const index = images.indexOf(e.target.dataset.full);

        GalleryView.open(images, index);
    });

    GalleryView.nextBtn.onclick = () => GalleryView.next();
    GalleryView.prevBtn.onclick = () => GalleryView.prev();
    GalleryView.closeBtn.onclick = () => GalleryView.close();

    GalleryView.popup.addEventListener('click', e => {
        if (e.target.classList.contains('modal__overlay')) {
            GalleryView.close();
        }
    });

    document.addEventListener('keydown', e => {
        if (!GalleryView.popup.classList.contains('active')) return;

        if (e.key === 'ArrowRight') GalleryView.next();
        if (e.key === 'ArrowLeft') GalleryView.prev();
        if (e.key === 'Escape') GalleryView.close();
    });
}
