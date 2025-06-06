document.addEventListener('DOMContentLoaded', function() {
    let images = document.querySelectorAll('.carousel-image');
    const prevBtn = document.querySelector('.carousel-button.prev');
    const nextBtn = document.querySelector('.carousel-button.next');
    const popupOverlay = document.getElementById('popup-overlay');
    const popupImage = document.getElementById('popup-image');
    const popupDeleteBtn = document.querySelector('.popup-delete-btn');
    const popupCloseBtn = document.querySelector('.popup-close-btn');
    
    let currentIndex = Array.from(images).findIndex(img => img.classList.contains('active'));
    if (currentIndex === -1) currentIndex = Math.floor(images.length / 2);
    
    function updateCarousel() {
        images.forEach((img, index) => {
            img.classList.remove('active', 'prev', 'next');
            
            if (index === currentIndex) {
                img.classList.add('active');
            } else if (index === currentIndex - 1) {
                img.classList.add('prev');
            } else if (index === currentIndex + 1) {
                img.classList.add('next');
            }
        });
        
        prevBtn.disabled = currentIndex === 0;
        nextBtn.disabled = currentIndex === images.length - 1;
    }
    
    // Переключение фотографий
    prevBtn.addEventListener('click', function() {
        if (currentIndex > 0) {
            currentIndex--;
            updateCarousel();
        }
    });
    
    nextBtn.addEventListener('click', function() {
        if (currentIndex < images.length - 1) {
            currentIndex++;
            updateCarousel();
        }
    });
    
    // Открытие попапа при клике на активное фото
    document.querySelector('.carousel-container').addEventListener('click', function(e) {
        const clickedImage = e.target.closest('.carousel-image.active');
        if (clickedImage) {
            popupImage.src = clickedImage.src;
            popupImage.alt = clickedImage.alt;
            popupOverlay.classList.add('active'); 
        }
    });
    
    // Удаление фото
    popupDeleteBtn.addEventListener('click', function() {
        if (confirm('Вы уверены, что хотите удалить это фото?')) {
            images[currentIndex].remove();
            images = document.querySelectorAll('.carousel-image');
            if (currentIndex >= images.length) currentIndex = images.length - 1;
            updateCarousel();
            popupOverlay.classList.remove('active');
        }
    });
    
    // Закрытие попапа
    popupCloseBtn.addEventListener('click', function() {
        popupOverlay.classList.remove('active');
    });
    
    popupOverlay.addEventListener('click', function(e) {
        if (e.target === popupOverlay) {
            popupOverlay.classList.remove('active');
        }
    });
    
    updateCarousel();
});