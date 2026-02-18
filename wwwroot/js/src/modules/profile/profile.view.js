import { Modal } from '../../ui/modal.js';
import { ImagePreview } from '../../ui/imagePreview.js';

export const ProfileView = {
    openEditProfile(user, onRemove) {
        Modal.open('modal-edit-profile');

        document.getElementById('profile-name').value = user.userName || '';
        document.getElementById('profile-about').value = user.bio || '';

        const previewEl = document.getElementById('profile-avatar-preview');
        previewEl.innerHTML = '';

        if (user.avatarPath) {
            ImagePreview.renderSingle(previewEl, { src: user.avatarPath }, onRemove);
        }
    },

    updateAvatarPreview(file, onRemove) {
        const previewEl = document.getElementById('profile-avatar-preview');
        ImagePreview.renderSingle(previewEl, file, onRemove);
    },

    updateProfileUI({ name, bio, avatarPath }) {
        if (name) document.querySelector('.profile-name').textContent = name;
        if (bio) document.querySelector('.profile-about').textContent = bio;
        if (avatarPath) {
            document.querySelector('.profile-avatar').src = avatarPath;
        }
    }
};
