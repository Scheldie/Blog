import { LikesService } from '../../services/likes.service.js';
import { LikesView } from './likes.view.js';

export function initLikesController() {
    document.addEventListener('click', async e => {
        // Лайк поста
        const postLike = e.target.closest('.like-icon');
        if (postLike) {
            e.preventDefault();
            e.stopPropagation();

            const section = postLike.closest('.like-section');
            const postId = section.dataset.postId;

            const result = await LikesService.togglePost(postId);
            if (result.success) {
                LikesView.updatePostLike(section, result.isLiked, result.likesCount);
            }
            return;
        }

        // Лайк комментария
        const commentLike = e.target.closest('.like-btn');
        if (commentLike) {
            e.preventDefault();
            e.stopPropagation();

            const comment = commentLike.closest('.comment');
            const commentId = comment.dataset.id;

            const postSection = comment.closest('.publication').querySelector('.like-section');
            const postId = postSection.dataset.postId;

            const result = await LikesService.toggleComment(postId, commentId);
            if (result.success) {
                LikesView.updateCommentLike(commentLike, result.isLiked, result.likesCount);
            }
        }
    });
}
