export function initGallery() {
    const popup = document.getElementById('galleryPopup');
    const img = document.getElementById('galleryCurrentImage');
    const counter = document.getElementById('galleryCounter');

    let images = [];
    let index = 0;

    function update() {
        img.src = images[index];
        counter.textContent = `${index + 1}/${images.length}`;
    }

    window.openGallery = function(list, start = 0) {
        images = list;
        index = start;
        update();
        popup.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    };

    popup.addEventListener('click', e => {
        if (e.target === popup) close();
    });

    function close() {
        popup.style.display = 'none';
        document.body.style.overflow = '';
    }

    document.querySelector('.gallery-next').onclick = () => {
        index = (index + 1) % images.length;
        update();
    };

    document.querySelector('.gallery-prev').onclick = () => {
        index = (index - 1 + images.length) % images.length;
        update();
    };

    document.querySelector('.close-btn-gallery').onclick = close;
}
