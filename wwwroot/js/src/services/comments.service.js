import { httpPost } from '../core/http.js';

export const CommentsService = {
    async load(postId) {
        const res = await fetch(`/Comment/GetComments?postId=${postId}`);
        return res.json();
    },

    async loadReplies(commentId) {
        const res = await fetch(`/Comment/GetReplies?commentId=${commentId}`);
        return res.json();
    },

    add(postId, text, parentId = null) {
        return httpPost('/Comment/AddComment', { postId, text, parentId });
    },

    edit(id, text) {
        return httpPost('/Comment/EditComment', { id, text });
    },

    delete(id) {
        return httpPost(`/Comment/DeleteComment?commentId=${id}`, {});
    }
};
