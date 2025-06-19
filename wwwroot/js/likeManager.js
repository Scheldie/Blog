export function initLikeManager() {
    console.log('LikeManager initialized');

    // Функция для инициализации состояния лайков
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

    // Обработчик кликов для лайков
    document.addEventListener('click', async (e) => {
        // Обработка лайков поста
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

                if (!response.ok) throw new Error('Ошибка сети');

                const result = await response.json();
                if (result.success) {
                    likeIcon.src = result.isLiked
                        ? "/img/Heart Active.png"
                        : "/img/Heart.png";
                    likeCount.textContent = result.likesCount;
                    if (result.isLiked) {
                        likeIcon.classList.add('active');
                    }
                    else {
                        likeIcon.classList.remove('active');
                    }
                }
            } catch (error) {
                console.error('Ошибка лайка поста:', error);
            }
        }

        // Обработка лайков комментариев
        if (e.target.closest('.like-btn')) {
            const likeBtn = e.target.closest('.like-btn');

            // Создаем иконку, если её нет
            if (!likeBtn.querySelector('.like-icon')) {
                const isActive = likeBtn.classList.contains('active');

            }

            const comment = likeBtn.closest('.comment');
            if (!comment) return;

            const commentId = comment.dataset.id;

            const likeCount = likeBtn.querySelector('.like-count');

            e.preventDefault();
            e.stopPropagation();

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                const response = await fetch(`/Profile/ToggleLike?postId=${commentId}&isComment=true`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) throw new Error('Ошибка сервера');

                const result = await response.json();
                if (result.success) {
                    
                    likeCount.textContent = result.likesCount;
                    likeBtn.classList.toggle('active', result.isLiked);

                    setTimeout(() => likeIcon.style.transform = '', 300);
                }
            } catch (error) {
                console.error('Ошибка:', error);
            }
        }
    });

    return {
        setupLikes: function (element) {
            // Для динамически добавленных элементов
            element.querySelectorAll('.like-btn').forEach(btn => {
                if (!btn.querySelector('.like-icon')) {
                    const icon = document.createElement('img');
                    icon.className = 'like-icon';
                    icon.src = btn.classList.contains('active')
                        ? "/img/Heart Active.png"
                        : "/img/Heart.png";
                    icon.width = 16;
                    icon.height = 16;
                    btn.prepend(icon);
                }
            });
        }
    };
}