import {httpPost, httpPostHtml } from '../core/http.js';

export const CommentsService = {

    async load(postId, page = 1) {
        const res = await fetch(`/Comment/GetComments?postId=${postId}&page=${page}`);
        return await res.text();
    },

    async loadReplies(commentId, page = 1) {
        const res = await fetch(`/Comment/GetReplies?commentId=${commentId}&page=${page}`);
        return await res.text();
    },


    add(postId, text, parentId = null) {
        return httpPostHtml('/Comment/AddComment', {postId, text, parentId});
    },

    edit(id, text) {
        return httpPostHtml('/Comment/EditComment', {id, text});
    },

    delete(id) {
        return httpPost(`/Comment/DeleteComment?commentId=${id}`, {});
    }
};
