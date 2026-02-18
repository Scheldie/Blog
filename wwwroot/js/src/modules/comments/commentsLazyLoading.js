import { loadComments } from "./comments.controller.js";

export function initCommentsForNewPosts(root = document) {
    root.querySelectorAll('.toggle-comments').forEach(toggle => {
        if (toggle.dataset.bound) return;
        toggle.dataset.bound = "1";

        toggle.addEventListener('click', () => {
            const section = toggle.closest('.publication').querySelector('.comments-section');
            const list = section.querySelector('.comments-list');

            if (section.style.display === 'block') {
                section.style.display = 'none';
                return;
            }

            section.style.display = 'block';

            const postId = toggle.closest('.publication').dataset.postId;
            loadComments(postId, list);
        });
    });
}
