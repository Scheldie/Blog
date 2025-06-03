export function initModalManager() {
    // Создаем контейнер для модальных окон
    const modalContainer = document.createElement('div');
    modalContainer.id = 'modal-container';
    document.body.appendChild(modalContainer);

    function openEditPopup(postData) {
        modalContainer.innerHTML = '';
        
        const modal = document.createElement('div');
        modal.className = 'modal-overlay active';
        
        let imagesHtml = '';
        postData.images.forEach((image, index) => {
            imagesHtml += `
                <div class="image-preview-item" data-index="${index}">
                    <img src="${image.src}">
                    <button class="remove-image-btn">×</button>
                </div>
            `;
        });
        
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close-btn">&times;</span>
                <h3>Редактирование публикации</h3>
                <div class="edit-form">
                    <label>Заголовок:</label>
                    <input type="text" class="edit-title" value="${postData.title}">
                    
                    <label>Описание:</label>
                    <textarea class="edit-description">${postData.description}</textarea>
                    
                    <label>Изображения (макс. 4):</label>
                    <div class="images-preview" id="edit-images-preview">
                        ${imagesHtml}
                    </div>
                    <input type="file" class="edit-image-input" multiple>
                    
                    <div class="edit-buttons">
                        <button class="delete-post-btn">Удалить пост</button>
                        <div>
                            <button class="cancel-edit">Отмена</button>
                            <button class="save-edit">Сохранить</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        modalContainer.appendChild(modal);
        
        // Обработчики событий
        const closeModal = () => {
            modal.classList.remove('active');
            setTimeout(() => modalContainer.innerHTML = '', 300);
        };
        
        modal.querySelector('.close-btn').addEventListener('click', closeModal);
        modal.querySelector('.cancel-edit').addEventListener('click', closeModal);
        
        // Удаление поста
        modal.querySelector('.delete-post-btn').addEventListener('click', () => {
            postData.onDelete();
            closeModal();
        });
        
        // Сохранение изменений
        modal.querySelector('.save-edit').addEventListener('click', () => {
            const newData = {
                title: modal.querySelector('.edit-title').value,
                description: modal.querySelector('.edit-description').value,
                images: postData.images
            };
            postData.onSave(newData);
            closeModal();
        });
        
        // Удаление отдельных изображений
        modal.querySelectorAll('.remove-image-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                const item = this.closest('.image-preview-item');
                const index = parseInt(item.dataset.index);
                
                // Удаляем изображение из массива
                postData.images.splice(index, 1);
                
                // Обновляем индексы у оставшихся элементов
                const items = modal.querySelectorAll('.image-preview-item');
                items.forEach((el, i) => {
                    el.dataset.index = i;
                });
                
                // Удаляем элемент из DOM
                item.remove();
            });
        });
        
        // Добавление новых изображений
        modal.querySelector('.edit-image-input').addEventListener('change', function(e) {
            const files = Array.from(e.target.files);
            if (files.length + postData.images.length > 4) {
                this.value = '';
                openAlert('Ошибка', 'Можно загрузить не более 4 изображений');
                return;
            }
            
            files.forEach(file => {
                const reader = new FileReader();
                reader.onload = function(e) {
                    const index = postData.images.length;
                    postData.images.push({ src: e.target.result, isFile: true });
                    
                    const previewItem = document.createElement('div');
                    previewItem.className = 'image-preview-item';
                    previewItem.dataset.index = index;
                    previewItem.innerHTML = `
                        <img src="${e.target.result}">
                        <button class="remove-image-btn">×</button>
                    `;
                    
                    previewItem.querySelector('.remove-image-btn').addEventListener('click', function() {
                        const itemIndex = parseInt(previewItem.dataset.index);
                        postData.images.splice(itemIndex, 1);
                        previewItem.remove();
                    });
                    
                    document.getElementById('edit-images-preview').appendChild(previewItem);
                };
                reader.readAsDataURL(file);
            });
            
            this.value = '';
        });
    }
    
    function openAlert(title, message) {
        modalContainer.innerHTML = '';
        
        const modal = document.createElement('div');
        modal.className = 'modal-overlay active';
        modal.innerHTML = `
            <div class="modal-content">
                <h3>${title}</h3>
                <p>${message}</p>
                <button class="modal-ok">OK</button>
            </div>
        `;
        
        modalContainer.appendChild(modal);
        modal.querySelector('.modal-ok').addEventListener('click', () => {
            modal.classList.remove('active');
            setTimeout(() => modalContainer.innerHTML = '', 300);
        });
    }
    
    return {
        openEditPopup,
        openAlert
    };
}