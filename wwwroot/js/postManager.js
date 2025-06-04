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
    }

    function createPost(id, date, title, description, images) {
        const postElement = document.createElement('div');
        postElement.className = 'publication';
        postElement.id = id;

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
                <div class="like-section"> 
                    <span class="like-count">0</span>
                    <img class="like-icon" src="../img/Heart.png" alt="Лайк">
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
        postElement.querySelector('.edit-icon').addEventListener('click', () => {
            currentEditPost = postElement;
            const postData = getPostData(postElement);
            modalManager.openEditPopup(postData);
        });

        commentManager.setupComments(postElement);
        likeManager.setupLikes(postElement);
    }

    function getPostData(postElement) {
        return {
            title: postElement.querySelector('.publication-title').textContent,
            description: postElement.querySelector('.publication-description').textContent,
            images: Array.from(postElement.querySelectorAll('.post-image')).map(img => ({
                src: img.src,
                isFile: false
            })),
            onSave: (newData) => {
                postElement.querySelector('.publication-title').textContent = newData.title;
                postElement.querySelector('.publication-description').textContent = newData.description;

                const imagesContainer = postElement.querySelector('.post-images-container');
                const imagesContainerClass = newData.images.length === 1 ? 'single-image' :
                    newData.images.length === 2 ? 'double-image' :
                        'multiple-images';
                imagesContainer.className = `post-images-container ${imagesContainerClass}`;
                imagesContainer.innerHTML = '';

                newData.images.forEach((image, index) => {
                    const imgContainer = document.createElement('div');
                    imgContainer.className = 'post-image-item';

                    const img = document.createElement('img');
                    img.className = 'post-image';
                    img.alt = `Изображение ${index + 1}`;
                    img.src = image.src;

                    imgContainer.appendChild(img);
                    imagesContainer.appendChild(imgContainer);

                    if (newData.images.length === 1) {
                        img.onload = function () {
                            adjustSingleImageSize(img);
                        };
                    }
                });
            },
            onDelete: () => {
                postElement.remove();
            }
        };
    }

    init();

    return {
        createPost
    };
}