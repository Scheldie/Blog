import { Modal } from '../../ui/modal.js';

export function initCarousel() {
    let images = Array.from(document.querySelectorAll('.carousel-image'));
    const prevBtn = document.querySelector('.carousel-button.prev');
    const nextBtn = document.querySelector('.carousel-button.next');
    if (!prevBtn || !nextBtn) return; 
    let currentIndex = 1;

    function loadImage(img) {
        if (!img.dataset.src) return;
        img.src = img.dataset.src;
        img.removeAttribute('data-src');
    }

    function updateCarousel() {
        images.forEach((img, index) => {
            img.classList.remove('active', 'prev', 'next');

            if (index === currentIndex) {
                img.classList.add('active');
                loadImage(img); 
            }
            else if (index === currentIndex - 1) {
                img.classList.add('prev');
                loadImage(img); 
            }
            else if (index === currentIndex + 1) {
                img.classList.add('next');
                loadImage(img);
            }
        });

        prevBtn.disabled = currentIndex === 0;
        nextBtn.disabled = currentIndex === images.length - 1;
    }


    prevBtn.onclick = () => {
        if (currentIndex > 0) {
            currentIndex--;
            updateCarousel();
        }
    };

    nextBtn.onclick = () => {
        if (currentIndex < images.length - 1) {
            currentIndex++;
            updateCarousel();
        }
    };

    // === ОТКРЫТИЕ ПОПАПА ПРОСМОТРА ФОТО ===
    document.querySelector('.carousel-container').onclick = e => {
        const active = e.target.closest('.carousel-image.active');
        if (!active) return;

        const postId = active.dataset.postId;

        // открываем новый попап
        const realSrc = active.src || active.dataset.src; 
        window.openPhotoView(realSrc, postId);
    };

    updateCarousel();
}

export function initPhotoView() {
    const modalImage = document.getElementById('photoViewImage');
    const postBtn = document.getElementById('photoViewPostBtn');

    let currentPostId = null;

    // вызывается при клике на фото
    window.openPhotoView = (src, postId) => {
        modalImage.src = src;
        currentPostId = postId;

        Modal.open('modal-photo-view');
    };

    // обработчик кнопки "Перейти к посту"
    postBtn.onclick = async () => {
        if (!currentPostId) return;

        Modal.close('modal-photo-view');

        setTimeout(async () => {
            const post = await loadSinglePost(currentPostId);
            if (!post) return;

            post.scrollIntoView({ behavior: 'smooth', block: 'center' });
            post.classList.add('highlight-post');
            setTimeout(() => post.classList.remove('highlight-post'), 1500);
        }, 200);
    };
}

async function loadSinglePost(postId) {
    const container = document.getElementById('posts-container');
    const sentinel = document.getElementById('posts-sentinel');

    // Проверяем, есть ли пост уже в DOM
    let post = container.querySelector(`.publication[data-post-id="${postId}"]`);
    if (post) return post;

    // Грузим конкретный пост
    const res = await fetch(`/Post/GetPost?id=${postId}`);
    const html = await res.text();

    if (!html.trim()) return null;

    // Вставляем в DOM
    container.insertAdjacentHTML('beforeend', html);
    container.appendChild(sentinel);

    // Ищем снова
    return container.querySelector(`.publication[data-post-id="${postId}"]`);
}


