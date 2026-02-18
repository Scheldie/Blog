export function initGalleryForNewPosts(root = document) {
    root.querySelectorAll('.post-image').forEach(img => {
        if (img.dataset.bound) return;
        img.dataset.bound = "1";

        img.addEventListener('click', () => {
            openGallery(img);
        });
    });
}
