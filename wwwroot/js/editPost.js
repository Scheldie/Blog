document.addEventListener('DOMContentLoaded', function () {
    const editPopup = document.getElementById('edit-popup-overlay');
    const cancelBtn = editPopup.querySelector('.cancel-edit');
    const saveBtn = editPopup.querySelector('.save-edit');
    const deleteBtn = editPopup.querySelector('.delete-post-btn');
    const editImageInput = editPopup.querySelector('.edit-image-input');
    const editImagesPreview = editPopup.querySelector('#edit-images-preview');

    let currentPostId = null;
    let currentImages = [];

    // Закрытие попапа
    cancelBtn.addEventListener('click', () => {
        editPopup.style.display = 'none';
    });

    // Обработка выбора новых изображений
    editImageInput.addEventListener('change', function (e) {
        if (this.files.length + currentImages.length > 4) {
            alert('Можно загрузить не более 4 изображений');
            this.value = '';
            return;
        }

        Array.from(this.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (event) {
                const imgContainer = document.createElement('div');
                imgContainer.className = 'preview-image';
                imgContainer.innerHTML = `
                    <img src="${event.target.result}" alt="Preview">
                    <span class="remove-image">&times;</span>
                `;

                imgContainer.querySelector('.remove-image').addEventListener('click', () => {
                    imgContainer.remove();
                    currentImages = currentImages.filter(img => img !== file);
                });

                editImagesPreview.appendChild(imgContainer);
                currentImages.push(file);
            };
            reader.readAsDataURL(file);
        });

        this.value = '';
    });

    // Сохранение изменений
    saveBtn.addEventListener('click', async function () {
        const description = editPopup.querySelector('.edit-description').value.trim();
        const title = editPopup.querySelector('.edit-title').value.trim();

        if (!title || !description) {
            alert('Пожалуйста, заполните заголовок и описание');
            return;
        }

        const formData = new FormData();
        formData.append('PostId', currentPostId);
        formData.append('Description', description);
        formData.append('Title', title);

        for (let i = 0; i < currentImages.length; i++) {
            if (currentImages[i] instanceof File) {
                formData.append('NewImageFiles', currentImages[i]);
            }
        }

        try {
            const response = await fetch('/Profile/EditPost', {
                method: 'POST',
                body: formData
            });

            const data = await response.json();

            if (data.success) {
                // Перезагружаем страницу для отображения изменений
                location.reload();
            } else {
                alert('Ошибка при сохранении изменений');
            }
        } catch (error) {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при отправке данных');
        }
    });

    // Удаление поста
    deleteBtn.addEventListener('click', async function () {
        if (!confirm('Вы уверены, что хотите удалить этот пост?')) {
            return;
        }

        try {
            const response = await fetch('/Profile/DeletePost', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ postId: parseInt(currentPostId) })
            });

            const data = await response.json();

            if (data.success) {
                // Перезагружаем страницу
                location.reload();
            } else {
                alert('Ошибка при удалении поста');
            }
        } catch (error) {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при удалении поста');
        }
    });

    // Функция для открытия попапа редактирования
    window.openEditPostPopup = function (postId) {

        console.log('Trying to open popup for post ID:', postId);
        console.log('Found post element:', document.querySelector(`.publication[data-post-id="post-${postId}"]`));
        currentPostId = postId;
        const postElement = document.querySelector(`.publication[data-post-id="post-${postId}"]`);

        if (!postElement) {
            console.error('Post element not found');
            return;
        }

        // Заполняем данные
        const titleElement = postElement.querySelector('.publication-title');
        const descriptionElement = postElement.querySelector('.publication-description');

        editPopup.querySelector('.edit-title').value = titleElement ? titleElement.textContent : '';
        editPopup.querySelector('.edit-description').value = descriptionElement ? descriptionElement.textContent : '';

        // Очищаем превью
        editImagesPreview.innerHTML = '';
        currentImages = [];

        // Добавляем текущие изображения
        const images = postElement.querySelectorAll('.post-image');
        images.forEach(img => {
            const imgContainer = document.createElement('div');
            imgContainer.className = 'preview-image';
            imgContainer.innerHTML = `
            <img src="${img.src}" alt="Preview">
            <span class="remove-image">&times;</span>
        `;

            imgContainer.querySelector('.remove-image').addEventListener('click', () => {
                imgContainer.remove();
                currentImages = currentImages.filter(i => i.src !== img.src);
            });

            editImagesPreview.appendChild(imgContainer);
            currentImages.push({ src: img.src });
        });

        editPopup.style.display = 'flex';
    };
});