document.addEventListener('DOMContentLoaded', function() {
  // Элементы галереи
  const galleryPopup = document.getElementById('galleryPopup');
  const galleryImage = document.getElementById('galleryCurrentImage');
  const galleryCounter = document.getElementById('galleryCounter');
  const closeBtn = document.querySelector('.close-btn-gallery');
  const nextBtn = document.querySelector('.gallery-next');
  const prevBtn = document.querySelector('.gallery-prev');
  
  // Переменные для хранения данных галереи
  let currentImages = [];
  let currentIndex = 0;
  
  // Функция открытия галереи
  window.openGallery = function(images, index = 0) {
    currentImages = images;
    currentIndex = index;
    updateGallery();
    galleryPopup.style.display = 'flex';
    document.body.style.overflow = 'hidden';
  };
  
  // Функция закрытия галереи
  function closeGallery() {
    galleryPopup.style.display = 'none';
    document.body.style.overflow = 'auto';
  }
  
  // Обновление отображаемого изображения
  function updateGallery() {
    galleryImage.src = currentImages[currentIndex];
    galleryCounter.textContent = `${currentIndex + 1}/${currentImages.length}`;
  }
  
  // Переключение на следующее изображение
  function nextImage() {
    currentIndex = (currentIndex + 1) % currentImages.length;
    updateGallery();
  }
  
  // Переключение на предыдущее изображение
  function prevImage() {
    currentIndex = (currentIndex - 1 + currentImages.length) % currentImages.length;
    updateGallery();
  }
  
  // Обработчики событий
  closeBtn.addEventListener('click', closeGallery);
  nextBtn.addEventListener('click', nextImage);
  prevBtn.addEventListener('click', prevImage);
  
  galleryPopup.addEventListener('click', function(e) {
    if (e.target === galleryPopup) {
      closeGallery();
    }
  });
  
  document.addEventListener('keydown', function(e) {
    if (galleryPopup.style.display === 'flex') {
      if (e.key === 'ArrowRight') nextImage();
      if (e.key === 'ArrowLeft') prevImage();
      if (e.key === 'Escape') closeGallery();
    }
  });
  
  // Автоматически находим все изображения постов и добавляем обработчики
  document.addEventListener('click', function(e) {
    if (e.target.classList.contains('post-image')) {
      const post = e.target.closest('.publication');
      if (post) {
        const images = Array.from(post.querySelectorAll('.post-image')).map(img => img.src);
        const clickedIndex = Array.from(post.querySelectorAll('.post-image')).indexOf(e.target);
        openGallery(images, clickedIndex);
      }
    }
  });
});