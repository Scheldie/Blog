import {ProfileView} from "./profile.view.js";
import {ProfileService} from "../../services/profile.service.js";
import {Modal} from "../../ui/modal.js";
import {initLazyImages} from "../../ui/lazyLoader.js";


export function initProfileController() {
    const editBtn = document.getElementById('editProfileBtn');
    const avatarInput = document.getElementById('profile-avatar-input');
    const profileData = document.getElementById('profile-data');

    let newAvatar = null;
    let removeAvatar = false;
    initLazyImages();
    const onAvatarRemove = () => {
        newAvatar = null;
        removeAvatar = true;
        document.getElementById('profile-avatar-preview').innerHTML = '';
    };

    if (editBtn) {
        editBtn.onclick = () => {
            ProfileView.openEditProfile({
                userName: profileData.dataset.username,
                bio: profileData.dataset.bio,
                avatarPath: profileData.dataset.avatar
            }, onAvatarRemove);
        }; 
    }
    if (avatarInput) {
        avatarInput.onchange = e => {
            newAvatar = e.target.files[0];
            removeAvatar = false;
            ProfileView.updateAvatarPreview(newAvatar, onAvatarRemove);
        };
    }
    const removeBtn = document.getElementById('avatar-remove-btn');
    if (removeBtn) {
        removeBtn.addEventListener('click', e => {
            e.stopPropagation();
            e.preventDefault();

            newAvatar = null;
            removeAvatar = true;

            document.getElementById('profile-avatar-preview').innerHTML = '';
            profileData.dataset.avatar = "";
        });
    }



    const form = document.getElementById('edit-profile-form'); 
    if (form) {
        form.onsubmit = async e => {
            e.preventDefault();
    
            const fd = new FormData();
            fd.append('UserName', document.getElementById('profile-name').value);
            fd.append('Bio', document.getElementById('profile-about').value);
    
            if (newAvatar) {
                fd.append('Avatar', newAvatar);
            }
            if (removeAvatar) {
                fd.append('RemoveAvatar', 'true');
            }
    
            await ProfileService.edit(fd);
    
            ProfileView.updateProfileUI({
                name: document.getElementById('profile-name').value,
                bio: document.getElementById('profile-about').value,
                avatarPath: newAvatar ? URL.createObjectURL(newAvatar) : null
            });
            profileData.dataset.avatar = newAvatar ? URL.createObjectURL(newAvatar) : "";
            
            Modal.close('modal-edit-profile');
        }; 
    }
}
