export function initCommentManager() {
    const BASE_URL = '/Profile/Users';

    async function loadComments(postId, container) {
        try {
            const response = await fetch(`/Comment/GetComments?postId=${postId}`);
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

    function renderComments(comments, container, isTopLevel = true, parentComment = null) {
        if (isTopLevel) {
            container.innerHTML = '';
        }

        comments.forEach(comment => {
            const isReply = comment.isReply || (comment.ParentId !== null && comment.ParentId !== undefined);
            const commentElement = document.createElement('div');
            commentElement.className = 'comment';
            commentElement.dataset.id = comment.id;
            commentElement.dataset.userId = comment.user.id;
            commentElement.dataset.isReply = isReply;
            commentElement.dataset.parentId = comment.parentId;
            commentElement.dataset.replyToId = comment.replyToId;

            // Для ответов показываем @username автора комментария, на который отвечаем
            const replyToSection = comment.replyToId !== null ?
                `<span class="reply-to">@${comment.replyToUser}</span>` :
                '';

            let repliesSection = '';
            if (!comment.isReply) {
                repliesSection = `
                <div class="replies-container">
                    <div class="toggle-replies" data-comment-id="${comment.id}">
                        <img src="/img/Chevron down.png" class="toggle-icon">
                        <span class="replies-count">${comment.replies?.length || 0} ответов</span>
                    </div>
                    <div class="replies-list" style="display: none;"></div>
                </div>
            `;
            }

            commentElement.innerHTML = `
            <div class="comment-header">
                <a href="/Profile/Users/${comment.user.id}">
                    <img src="${comment.user.avatarPath || '/img/default-avatar.png'}" class="comment-avatar">
                </a>
                <a href="/Profile/Users/${comment.user.id}" class="comment-username">${comment.user.userName}</a>
                ${replyToSection}
                <span class="date">${comment.createdAt}</span>
                ${comment.isCurrentUser ? `
                    <div class="comment-actions-menu">
                        <button class="edit-comment-btn" data-comment-id="${comment.id}">
                            <img src="/img/edit-icon.png" alt="Редактировать" width="16">
                        </button>
                        <button class="delete-comment-btn" data-comment-id="${comment.id}">
                            <img src="/img/delete-icon.png" alt="Удалить" width="16">
                        </button>
                    </div>
                ` : ''}
            </div>
            <div class="comment-text" data-comment-id="${comment.id}">${comment.text}</div>
            <div class="comment-actions">
                <button class="like-btn ${comment.isLiked ? 'active' : ''}" data-likes="${comment.likesCount}"> 
                    <span class="like-count">${comment.likesCount}</span>
                </button>
                <button class="reply-btn">Ответить</button>
            </div>
            ${repliesSection}
            <div class="reply-form" style="display: none;">
                <textarea class="reply-textarea" placeholder="Ваш ответ..."></textarea>
                <div class="reply-form-actions">
                    <button class="submit-reply">Отправить</button>
                    <button class="cancel-reply">Отмена</button>
                </div>
            </div>
            <div class="edit-form" style="display: none;">
                <textarea class="edit-textarea" data-comment-id="${comment.id}">${comment.text}</textarea>
                <div class="edit-form-actions">
                    <button class="save-edit-btn" data-comment-id="${comment.id}">Сохранить</button>
                    <button class="cancel-edit-btn" data-comment-id="${comment.id}">Отмена</button>
                </div>
            </div>
        `;

            container.appendChild(commentElement);
            setupCommentEvents(commentElement, container.dataset.postId);

            if (!comment.isReply && comment.replies && comment.replies.length > 0) {
                const repliesList = commentElement.querySelector('.replies-list');
                renderComments(comment.replies, repliesList, false, commentElement);
            }
        });
    }

    async function deleteComment(commentId) {
        try {
            const response = await fetch(`/Comment/DeleteComment?commentId=${commentId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (!response.ok) throw new Error('Failed to delete comment');

            const result = await response.json();
            return result.success;
        } catch (error) {
            console.error('Error deleting comment:', error);
            return false;
        }
    }

    async function editComment(commentId, newText) {
        try {
            const response = await fetch(`/Comment/EditComment`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    Id: commentId,
                    text: newText
                })
            });

            if (!response.ok) throw new Error('Failed to edit comment');

            const result = await response.json();
            return result.success;
        } catch (error) {
            console.error('Error editing comment:', error);
            return false;
        }
    }

    async function addComment(postId, text, commentsContainer, parentId = null) {
        try {
            const response = await fetch('/Comment/AddComment', {
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

            const result = await response.json();

            if (!response.ok) {
                throw new Error(result.error || 'Failed to add comment');
            }

            if (parentId) {
                const parentComment = commentsContainer.querySelector(`.comment[data-id="${parentId}"]`);
                if (parentComment) {
                    const repliesList = parentComment.querySelector('.replies-list');
                    if (repliesList) {
                        // Очищаем контейнер перед загрузкой новых данных
                        repliesList.innerHTML = '';

                        // Загружаем свежий список ответов с сервера
                        const response = await fetch(`/Profile/GetReplies?commentId=${parentId}`);
                        const replies = await response.json();

                        // Рендерим ответы
                        renderComments(replies, repliesList, false, parentComment);

                        // Обновляем счетчик
                        const repliesCount = parentComment.querySelector('.replies-count');
                        if (repliesCount) {
                            repliesCount.textContent = `${replies.length} ответов`;
                        }

                        // Показываем список ответов, если он был скрыт
                        repliesList.style.display = 'block';
                        const toggleIcon = parentComment.querySelector('.toggle-icon');
                        if (toggleIcon) {
                            toggleIcon.src = '/img/Chevron up.png';
                        }
                    }
                }
            } else {
                // Для корневых комментариев перезагружаем весь список
                await loadComments(postId, commentsContainer);
            }

        } catch (error) {
            console.error('Error adding comment:', error);
            showError(commentsContainer, error.message || 'Failed to add comment. Please try again.');
        }
    }

    function setupCommentEvents(commentElement, postId) {
        const commentId = commentElement.dataset.id;
        const userId = commentElement.dataset.userId;

        // Reply functionality
        const replyBtn = commentElement.querySelector('.reply-btn');
        const replyForm = commentElement.querySelector('.reply-form');
        const replyTextarea = commentElement.querySelector('.reply-textarea');
        const submitReply = commentElement.querySelector('.submit-reply');
        const cancelReply = commentElement.querySelector('.cancel-reply');

        if (replyBtn && replyForm) {
            replyBtn.addEventListener('click', () => {
                replyForm.style.display = 'block';
                replyTextarea.focus();
            });

            cancelReply.addEventListener('click', () => {
                replyForm.style.display = 'none';
                replyTextarea.value = '';
            });

            submitReply.addEventListener('click', async () => {
                const replyText = replyTextarea.value.trim();
                if (!replyText) return;

                try {
                    await addComment(
                        postId,
                        replyText,
                        commentElement.closest('.comments-list'),
                        commentId
                    );

                    replyForm.style.display = 'none';
                    replyTextarea.value = '';
                } catch (error) {
                    console.error('Error submitting reply:', error);
                    // Показать сообщение об ошибке пользователю
                }
            });
        }

        // Delete functionality
        const deleteBtn = commentElement.querySelector('.delete-comment-btn');
        if (deleteBtn) {
            deleteBtn.addEventListener('click', async () => {
                if (confirm('Вы уверены, что хотите удалить этот комментарий?')) {
                    const success = await deleteComment(commentId);
                    if (success) {
                        commentElement.remove();
                    }
                }
            });
        }

        // Edit functionality
        const editBtn = commentElement.querySelector('.edit-comment-btn');
        const editForm = commentElement.querySelector('.edit-form');
        const editTextarea = commentElement.querySelector('.edit-textarea');
        const saveEditBtn = commentElement.querySelector('.save-edit-btn');
        const cancelEditBtn = commentElement.querySelector('.cancel-edit-btn');
        const commentTextElement = commentElement.querySelector('.comment-text');

        if (editBtn && editForm) {
            editBtn.addEventListener('click', () => {
                editForm.style.display = 'block';
                editTextarea.value = commentTextElement.textContent;
                commentTextElement.style.display = 'none';
                editTextarea.focus();
            });

            cancelEditBtn.addEventListener('click', () => {
                editForm.style.display = 'none';
                commentTextElement.style.display = 'block';
            });

            saveEditBtn.addEventListener('click', async () => {
                const newText = editTextarea.value.trim();
                if (!newText) return;

                const success = await editComment(commentId, newText);
                if (success) {
                    commentTextElement.textContent = newText;
                    editForm.style.display = 'none';
                    commentTextElement.style.display = 'block';
                }
            });
        }

        // Toggle replies functionality
        const toggleReplies = commentElement.querySelector('.toggle-replies');
        if (toggleReplies) {
            const repliesList = commentElement.querySelector('.replies-list');
            const toggleIcon = toggleReplies.querySelector('.toggle-icon');
            const repliesCount = toggleReplies.querySelector('.replies-count');

            toggleReplies.addEventListener('click', async () => {
                if (repliesList.style.display === 'none') {
                    // Load replies if not already loaded
                    if (repliesList.innerHTML === '') {
                        try {
                            const response = await fetch(`/Profile/GetReplies?commentId=${commentId}`);
                            const replies = await response.json();

                            // Update the count
                            if (repliesCount) {
                                repliesCount.textContent = `${replies.length} ответов`;
                            }

                            // Render the replies (pass parent comment for @mentions)
                            renderComments(replies, repliesList, false, commentElement);
                        } catch (error) {
                            console.error('Error loading replies:', error);
                            repliesList.innerHTML = `<div class="error">Ошибка загрузки ответов</div>`;
                        }
                    }
                    repliesList.style.display = 'block';
                    if (toggleIcon) {
                        toggleIcon.src = '/img/Chevron up.png';
                    }
                } else {
                    repliesList.style.display = 'none';
                    if (toggleIcon) {
                        toggleIcon.src = '/img/Chevron down.png';
                    }
                }
            });
        }
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

            commentsList.dataset.postId = postId;

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