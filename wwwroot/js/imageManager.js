export function initImageManager() {
    let selectedImages = [];
    
    function handleImageSelection(event) {
        const files = Array.from(event.target.files);
        if (files.length > 4) {
            return { error: 'Можно загрузить не более 4 изображений' };
        }
        selectedImages = files;
        return { images: selectedImages };
    }
    
    function updateImagesPreview(containerId, images) {
        const container = document.getElementById(containerId);
        container.innerHTML = '';
        
        images.forEach((image, index) => {
            const previewItem = document.createElement('div');
            previewItem.className = 'image-preview-item';
            
            if (image instanceof File) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    previewItem.innerHTML = `
                        <img src="${e.target.result}">
                        <button class="remove-image-btn" data-index="${index}">×</button>
                    `;
                    container.appendChild(previewItem);
                };
                reader.readAsDataURL(image);
            } else {
                previewItem.innerHTML = `
                    <img src="${image.src}">
                    <button class="remove-image-btn" data-index="${index}">×</button>
                `;
                container.appendChild(previewItem);
            }
        });
    }
    
    function getSelectedImages() {
        return selectedImages;
    }
    
    function clearSelectedImages() {
        selectedImages = [];
    }
    
    function clearImagesPreview(containerId) {
        document.getElementById(containerId).innerHTML = '';
    }
    
    return {
        handleImageSelection,
        updateImagesPreview,
        getSelectedImages,
        clearSelectedImages,
        clearImagesPreview
    };
}