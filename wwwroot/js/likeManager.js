export function initLikeManager() {
    console.log('LikeManager initialized');

    // ������� ��� ������������� ��������� ������
    function initializeLikes() {
        document.querySelectorAll('.like-section').forEach(section => {
            const likeIcon = section.querySelector('.like-icon');
            const isActive = likeIcon.src.includes('Heart Active');
            likeIcon.src = isActive ? "/img/Heart Active.png" : "/img/Heart.png";
            likeIcon.style.cursor = 'pointer';
        });

        document.querySelectorAll('.like-btn').forEach(btn => {
            btn.style.cursor = 'pointer';
        });
    }

    // ���������� ������ ��� ������
    document.addEventListener('click', async (e) => {
        // ��������� ������ �����
        if (e.target.closest('.like-icon')) {
            const likeIcon = e.target.closest('.like-icon');
            const likeSection = likeIcon.closest('.like-section');
            const likeCount = likeSection.querySelector('.like-count');
            const postId = likeSection.dataset.postId;

            e.preventDefault();
            e.stopPropagation();

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                const response = await fetch(`/Profile/ToggleLike?postId=${postId}&isComment=false`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });

                if (!response.ok) throw new Error('������ ����');

                const result = await response.json();
                if (result.success) {
                    likeIcon.src = result.isLiked
                        ? "/img/Heart Active.png"
                        : "/img/Heart.png";
                    likeCount.textContent = result.likesCount;
                }
            } catch (error) {
                console.error('������ ����� �����:', error);
            }
        }

        // ��������� ������ ������������
        if (e.target.closest('.like-btn')) {
            const likeBtn = e.target.closest('.like-btn');
            const comment = likeBtn.closest('.comment');
            const commentId = comment.dataset.id;

            e.preventDefault();
            e.stopPropagation();

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                const response = await fetch(`/Profile/ToggleLike?postId=${commentId}&isComment=true`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });

                if (!response.ok) throw new Error('������ ����');

                const result = await response.json();
                if (result.success) {
                    likeBtn.textContent = `Like (${result.likesCount})`;
                    likeBtn.classList.toggle('active', result.isLiked);
                }
            } catch (error) {
                console.error('������ ����� �����������:', error);
            }
        }
    });

    // ������������� ��� ��������
    initializeLikes();

    return {
        // ��� ����������� ����������� ���������
        setupLikes: function (postElement) {
            const likeIcon = postElement.querySelector('.like-icon');
            if (likeIcon) {
                likeIcon.style.cursor = 'pointer';
            }

            postElement.querySelectorAll('.like-btn').forEach(btn => {
                btn.style.cursor = 'pointer';
            });
        }
    };
}