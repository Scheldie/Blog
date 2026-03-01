import { CommentsService } from '../../services/comments.service.js';
import { initLazyImages } from "../../ui/lazyLoader.js";
import { initLazyCommentsForPost, initLazyRepliesForComment } from "./commentsLazyLoading.js";

export function initCommentsController(root = document) {
    
    let globalForm = document.getElementById('global-reply-form');
    
    if (!globalForm) {
        globalForm = document.createElement('div');
        globalForm.id = 'global-reply-form';
        globalForm.className = 'reply-form';
        globalForm.style.display = 'none';
        globalForm.innerHTML = `
            <textarea class="reply-textarea" placeholder="Ваш ответ..."></textarea>
            <div class="reply-form-actions">
                <button class="submit-reply">Отправить</button>
                <button class="cancel-reply">Отмена</button>
            </div>
        `;
        document.body.appendChild(globalForm);
    }

    const globalTextarea = globalForm.querySelector('.reply-textarea');
    const globalSubmit = globalForm.querySelector('.submit-reply');
    const globalCancel = globalForm.querySelector('.cancel-reply');
    
    root.querySelectorAll('.add-comment').forEach(addBtn => {
        if (addBtn.dataset.bound) return;
        addBtn.dataset.bound = "1";

        addBtn.onclick = async () => {
            const post = addBtn.closest('.publication');
            const postId = post.dataset.postId;

            const textarea = post.querySelector('.comment-area');
            const text = textarea.value.trim();
            if (!text) return;

            const html = await CommentsService.add(postId, text);
            const list = post.querySelector('.comments-list');

            list.insertAdjacentHTML('afterbegin', html);

            const newComment = list.querySelector('.comment');

            textarea.value = '';

            initLazyImages();
            if (newComment) initCommentsController(newComment);
        };
    });
    
    const comments =
        root.classList && root.classList.contains('comment')
            ? [root]
            : root.querySelectorAll('.comment');

    comments.forEach(comment => {
        if (comment.dataset.bound) return;
        comment.dataset.bound = "1";

        const postId = comment.closest('.publication').dataset.postId;
        const id = comment.dataset.id;
        const parentId = comment.dataset.parentId;
        
        const replyBtn = comment.querySelector('.reply-btn');
        if (replyBtn) {
            replyBtn.onclick = () => {
                comment.after(globalForm);
                globalForm.style.display = 'block';
                globalForm.dataset.parentId = id;
                globalForm.dataset.postId = postId;
                globalTextarea.focus();
            };
        }
        
        const del = comment.querySelector('.delete-comment-btn');
        if (del) {
            del.onclick = async () => {
                if (!confirm('Удалить комментарий?')) return;
                await CommentsService.delete(id);
                comment.remove();
            };
        }
        
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

                    if (!list.dataset.loaded && !list.querySelector('.comment')) {
                        const html = await CommentsService.loadReplies(id, 1);
                        const sentinel = comment.querySelector('.replies-sentinel');

                        sentinel.insertAdjacentHTML('beforebegin', html);

                        list.dataset.page = 2;
                        list.dataset.loaded = "1";

                        initLazyImages();
                        initCommentsController(list);
                        initLazyRepliesForComment(comment);
                    }

                    list.style.display = 'block';
                    icon.src = '/img/Chevron up.png';
                    initCommentDescriptionExpand(list);
                };
            }
        }

    });
    
    if (!globalForm.dataset.bound) {
        globalForm.dataset.bound = "1";

        globalSubmit.onclick = async () => {
            let text = globalTextarea.value.trim();
            if (!text) return;

            const parentId = globalForm.dataset.parentId;
            const postId = globalForm.dataset.postId;

            const parentComment = document.querySelector(`.comment[data-id="${parentId}"]`);

          
            if (parentComment.dataset.parentId) {
                const username = parentComment.querySelector('.comment-username')?.textContent.trim();
                if (username) {
                    text = `@${username}, ${text}`;
                }
            }

            const html = await CommentsService.add(postId, text, parentId);
            let rootComment = parentComment;

            if (parentComment.dataset.parentId) {
                rootComment = document.querySelector(
                    `.comment[data-id="${parentComment.dataset.parentId}"]`
                );
            }

            const list = rootComment.querySelector('.replies-list');

            list.insertAdjacentHTML('afterbegin', html);

            const newReply = list.querySelector('.comment');

            globalForm.style.display = 'none';
            globalTextarea.value = '';

            initLazyImages();
            if (newReply) {
                initCommentsController(newReply);
                initCommentDescriptionExpand(newReply)
            }

        };

        globalCancel.onclick = () => {
            globalForm.style.display = 'none';
            globalTextarea.value = '';
        };
    }
    initCommentDescriptionExpand(root);
}

const COMMENT_COLLAPSED_HEIGHT = 180;

function initCommentDescriptionExpand(root = document) {
    const comments = root.classList?.contains('comment')
        ? [root]
        : root.querySelectorAll('.comment');

    comments.forEach(comment => {
        const wrapper = comment.querySelector('.comment-text-wrapper');
        const text = comment.querySelector('.comment-text');
        const moreBtn = comment.querySelector('.comment-more-btn');

        if (!wrapper || !text || !moreBtn) return;

        // если скрыт — не трогаем
        if (getComputedStyle(wrapper).display === 'none') return;

        // если уже был обработан — пропускаем
        if (moreBtn.dataset.bound) return;

        // сбрасываем высоту
        wrapper.style.maxHeight = 'none';

        const fullHeight = wrapper.scrollHeight;

        if (fullHeight <= COMMENT_COLLAPSED_HEIGHT + 10) {
            moreBtn.style.display = 'none';
            wrapper.classList.remove('fade');
            return;
        }

        // обрезаем wrapper
        wrapper.style.maxHeight = COMMENT_COLLAPSED_HEIGHT + 'px';
        wrapper.classList.add('fade');
        moreBtn.style.display = 'inline';
        moreBtn.dataset.bound = "1";

        let expanded = false;

        moreBtn.onclick = () => {
            expanded = !expanded;

            if (expanded) {
                wrapper.style.maxHeight = fullHeight + 'px';
                wrapper.classList.remove('fade');
                moreBtn.textContent = 'скрыть';
            } else {
                wrapper.style.maxHeight = COMMENT_COLLAPSED_HEIGHT + 'px';
                wrapper.classList.add('fade');
                moreBtn.textContent = 'ещё';
            }
        };
    });
}






