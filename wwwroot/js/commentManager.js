export function initCommentManager() {
    const BASE_URL = '/Profile/Users';

    async function loadComments(postId, container) {
        try {
            const response = await fetch(`/Profile/GetComments?postId=${postId}`);
            if (!response.ok) throw new Error('Network response was not ok');

            const data = await response.json();
            if (data.error) throw new Error(data.error);

            renderComments(data, container);
        } catch (error) {
            console.error('Error:', error);
            container.innerHTML = `
            <div class="alert alert-danger">
                ${error.message}
                <button onclick="loadComments(${postId}, this.parentElement)">
                    Try Again
                </button>
            </div>
        `;
        }
    }

    function renderComments(comments, container) {
        container.innerHTML = comments.map(comment => `
        <div class="comment" data-id="${comment.id}">
            <div class="comment-header">
                <img src="${comment.user.avatarPath || '/default-avatar.png'}" 
                     alt="${comment.user.userName}">
                <span class="username">${comment.user.userName}</span>
                <span class="date">${comment.createdAt}</span>
            </div>
            <div class="comment-text">${comment.text}</div>
            <div class="comment-actions">
                <button class="like-btn" data-likes="${comment.likesCount}">
                    <img src="../img/Heart.png">
                    Like (${comment.likesCount})
                </button>
            </div>
            ${comment.replies ? `
                <div class="replies">
                    ${renderComments(comment.replies, container)}
                </div>
            ` : ''}
        </div>
    `).join('');
    }

    async function addComment(postId, text, commentsContainer, parentId = null) {
        try {
            const response = await fetch('/Profile/AddComment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value,
                    'Accept': 'application/json'
                },
                body: JSON.stringify({
                    postId: postId,
                    text: text,
                    parentId: parentId
                })
            });

            // First get the response as text
            const responseText = await response.text();
            console.log('Add comment response:', responseText);

            if (!response.ok) {
                try {
                    const errorData = JSON.parse(responseText);
                    throw new Error(errorData.error || 'Failed to add comment');
                } catch (e) {
                    throw new Error(responseText || 'Failed to add comment');
                }
            }

            // Parse the successful response
            const result = JSON.parse(responseText);

            // Refresh comments after successful addition
            await loadComments(postId, commentsContainer);

        } catch (error) {
            console.error('Error adding comment:', error);
            showError(commentsContainer, error.message || 'Failed to add comment. Please try again.');
        }
    }

    function createCommentElement(comment) {
        const commentElement = document.createElement('div');
        commentElement.className = 'comment';
        commentElement.dataset.commentId = comment.id;
        commentElement.dataset.postId = comment.postId;

        commentElement.innerHTML = `
            <div class="comment-header">
                <img src="${comment.user.avatarPath || '/img/profile.png'}" 
                     class="comment-avatar">
                <div class="comment-user-info">
                    <strong>${comment.user.userName}</strong>
                    <span class="comment-date">
                        ${new Date(comment.createdAt).toLocaleString()}
                    </span>
                </div>
            </div>
            <div class="comment-text">${comment.text}</div>
            <div class="comment-actions">
                <button class="reply-btn">Ответить</button>
            </div>
            <div class="reply-form" style="display: none;">
                <textarea class="reply-textarea" placeholder="Ваш ответ..."></textarea>
                <button class="submit-reply">Отправить</button>
                <button class="cancel-reply">Отмена</button>
            </div>
            <div class="replies-container"></div>
        `;

        return commentElement;
    }

    function setupCommentEvents(commentElement, postId) {
        const replyBtn = commentElement.querySelector('.reply-btn');
        const replyForm = commentElement.querySelector('.reply-form');
        const replyTextarea = commentElement.querySelector('.reply-textarea');
        const submitReply = commentElement.querySelector('.submit-reply');
        const cancelReply = commentElement.querySelector('.cancel-reply');
        const repliesContainer = commentElement.querySelector('.replies-container');

        replyBtn.addEventListener('click', () => {
            replyForm.style.display = 'block';
        });

        cancelReply.addEventListener('click', () => {
            replyForm.style.display = 'none';
            replyTextarea.value = '';
        });

        submitReply.addEventListener('click', async () => {
            const replyText = replyTextarea.value.trim();
            if (!replyText) return;

            await addComment(
                postId,
                replyText,
                commentElement.closest('.comments-section').querySelector('.comments-list'),
                commentElement.dataset.commentId
            );

            replyForm.style.display = 'none';
            replyTextarea.value = '';
        });
    }

    function showError(container, message) {
        container.innerHTML = `
            <div class="error-message">
                ${message}
                <button class="retry-btn">Попробовать снова</button>
            </div>
        `;

        container.querySelector('.retry-btn').addEventListener('click', () => {
            loadComments(container.closest('.publication').dataset.postId, container);
        });
    }

    function initAllComments() {
        document.querySelectorAll('.publication').forEach(postElement => {
            const postId = postElement.dataset.postId;
            const commentsSection = postElement.querySelector('.comments-section');
            const commentsList = commentsSection.querySelector('.comments-list');
            const addCommentBtn = postElement.querySelector('.add-comment');
            const commentArea = postElement.querySelector('.comment-area');
            const toggleComments = postElement.querySelector('.toggle-comments');

            let commentsLoaded = false;

            toggleComments.addEventListener('click', async function () {
                const isActive = commentsSection.style.display === 'block';

                if (!isActive && !commentsLoaded) {
                    await loadComments(postId, commentsList);
                    commentsLoaded = true;
                }

                commentsSection.style.display = isActive ? 'none' : 'block';
                this.style.transform = isActive ? 'rotate(0deg)' : 'rotate(180deg)';
            });

            addCommentBtn.addEventListener('click', async () => {
                const commentText = commentArea.value.trim();
                if (commentText) {
                    await addComment(postId, commentText, commentsList);
                    commentArea.value = '';
                }
            });

            commentArea.addEventListener('keypress', async (e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    const commentText = commentArea.value.trim();
                    if (commentText) {
                        await addComment(postId, commentText, commentsList);
                        commentArea.value = '';
                    }
                }
            });
        });
    }

    return { initAllComments };
}