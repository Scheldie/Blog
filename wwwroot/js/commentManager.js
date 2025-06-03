export function initCommentManager(setupPostEvents) {
    function setupComments(postElement) {
        const toggleComments = postElement.querySelector('.toggle-comments');
        const commentsSection = postElement.querySelector('.comments-section');
        const addCommentBtn = postElement.querySelector('.add-comment');
        const commentArea = postElement.querySelector('.comment-area');
        const commentsList = postElement.querySelector('.comments-list');
        
        toggleComments.addEventListener('click', function() {
            commentsSection.classList.toggle('active');
            this.style.transform = commentsSection.classList.contains('active') ? 'rotate(180deg)' : 'rotate(0deg)';
        });
        
        addCommentBtn.addEventListener('click', function() {
            const commentText = commentArea.value.trim();
            if (commentText) {
                const commentElement = document.createElement('div');
                commentElement.className = 'comment';
                commentElement.textContent = commentText;
                commentsList.appendChild(commentElement);
                commentArea.value = '';
            }
        });
    }

    return {
        setupComments
    };
}