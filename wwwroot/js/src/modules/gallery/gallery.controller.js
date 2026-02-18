import {GalleryView} from "./gallery.view.js";

export function initGalleryController() {
    GalleryView.init();

    // Клик по изображению поста
    document.addEventListener('click', e => {
        if (!e.target.classList.contains('post-image')) return;

        const post = e.target.closest('.publication');
        const images = Array.from(post.querySelectorAll('.post-image')).map(img => img.src);
        const index = images.indexOf(e.target.src);

        GalleryView.open(images, index);
    });

    // Кнопки
    GalleryView.nextBtn.onclick = () => GalleryView.next();
    GalleryView.prevBtn.onclick = () => GalleryView.prev();
    GalleryView.closeBtn.onclick = () => GalleryView.close();

    // Клик по оверлею
    GalleryView.popup.addEventListener('click', e => {
        if (e.target.classList.contains('modal__overlay')) {
            GalleryView.close();
        }
    });

    // Клавиатура
    document.addEventListener('keydown', e => {
        if (!GalleryView.popup.classList.contains('active')) return;

        if (e.key === 'ArrowRight') GalleryView.next();
        if (e.key === 'ArrowLeft') GalleryView.prev();
        if (e.key === 'Escape') GalleryView.close();
    });
}
