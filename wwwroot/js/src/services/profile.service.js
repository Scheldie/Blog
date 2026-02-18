import { httpPost } from '../core/http.js';

export const ProfileService = {
    edit(formData) {
        return httpPost('/Profile/EditProfile', formData);
    }
};
