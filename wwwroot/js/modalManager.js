export function initModalManager() {
    const modalContainer = document.getElementById('edit-popup-container');
    const modalOverlay = document.getElementById('edit-popup-overlay');
    const modalContent = document.getElementById('edit-popup-content');
    const closeBtn = document.querySelector('.cancel-edit');
    const saveBtn = document.querySelector('.save-edit');
    const deleteBtn = document.getElementById('delete-post-btn');

    if (!modalContainer || !modalOverlay || !modalContent) {
        console.error('Modal elements not found');
        return {
            openEditPopup: () => console.error('Modal not initialized'),
            closeModal: () => console.error('Modal not initialized')
        };
    }

    let currentPostData = null;

    function openEditPopup(postData) {
        currentPostData = postData;
        const deletedFilesPaths = [];
        currentPostData.deletedFilesPaths = [];
        document.getElementById('edit-title').value = postData.title || '';
        document.getElementById('edit-description').value = postData.description || '';
        document.getElementById('edit-post-id').value = postData.postId || '';

        console.log(postData.postId, postData.title, postData.description);
        const imageInput = document.getElementById('edit-image-input');
        if (imageInput) {
            imageInput.addEventListener('change', function (e) {
                const files = Array.from(e.target.files);
                if (files.length > 0) {
                    files.forEach(file => {
                        const reader = new FileReader();
                        reader.onload = function (e) {
                            const imgElement = document.createElement('div');
                            imgElement.className = 'image-preview-item';
                            imgElement.innerHTML = `
                            <img src="${e.target.result}" class="preview-image">
                            <button class="remove-image-btn">×</button>
                        `;
                            previewContainer.appendChild(imgElement);
                        };
                        reader.readAsDataURL(file);
                    });
                }
            });
        }

        const previewContainer = document.getElementById('edit-images-preview');
        previewContainer.innerHTML = '';
        postData.images.forEach(img => {
            const imgElement = document.createElement('div');
            imgElement.className = 'image-preview-item';
            imgElement.innerHTML = `
                <img src="${img.src}" class="preview-image">
                <button class="remove-image-btn">×</button>
            `;
            previewContainer.appendChild(imgElement);

        });
        if (!imageInput || !previewContainer) {
            console.log('это imageInput или preview');
        }
        previewContainer.addEventListener('click', function (e) {
            if (e.target.classList.contains('remove-image-btn')) {
                e.preventDefault();
                e.stopPropagation();

                const imageItem = e.target.closest('.image-preview-item');
                if (imageItem) {
                    const imgElement = imageItem.querySelector('img');
                    if (imgElement) {
                        if (imgElement.src.startsWith('http')) {
                            const url = new URL(imgElement.src);
                            // Добавляем в массив currentPostData
                            currentPostData.deletedFilesPaths.push(url.pathname);
                        }
                    }
                    imageItem.style.opacity = '0';
                    setTimeout(() => imageItem.remove(), 300);
                }
            }
        });

        modalContainer.style.display = 'flex';
        setTimeout(() => {
            modalOverlay.classList.add('active');
        }, 10);
    }

    function closeModal() {
        modalOverlay.classList.remove('active');
        setTimeout(() => {
            modalContainer.style.display = 'none';
            currentPostData = null;
        }, 300);
    }

    // Исправленный обработчик клика по оверлею
    modalOverlay.addEventListener('click', function (e) {
        if (e.target === this) { // Проверяем, что кликнули именно на оверлей
            closeModal();
        }
    });

    // Обработчики кнопок
    if (closeBtn) closeBtn.addEventListener('click', closeModal);

    if (saveBtn) {
        saveBtn.addEventListener('click', async function () {
            try {
                const formData = new FormData();

                // Обязательные поля
                const postId = document.getElementById('edit-post-id').value;
                const title = document.getElementById('edit-title').value;
                const description = document.getElementById('edit-description').value;

                if (!postId || !title || !description) {
                    throw new Error('Заполните все обязательные поля');
                }

                formData.append('PostId', postId);
                formData.append('Title', title);
                formData.append('Description', description);

                // Удаленные изображения
                if (currentPostData.deletedFilesPaths && currentPostData.deletedFilesPaths.length > 0) {
                    formData.append('DeleteExistingImages', 'true');
                    // Добавляем каждый путь отдельно
                    currentPostData.deletedFilesPaths.forEach(path => {
                        formData.append('DeletedFilesPaths', path);
                    });
                } else {
                    formData.append('DeleteExistingImages', 'false');
                }
                console.log('Deleting images:', currentPostData.deletedFilesPaths);

                // Новые изображения
                const imageInput = document.getElementById('edit-image-input');
                if (imageInput && imageInput.files.length > 0) {
                    Array.from(imageInput.files).forEach(file => {
                        formData.append('NewImageFiles', file);
                    });
                }

                // Отправка на сервер
                const response = await fetch('/Profile/EditPost', {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });

                if (!response.ok) {
                    const error = await response.text();
                    throw new Error(error);
                }

                if (currentPostData.onSave) {
                currentPostData.onSave({ 
                    title, 
                    description,
                    deletedFilesPaths: currentPostData.deletedFilesPaths 
                });
            }

                closeModal();
            } catch (error) {
                console.error('Ошибка сохранения:', error);
                alert('Ошибка при сохранении: ' + error.message);
            }
        });
    }

    if (deleteBtn) {
        deleteBtn.addEventListener('click', async function () {
            if (!currentPostData || !confirm('Вы уверены, что хотите удалить этот пост?')) return;

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const response = await fetch(`/Profile/DeletePost?postId=${currentPostData.postId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });

                if (!response.ok) {
                    const error = await response.text();
                    throw new Error(error || 'Ошибка сервера');
                }

                if (currentPostData.onDelete) {
                    currentPostData.onDelete();
                }
                closeModal();
            } catch (error) {
                console.error('Ошибка удаления:', error);
                alert('Ошибка при удалении: ' + error.message);
            }
        });
    }

    // Обработчик Esc
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && modalContainer.style.display === 'flex') {
            closeModal();
        }
    });

    return {
        openEditPopup,
        closeModal
    };
}