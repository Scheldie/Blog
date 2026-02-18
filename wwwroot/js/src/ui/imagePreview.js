export const ImagePreview = {
    render(container, files, onRemove) {
        container.innerHTML = '';

        files.forEach((file, index) => {
            const item = document.createElement('div');
            item.className = 'image-preview-item';

            const img = document.createElement('img');
            img.className = 'preview-image';

            if (file instanceof File) {
                const reader = new FileReader();
                reader.onload = e => img.src = e.target.result;
                reader.readAsDataURL(file);
            } else {
                img.src = file.src;
            }

            const removeBtn = document.createElement('button');
            removeBtn.className = 'remove-image-btn';
            removeBtn.textContent = '×';
            removeBtn.onclick = () => onRemove(index, file);

            item.appendChild(img);
            item.appendChild(removeBtn);
            container.appendChild(item);
        });
    },

    renderSingle(container, file, onRemove) {
        container.innerHTML = '';

        const img = document.createElement('img');
        img.className = 'preview-image';

        if (file instanceof File) {
            const reader = new FileReader();
            reader.onload = e => img.src = e.target.result;
            reader.readAsDataURL(file);
        } else {
            img.src = file.src;
        }

        container.appendChild(img);

        if (onRemove) {
            const removeBtn = document.createElement('button');
            removeBtn.className = 'avatar-remove-btn';
            removeBtn.textContent = '×';
            removeBtn.onclick = onRemove;
            container.appendChild(removeBtn);
        }
    },

    update(container, file)
    {
        this.renderSingle(container, file);
    }
};
