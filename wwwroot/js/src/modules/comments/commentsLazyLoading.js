import {initCommentsController} from "./comments.controller.js";
import {initLazyImages} from "../../ui/lazyLoader.js";
import {CommentsService} from "../../services/comments.service.js";

export function initCommentsForNewPosts(root = document) {
    root.querySelectorAll('.toggle-comments').forEach(toggle => {
        if (toggle.dataset.bound) return;
        toggle.dataset.bound = "1";

        toggle.onclick = async () => {
            const post = toggle.closest('.publication');
            const section = post.querySelector('.comments-section');
            const list = section.querySelector('.comments-list');

            if (section.style.display === 'block') {
                section.style.display = 'none';
                return;
            }

            section.style.display = 'block';

            if (!list.dataset.loaded) {
                const postId = post.dataset.postId;
                const html = await CommentsService.load(postId, 1);

                list.innerHTML = html;
                list.dataset.page = 2;
                list.dataset.loaded = "1";

                initLazyImages();
                initCommentsController(list);
                initLazyCommentsForPost(post);
            }
        };
    });
}

export function initLazyCommentsForPost(postElement) {
    const list = postElement.querySelector('.comments-list');
    const sentinel = postElement.querySelector('.comments-sentinel');
    if (!list || !sentinel) return;

    if (list.dataset.lazyBound) return;
    list.dataset.lazyBound = "1";

    let loading = false;

    const observer = new IntersectionObserver(async entries => {
        if (!entries[0].isIntersecting || loading) return;

        loading = true;

        const page = Number(list.dataset.page || "1");
        const postId = postElement.dataset.postId;

        const html = await CommentsService.load(postId, page);

        if (html.trim().length > 0) {
            list.insertAdjacentHTML('beforeend', html);
            list.dataset.page = page + 1;

            initLazyImages();
            initCommentsController(list);
        } else {
            observer.unobserve(sentinel);
        }

        loading = false;
    });

    observer.observe(sentinel);
}

export function initLazyRepliesForComment(commentElement) {
    const list = commentElement.querySelector('.replies-list');
    const sentinel = commentElement.querySelector('.replies-sentinel');
    if (!list || !sentinel) return;

    if (list.dataset.lazyBound) return;
    list.dataset.lazyBound = "1";

    let loading = false;

    const observer = new IntersectionObserver(async entries => {
        if (!entries[0].isIntersecting || loading) return;

        loading = true;

        const page = Number(list.dataset.page || "1");
        const commentId = commentElement.dataset.id;

        const html = await CommentsService.loadReplies(commentId, page);

        if (html.trim().length > 0) {
            sentinel.insertAdjacentHTML('beforebegin', html);
            list.dataset.page = page + 1;

            initLazyImages();
            initCommentsController(list);
        } else {
            observer.unobserve(sentinel);
        }

        loading = false;
    });

    observer.observe(sentinel);
}
