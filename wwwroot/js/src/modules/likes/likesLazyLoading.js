export function initLikesForNewPosts(root = document) {
    root.querySelectorAll('.like-section').forEach(section => {
        if (section.dataset.bound) return;
        section.dataset.bound = "1";

        section.addEventListener('click', async () => {
            const postId = section.dataset.postId;
            const icon = section.querySelector('.like-icon');
            const count = section.querySelector('.like-count');

            const result = await PostsService.toggleLike(postId);

            icon.classList.toggle('active', result.isLiked);
            count.textContent = result.likesCount;
        });
    });
}
