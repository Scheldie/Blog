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
    imageInput.addEventListener('change', function (e) {
        console.log('Files selected:', this.files); // Отладочный вывод

        imagesPreview.innerHTML = '';

        if (this.files.length > 4) {
            alert('Максимальное количество изображений - 4');
            this.value = '';
            return;
        }

        Array.from(this.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (event) {

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

    // Отправка формы
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const title = titleInput.value.trim();
        const description = descriptionInput.value.trim();
        const images = imageInput.files;

        if (!title || !description) {
            alert('Пожалуйста, заполните заголовок и описание');
            return;
        }

        if (!images || images.length === 0) {
            alert('Пожалуйста, добавьте хотя бы одно изображение');
            return;
        }

        const formData = new FormData();
        formData.append('Title', title);
        formData.append('Description', description);

        for (let i = 0; i < images.length; i++) {
            formData.append('ImageFiles', images[i]);
        }

        try {
            const response = await fetch('Profile/CreatePost', {
                method: 'POST',
                body: formData
            });

            const data = await response.json();

            if (data.success) {
                // Закрываем попап и очищаем форму
                popup.style.display = 'none';
                form.reset();
                imagesPreview.innerHTML = '';
                document.body.style.overflow = 'auto';

                // Перезагружаем страницу для отображения нового поста
                location.reload();
            } else {
                alert('Ошибка при создании поста');
            }
        } catch (error) {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при отправке данных');
        }
    });
});