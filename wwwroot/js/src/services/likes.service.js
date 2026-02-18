import { httpPost } from '../core/http.js';

export const LikesService = {
    togglePost(postId) {
        return httpPost('/Like/ToggleLike', { 
            postId, 
            isComment: false 
        });
    },
    toggleComment(postId, commentId) {
        return httpPost('/Like/ToggleLike', { 
            postId, 
            commentId, 
            isComment: true 
        });
    }
};
