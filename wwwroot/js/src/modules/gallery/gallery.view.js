export const GalleryView = {
    popup: null,
    imageEl: null,
    counterEl: null,
    nextBtn: null,
    prevBtn: null,
    closeBtn: null,

    init() {
        this.popup = document.getElementById('modal-gallery');
        this.imageEl = document.getElementById('galleryCurrentImage');
        this.counterEl = document.getElementById('galleryCounter');
        this.nextBtn = document.querySelector('.gallery-next');
        this.prevBtn = document.querySelector('.gallery-prev');
        this.closeBtn = document.querySelector('#modal-gallery .modal__close');
    },

    open(images, index) {
        this.images = images;
        this.index = index;

        this.update();
        this.popup.classList.add('active');
        document.body.style.overflow = 'hidden';
    },

    close() {
        this.popup.classList.remove('active');
        document.body.style.overflow = 'auto';
    },

    update() {
        this.imageEl.src = this.images[this.index];
        this.counterEl.textContent = `${this.index + 1}/${this.images.length}`;
    },

    next() {
        this.index = (this.index + 1) % this.images.length;
        this.update();
    },

    prev() {
        this.index = (this.index - 1 + this.images.length) % this.images.length;
        this.update();
    }
};
