function initPostActions() {
    // Редактирование поста
    document.querySelectorAll('.publication-edit-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const postId = this.dataset.postId;
            openEditPostPopup(postId);
        });
    });

    // Лайки
    document.querySelectorAll('.like-btn').forEach(btn => {
        btn.addEventListener('click', async function () {
            const postId = this.dataset.postId;
            const likeIcon = this.querySelector('.like-icon');
            const likeCount = this.querySelector('.like-count');

            try {
                const response = await fetch('/Profile/ToggleLike', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ postId })
                });

                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }

                const data = await response.json();

                if (data.success) {
                    likeIcon.src = data.isLiked ? '../img/Heart Active.png' : '../img/Heart.png';
                    likeCount.textContent = data.likesCount;
                }
            } catch (error) {
                handleApiError(error);
            }
        });
    });
}

function openEditPostPopup(postId) {
    const popup = document.getElementById('edit-popup-overlay');
    const postElement = document.querySelector(`.publication[data-post-id="${postId}"]`);

    // Заполняем данные
    document.getElementById('edit-post-id').value = postId;
    document.getElementById('edit-description').value = postElement.querySelector('.publication-text').textContent;

    // Очищаем и заполняем превью изображений
    const imagesPreview = document.getElementById('edit-images-preview');
    imagesPreview.innerHTML = '';

    postElement.querySelectorAll('.publication-image').forEach(img => {
        const imgContainer = document.createElement('div');
        imgContainer.className = 'image-preview-container';

        const imgElement = document.createElement('img');
        imgElement.src = img.src;
        imgElement.className = 'preview-image';

        const checkbox = document.createElement('input');
        checkbox.type = 'checkbox';
        checkbox.className = 'delete-image-checkbox';
        checkbox.dataset.imagePath = img.src;

        const label = document.createElement('label');
        label.textContent = 'Удалить';

        imgContainer.appendChild(imgElement);
        imgContainer.appendChild(checkbox);
        imgContainer.appendChild(label);
        imagesPreview.appendChild(imgContainer);
    });

    // Показываем попап
    popup.style.display = 'block';

    // Обработчики кнопок
    document.querySelector('.cancel-edit').addEventListener('click', () => {
        popup.style.display = 'none';
    });

    document.getElementById('delete-post-btn').addEventListener('click', async () => {
        if (confirm('Вы уверены, что хотите удалить этот пост?')) {
            try {
                const response = await fetch('/Profile/DeletePost', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ postId })
                });

                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }

                const data = await response.json();

                if (data.success) {
                    showNotification('Пост успешно удален');
                    postElement.remove();
                    popup.style.display = 'none';
                }
            } catch (error) {
                handleApiError(error);
            }
        }
    });

    document.querySelector('.save-edit').addEventListener('click', async () => {
        const description = document.getElementById('edit-description').value;
        const imageInput = document.getElementById('edit-image-input');
        const deleteExistingImages = document.querySelectorAll('.delete-image-checkbox:checked').length > 0;

        const formData = new FormData();
        formData.append('PostId', postId);
        formData.append('Description', description);
        formData.append('DeleteExistingImages', deleteExistingImages);

        for (let i = 0; i < imageInput.files.length; i++) {
            formData.append('NewImageFiles', imageInput.files[i]);
        }

        try {
            const response = await fetch('/Profile/EditPost', {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const data = await response.json();

            if (data.success) {
                showNotification('Пост успешно обновлен');
                // Обновляем данные на странице
                postElement.querySelector('.publication-text').textContent = description;
                // Здесь нужно обновить изображения, но проще перезагрузить страницу
                window.location.reload();
            }
        } catch (error) {
            handleApiError(error);
        }
    });
}