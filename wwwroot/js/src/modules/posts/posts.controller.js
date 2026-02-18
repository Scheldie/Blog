import { PostsService } from '../../services/posts.service.js';
import { Modal } from '../../ui/modal.js';
import { ImagePreview } from '../../ui/imagePreview.js';
import { PostsView } from './posts.view.js';
import { initLazyImages } from '../../ui/lazyLoader.js';
import { initLikesForNewPosts } from '../likes/likesLazyLoading.js';
import { initGalleryForNewPosts } from '../gallery/galleryLazyLoading.js';
import { initCommentsController } from '../comments/comments.controller.js';
import { initCommentsForNewPosts } from '../comments/commentsLazyLoading.js';

export function initPostsController() {

    const container = document.getElementById('posts-container');
    if (!container) return; 
    const profileUserId = container.dataset.userId;

    const addForm = document.getElementById('add-publication-form');
    const addImageInput = document.getElementById('image');
    const addPreview = document.getElementById('images-preview');

    const editCloseBtn = document.querySelector('.close-edit-popup');
    const editCancelBtn = document.querySelector('.cancel-edit');
    const editOverlay = document.getElementById('edit-popup-overlay');


    initLazyImages();
    initCommentsForNewPosts(container);
    initCommentsController(container);
    initLikesForNewPosts(container);
    initGalleryForNewPosts(container);


    if (editCloseBtn) editCloseBtn.onclick = () => Modal.close('edit-popup-container');
    if (editCancelBtn) editCancelBtn.onclick = () => Modal.close('edit-popup-container');
    if (editOverlay) editOverlay.onclick = () => Modal.close('edit-popup-container');

    

    let addImages = [];

    if (addForm && addImageInput && addPreview) {
        
        addImageInput.onchange = e => {
            addImages = Array.from(e.target.files);
            ImagePreview.render(addPreview, addImages, i => {
                addImages.splice(i, 1);
                ImagePreview.render(addPreview, addImages, () => {});
            });
        };
        addForm.onsubmit = async e => {
            e.preventDefault();
    
            const fd = new FormData();
            fd.append('Title', document.getElementById('add-publication-title').value);
            fd.append('Description', document.getElementById('add-publication-description').value);
            addImages.forEach(f => fd.append('ImageFiles', f));
            
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (token) fd.append('__RequestVerificationToken', token);
    
            await PostsService.create(fd);
    
            Modal.close('modal-add-post');
            addForm.reset();
            addImages = [];
            addPreview.innerHTML = '';
        };
        container.onclick = e => {
            const editBtn = e.target.closest('.edit-icon');
            if (!editBtn) return;
            const post = editBtn.closest('.publication');
            openEditPopup(post);
        };
        const addBtn = document.getElementById('add-publication-button');
        if (addBtn) addBtn.onclick = () => Modal.open('modal-add-post');
        Modal.close('modal-add-post');
    }
    
    
    const sentinel = document.getElementById('posts-sentinel');
    let page = 1;
    let loading = false;

    if (sentinel) {
        const observer = new IntersectionObserver(async entries => {
            if (!entries[0].isIntersecting || loading) return;

            loading = true;

            const res = await fetch(`/Post/LoadPosts?userId=${profileUserId}&page=${page}`);
            const html = await res.text();

            if (html.trim().length > 0) {
                container.insertAdjacentHTML('beforeend', html);
                container.appendChild(sentinel);
                initLazyImages();
                initCommentsForNewPosts(container);
                initCommentsController(container);
                initLikesForNewPosts(container);
                initGalleryForNewPosts(container);
            }

            loading = false;
            page++;
        });

        observer.observe(sentinel);
    }


}

function openEditPopup(postElement) {
    const data = PostsView.getPostData(postElement);

    Modal.open('modal-edit-post');

    const titleInput = document.getElementById('edit-title');
    const descInput = document.getElementById('edit-description');
    const idInput = document.getElementById('edit-post-id');
    const preview = document.getElementById('edit-images-preview');
    const imageInput = document.getElementById('edit-image-input');

    titleInput.value = data.title;
    descInput.value = data.description;
    idInput.value = data.id;

    let oldImages = [...data.images];
    let newImages = [];

    ImagePreview.render(preview, oldImages, (i, file) => {
        oldImages.splice(i, 1);
        ImagePreview.render(preview, [...oldImages, ...newImages], () => {});
    });

    imageInput.onchange = e => {
        const files = Array.from(e.target.files);
        newImages.push(...files);

        ImagePreview.render(preview, [...oldImages, ...newImages], (i, file) => {
            if (file instanceof File) {
                newImages.splice(newImages.indexOf(file), 1);
            }
            ImagePreview.render(preview, [...oldImages, ...newImages], () => {});
        });

        imageInput.value = '';
    };

    document.getElementById('edit-post-form').onsubmit = async e => {
        e.preventDefault();

        const fd = new FormData();
        fd.append('Id', data.id);
        fd.append('Title', titleInput.value);
        fd.append('Description', descInput.value);

        oldImages.forEach(img => {
            const path = new URL(img.src).pathname;
            fd.append('ExistingImagePaths', path);
        });

        newImages.forEach(f => fd.append('NewImageFiles', f));

        await PostsService.edit(fd);

        PostsView.updatePostUI(postElement, {
            title: titleInput.value,
            description: descInput.value
        });

        Modal.close('modal-edit-post');
    };

    document.getElementById('delete-post-btn').onclick = async () => {
        if (!confirm('Удалить пост?')) return;

        await PostsService.delete(data.id);
        postElement.remove();
        Modal.close('modal-edit-post');
    };
}
