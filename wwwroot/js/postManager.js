export function initPostManager(modalManager, imageManager, commentManager, likeManager) {
    const postsContainer = document.getElementById('posts-container');
    const addPostForm = document.getElementById('add-publication-form');
    const imageInput = document.getElementById('image');
    const descriptionInput = document.getElementById('add-publication-description');
    const titleInput = document.getElementById('add-publication-title');
    let currentEditPost = null;

    async function submitPostToServer(formData) {
        try {
            // Получаем токен или используем пустую строку для отладки
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenElement ? tokenElement.value : '';

            const response = await fetch('/Post/CreatePost', {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': token
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Ошибка сервера: ${response.status} - ${errorText}`);
            }

            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.includes('application/json')) {
                const text = await response.text();
                throw new Error(`Ожидался JSON, но получен: ${contentType}. Ответ: ${text}`);
            }

            return await response.json();
        } catch (error) {
            console.error('Ошибка:', error);
            modalManager.openAlert('Ошибка', error.message || 'Не удалось сохранить пост');
            throw error;
        }
    }

    function init() {
        addPostForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            if (!addPostForm || !imageInput || !descriptionInput || !titleInput) {
                console.error('Не найдены необходимые элементы формы');
                return;
            }
            const description = descriptionInput.value.trim();
            const title = titleInput.value.trim();
            const images = imageManager.getSelectedImages();

            if (!description || images.length === 0) {
                modalManager.openAlert('Ошибка', 'Добавьте описание и хотя бы одно изображение');
                return;
            }

            // FormData для отправки на сервер
            const formData = new FormData();
            formData.append('Description', description);
            formData.append('Title', title);

            // Добавляем все выбранные изображения
            images.forEach((file, index) => {
                formData.append('ImageFiles', file);
            });

            try {
                // Отправляем данные на сервер
                const result = await submitPostToServer(formData);

                document.getElementById('add-publication-popup').classList.remove('active');
                document.body.style.overflow = '';
                

                // Очистка формы
                addPostForm.reset();
                imageManager.clearImagesPreview('images-preview');
                imageManager.clearSelectedImages();
            } catch (error) {
            }
        });

        imageInput.addEventListener('change', function (e) {
            const result = imageManager.handleImageSelection(e);
            if (result.error) {
                modalManager.openAlert('Ошибка', result.error);
                e.target.value = '';
            } else {
                imageManager.updateImagesPreview('images-preview', result.images);
            }
        });
        document.getElementById('posts-container').addEventListener('click', function (e) {
            // Проверяем, был ли клик по иконке или её контейнеру
            const editIcon = e.target.closest('.edit-icon, .edit-icon-container');

            if (editIcon) {
                e.preventDefault();
                e.stopPropagation();

                const postElement = editIcon.closest('.publication');
                const postData = getPostData(postElement);


                if (modalManager && typeof modalManager.openEditPopup === 'function') {
                    modalManager.openEditPopup(postData);
                } else {
                    console.error('ModalManager не инициализирован');
                }
            }
        });
    }


    function getPostData(postElement) {
        return {
            postId: postElement.dataset.postId,
            title: postElement.querySelector('.publication-title')?.textContent || '',
            description: postElement.querySelector('.publication-description')?.textContent || '',
            images: Array.from(postElement.querySelectorAll('.post-image')).map(img => ({
                src: img.src,
                isFile: false
            })),
            onSave: function (newData) {
                // Обновляем UI
                postElement.querySelector('.publication-title').textContent = newData.title;
                postElement.querySelector('.publication-description').textContent = newData.description;
            },
            onDelete: function () {
                postElement.remove();
            }
        };
    }

    init();

    return {
        createPost
    };
}