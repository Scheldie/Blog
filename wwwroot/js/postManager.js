export function initPostManager(modalManager, imageManager, commentManager, likeManager) {
    const postsContainer = document.getElementById('posts-container');
    const addPostForm = document.getElementById('add-publication-form');
    const imageInput = document.getElementById('image');
    const descriptionInput = document.getElementById('add-publication-description');
    const titleInput = document.getElementById('add-publication-title');
    console.log("ModalManager передан:", modalManager);
    let currentEditPost = null;

    async function submitPostToServer(formData) {
        try {
            // Получаем токен или используем пустую строку для отладки
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenElement ? tokenElement.value : '';

            const response = await fetch('/Profile/CreatePost', {
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
            for (let [key, value] of formData.entries()) {
                console.log(key, value);
            }

            try {
                // Отправляем данные на сервер
                const result = await submitPostToServer(formData);

                // Создаем пост на клиенте
                createPost(
                    'post-' + result.postId,
                    new Date().toLocaleDateString(),
                    title,
                    description,
                    images.map(file => ({ src: file, isFile: true }))
                );

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

    function createPost(id, date, title, description, images) {
        const postElement = document.createElement('div');
        postElement.className = 'publication';
        postElement.dataset.postId = id.replace('post-', '');

        const imagesContainerClass = images.length === 1 ? 'single-image' : 'multiple-images';

        postElement.innerHTML = `
            <div class="edit-icon-container">
                <img src="../img/edit-icon.png" class="edit-icon" alt="Редактировать">
            </div>
                <p class="publication-date">${date}</p>
                <h2 class="publication-title">${title}</h2>
                <div class="post-images-container ${imagesContainerClass}"></div>
                <h3 class="publication-description">${description}</h3>
                <div class="post-bottom">
                    <div class="comments-toggle">
                    <img src="../img/Chevron down.png" class="toggle-comments">
                    <span>Комментарии</span>
                </div>
                <div class="like-section" data-post-id="${id.replace('post-', '')}"> 
                    <span class="like-count">${postData?.likesCount || 0}</span>
                    <img class="like-icon" src="${postData?.isLiked ? '/img/Heart Active.png' : '/img/Heart.png'}" alt="Лайк">
                </div>
            </div>
            <div class="comments-section">
                <h3 class="comment-header">Комментарии:</h3>
                <div class="comments-list"></div>
                <textarea class="comment-area" placeholder="Введите ваш комментарий"></textarea>
                <button class="add-comment">Добавить комментарий</button>
            </div>
        `;

        const imagesContainer = postElement.querySelector('.post-images-container');
        images.forEach((image, index) => {
            const imgContainer = document.createElement('div');
            imgContainer.className = 'post-image-item';

            const img = document.createElement('img');
            img.className = 'post-image';
            img.alt = `Изображение ${index + 1}`;
            img.style.display = 'block';
            img.style.margin = '0 auto'; // Центрирование изображения

            if (image.isFile) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    img.src = e.target.result;
                    // Добавляем обработчик загрузки изображения
                    img.onload = function () {
                        adjustImageContainer(imgContainer, img);
                    };
                };
                reader.readAsDataURL(image.src);
            } else {
                img.src = image.src;
                img.onload = function () {
                    adjustImageContainer(imgContainer, img);
                };
            }

            imgContainer.appendChild(img);
            imagesContainer.appendChild(imgContainer);
        });

        postsContainer.prepend(postElement);
        setupPostEvents(postElement);

        return postElement;
    }

    function adjustImageContainer(container, imgElement) {
        const containerRatio = container.offsetWidth / container.offsetHeight;
        const imgRatio = imgElement.naturalWidth / imgElement.naturalHeight;

        if (imgRatio > containerRatio) {
            // Широкое изображение
            imgElement.style.width = '100%';
            imgElement.style.height = 'auto';
        } else {
            // Высокое изображение
            imgElement.style.width = 'auto';
            imgElement.style.height = '100%';
        }
    }

    function setupPostEvents(postElement) {
        const editIcon = postElement.querySelector('.edit-icon');

        if (!editIcon) {
            console.error('Иконка редактирования не найдена в посте:', postElement);
            return;
        }

        editIcon.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            console.log('Клик по иконке редактирования!');

            const postData = getPostData(postElement);
            console.log('Данные поста:', postData);

            if (modalManager && typeof modalManager.openEditPopup === 'function') {
                modalManager.openEditPopup(postData);
            } else {
                console.error('modalManager не инициализирован');
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