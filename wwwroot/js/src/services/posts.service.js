import { httpPost } from '../core/http.js';

export const PostsService = {
    create(formData) {
        return httpPost('/Post/CreatePost', formData);
    },
    edit(formData) {
        return httpPost('/Post/EditPost', formData);
    },
    delete(postId) {
        return httpPost(`/Post/DeletePost?postId=${postId}`);
    }
};
