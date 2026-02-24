import { CommentsService } from '../../services/comments.service.js';
import { CommentsView } from './comments.view.js';
import { initLazyImages } from "../../ui/lazyLoader.js";

export function initCommentsController(root = document) {

    // -------------------------
    // ADD COMMENT (always active)
    // -------------------------
    root.querySelectorAll('.add-comment').forEach(addBtn => {
        if (addBtn.dataset.bound) return;
        addBtn.dataset.bound = "1";

        addBtn.onclick = async () => {
            const post = addBtn.closest('.publication');
            const postId = post.dataset.postId;

            const textarea = post.querySelector('.comment-area');
            const text = textarea.value.trim();
            if (!text) return;

            const newComment = await CommentsService.add(postId, text);

            const list = post.querySelector('.comments-list');
            list.appendChild(CommentsView.renderComment(newComment.result));

            textarea.value = '';

            initLazyImages();
            initCommentsController(post);

        };
    });

    // -------------------------
    // EXISTING COMMENTS
    // -------------------------
    root.querySelectorAll('.comment').forEach(comment => {

        if (comment.dataset.bound) return;
        comment.dataset.bound = "1";

        const postId = comment.closest('.publication').dataset.postId;
        const id = comment.dataset.id;
        const parentId = comment.dataset.parentId;

        // REPLY BUTTON
        const replyBtn = comment.querySelector('.reply-btn');
        if (replyBtn) {
            replyBtn.onclick = () => {
                const form = comment.querySelector('.reply-form');
                form.style.display = 'block';
                form.querySelector('.reply-textarea').focus();
            };
        }

        // CANCEL REPLY
        const cancelReply = comment.querySelector('.cancel-reply');
        if (cancelReply) {
            cancelReply.onclick = () => {
                const form = comment.querySelector('.reply-form');
                form.style.display = 'none';
                form.querySelector('.reply-textarea').value = '';
            };
        }

        // SUBMIT REPLY
        const submitReply = comment.querySelector('.submit-reply');
        if (submitReply) {
            submitReply.onclick = async () => {
                const form = comment.querySelector('.reply-form');
                const textarea = form.querySelector('.reply-textarea');
                const text = textarea.value.trim();
                if (!text) return;

                form.style.display = 'none';
                textarea.value = '';

                const newReply = await CommentsService.add(postId, text, id);

                const list = comment.querySelector('.replies-list');
                list.appendChild(CommentsView.renderComment(newReply.result));

                initLazyImages();
                initCommentsController(list);
            };
        }

        // DELETE COMMENT
        const del = comment.querySelector('.delete-comment-btn');
        if (del) {
            del.onclick = async () => {
                if (!confirm('Удалить комментарий?')) return;
                await CommentsService.delete(id);
                comment.remove();
            };
        }

        // EDIT COMMENT
        const edit = comment.querySelector('.edit-comment-btn');
        if (edit) {
            const editForm = comment.querySelector('.edit-form');
            const textBlock = comment.querySelector('.comment-text');

            edit.onclick = () => {
                editForm.style.display = 'block';
                textBlock.style.display = 'none';
            };

            comment.querySelector('.cancel-edit-btn').onclick = () => {
                editForm.style.display = 'none';
                textBlock.style.display = 'block';
            };

            comment.querySelector('.save-edit-btn').onclick = async () => {
                const newText = comment.querySelector('.edit-textarea').value.trim();
                if (!newText) return;

                await CommentsService.edit(id, newText);

                textBlock.textContent = newText;
                editForm.style.display = 'none';
                textBlock.style.display = 'block';
            };
        }

        // TOGGLE REPLIES
        if (!parentId) {
            const toggle = comment.querySelector('.toggle-replies');
            if (toggle) {
                toggle.onclick = async () => {
                    const list = comment.querySelector('.replies-list');
                    const icon = toggle.querySelector('.toggle-icon');

                    if (list.style.display === 'block') {
                        list.style.display = 'none';
                        icon.src = '/img/Chevron down.png';
                        return;
                    }

                    const replies = await CommentsService.loadReplies(id);
                    CommentsView.renderList(list, replies);

                    initLazyImages();
                    initCommentsController(list);

                    list.style.display = 'block';
                    icon.src = '/img/Chevron up.png';
                };
            }
        }
    });
}

export async function loadComments(postId, container) {
    if (container.dataset.loaded) return;
    container.dataset.loaded = "1";

    const comments = await CommentsService.load(postId);
    CommentsView.renderList(container, comments);

    initLazyImages();
    initCommentsController(container);
}
