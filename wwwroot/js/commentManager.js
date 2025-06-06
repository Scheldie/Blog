// commentManager.js
export function initCommentManager() {
    // ������������� ���� ������������ �� ��������
    function initAllComments() {
        const posts = document.querySelectorAll('.publication');
        posts.forEach(post => {
            setupComments(post);
        });
    }

    // ��������� ������������ ��� ����������� �����
    function setupComments(postElement) {
        const toggleComments = postElement.querySelector('.toggle-comments');
        const commentsSection = postElement.querySelector('.comments-section');
        const addCommentBtn = postElement.querySelector('.add-comment');
        const commentArea = postElement.querySelector('.comment-area');
        const commentsList = postElement.querySelector('.comments-list');

        // �������������� ��������� ���������
        commentsSection.style.display = 'none';
        toggleComments.style.transition = 'transform 0.3s ease';

        // ���������� ����� �� ������ ������������
        toggleComments.addEventListener('click', function () {
            if (commentsSection.style.display === 'none') {
                commentsSection.style.display = 'block';
                this.style.transform = 'rotate(180deg)';
            } else {
                commentsSection.style.display = 'none';
                this.style.transform = 'rotate(0deg)';
            }
        });

        // ���������� ���������� �����������
        addCommentBtn.addEventListener('click', function () {
            const commentText = commentArea.value.trim();
            if (commentText) {
                addComment(commentsList, commentText);
                commentArea.value = '';

                // ��������� ������� ������������
                updateCommentCount(postElement, 1);
            }
        });

        // ����� ��������� ���������� ��� Enter � textarea
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

    // ��������������� ������� ��� ���������� �����������
    function addComment(commentsList, text) {
        const commentElement = document.createElement('div');
        commentElement.className = 'comment';
        commentElement.textContent = text;
        commentsList.appendChild(commentElement);
    }

    // ���������� �������� ������������
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