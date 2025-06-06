// commentManager.js
export function initCommentManager() {
    // Инициализация всех комментариев на странице
    function initAllComments() {
        const posts = document.querySelectorAll('.publication');
        posts.forEach(post => {
            setupComments(post);
        });
    }

    // Настройка комментариев для конкретного поста
    function setupComments(postElement) {
        const toggleComments = postElement.querySelector('.toggle-comments');
        const commentsSection = postElement.querySelector('.comments-section');
        const addCommentBtn = postElement.querySelector('.add-comment');
        const commentArea = postElement.querySelector('.comment-area');
        const commentsList = postElement.querySelector('.comments-list');

        // Инициализируем начальное состояние
        commentsSection.style.display = 'none';
        toggleComments.style.transition = 'transform 0.3s ease';

        // Обработчик клика по иконке комментариев
        toggleComments.addEventListener('click', function () {
            if (commentsSection.style.display === 'none') {
                commentsSection.style.display = 'block';
                this.style.transform = 'rotate(180deg)';
            } else {
                commentsSection.style.display = 'none';
                this.style.transform = 'rotate(0deg)';
            }
        });

        // Обработчик добавления комментария
        addCommentBtn.addEventListener('click', function () {
            const commentText = commentArea.value.trim();
            if (commentText) {
                addComment(commentsList, commentText);
                commentArea.value = '';

                // Обновляем счетчик комментариев
                updateCommentCount(postElement, 1);
            }
        });

        // Также добавляем обработчик для Enter в textarea
        commentArea.addEventListener('keypress', function (e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                const commentText = commentArea.value.trim();
                if (commentText) {
                    addComment(commentsList, commentText);
                    commentArea.value = '';
                    updateCommentCount(postElement, 1);
                }
            }
        });
    }

    // Вспомогательная функция для добавления комментария
    function addComment(commentsList, text) {
        const commentElement = document.createElement('div');
        commentElement.className = 'comment';
        commentElement.textContent = text;
        commentsList.appendChild(commentElement);
    }

    // Обновление счетчика комментариев
    function updateCommentCount(postElement, increment) {
        const commentCount = postElement.querySelector('.comment-count');
        if (commentCount) {
            const currentCount = parseInt(commentCount.textContent) || 0;
            commentCount.textContent = currentCount + increment;
        }
    }

    return {
        initAllComments
    };
}