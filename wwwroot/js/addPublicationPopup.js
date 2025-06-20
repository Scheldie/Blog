document.addEventListener('DOMContentLoaded', () => {
    
    // Элементы
    const addBtn = document.getElementById('add-publication-button');
    const popup = document.getElementById('add-publication-popup');
    const closeBtn = popup?.querySelector('.close-btn-add');
    const form = document.getElementById('add-publication-form');
    const imageInput = document.getElementById('image');
    const imagesPreview = document.getElementById('images-preview');


    // Управление попапом
    const showPopup = () => {
        popup.classList.add('active');
        document.body.style.overflow = 'hidden';
    };

    const hidePopup = () => {
        popup.classList.remove('active');
        document.body.style.overflow = '';
    };

    addBtn.addEventListener('click', showPopup);
    closeBtn.addEventListener('click', hidePopup);
    popup.addEventListener('click', (e) => e.target === popup && hidePopup());

    imageInput.addEventListener('change', function(e) {
        
        imagesPreview.innerHTML = '';
        
        if (this.files.length > 4) {
            alert('Максимальное количество изображений - 4');
            this.value = '';
            return;
        }
    });

    
});