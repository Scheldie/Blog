export const CommentsView = {
    renderList(container, comments) {
        container.innerHTML = '';
        comments.forEach(c => container.appendChild(this.renderComment(c)));
    },

    renderComment(comment) {
        const el = document.createElement('div');
        el.className = 'comment';
        el.dataset.id = comment.id;
        el.dataset.userId = comment.user.id;
        el.dataset.parentId = comment.parentId || '';
        el.dataset.replyToId = comment.replyToId || '';

        const replyTo = comment.replyToUser
            ? `<span class="reply-to">@${comment.replyToUser}</span>`
            : '';

        el.innerHTML = `
            <div class="comment-header">
                <a href="/Profile/Users/${comment.user.id}">
                    <img 
                        src="/img/placeholder-avatar.png"
                        data-src="${comment.user.avatarPath || '/img/default-avatar.png'}"
                        class="comment-avatar lazy">
                </a>
                <a href="/Profile/Users/${comment.user.id}" class="comment-username">${comment.user.userName}</a>
                ${replyTo}
                <span class="date">${comment.createdAt}</span>

                ${comment.isCurrentUser ? `
                    <div class="comment-actions-menu">
                        <button class="edit-comment-btn"><img src="/img/edit-icon.png" width="16"></button>
                        <button class="delete-comment-btn"><img src="/img/delete-icon.png" width="16"></button>
                    </div>
                ` : ''}
            </div>

            <div class="comment-text">${comment.text}</div>

            <div class="comment-actions">
                <button class="like-btn ${comment.isLiked ? 'active' : ''}">
                    <span class="like-count">${comment.likesCount}</span>
                </button>
                <button class="reply-btn">Ответить</button>
            </div>

            ${!comment.parentId ? ` 
            <div class="replies-container"> 
                <div class="toggle-replies"> 
                    <img src="/img/Chevron down.png" class="toggle-icon"> 
                    <span class="replies-count">${comment.replies?.length || 0} ответов</span> 
                </div> 
                <div class="replies-list" style="display:none;"></div> 
            </div>` : ''}

            <div class="reply-form" style="display:none;">
                <textarea class="reply-textarea" placeholder="Ваш ответ..."></textarea>
                <div class="reply-form-actions">
                    <button class="submit-reply">Отправить</button>
                    <button class="cancel-reply">Отмена</button>
                </div>
            </div>

            <div class="edit-form" style="display:none;">
                <textarea class="edit-textarea">${comment.text}</textarea>
                <div class="edit-form-actions">
                    <button class="save-edit-btn">Сохранить</button>
                    <button class="cancel-edit-btn">Отмена</button>
                </div>
            </div>
        `;

        return el;
    }
};
