document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM fully loaded'); // Отладочное сообщение
    
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

    // Улучшенная обработка файлов
    imageInput.addEventListener('change', function(e) {
        console.log('Files selected:', this.files); // Отладочный вывод
        
        imagesPreview.innerHTML = '';
        
        if (this.files.length > 4) {
            alert('Максимальное количество изображений - 4');
            this.value = '';
            return;
        }

        Array.from(this.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function(event) {
                
                imgContainer.querySelector('.remove-image').addEventListener('click', (e) => {
                    e.stopPropagation();
                    imgContainer.remove();
                    
                    // Обновляем input files
                    const dataTransfer = new DataTransfer();
                    Array.from(imageInput.files)
                        .filter(f => f !== file)
                        .forEach(f => dataTransfer.items.add(f));
                    
                    imageInput.files = dataTransfer.files;
                });
                
                imagesPreview.appendChild(imgContainer);
            };
            reader.readAsDataURL(file);
        });
    });

    
});