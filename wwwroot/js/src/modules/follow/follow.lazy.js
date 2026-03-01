export let followersObserver = null;
export let followingObserver = null;

import { FollowService } from "../../services/follow.service.js";
import { initFollowController } from "./follow.controller.js";
import { initLazyImages } from "../../ui/lazyLoader.js";

export function initLazyFollowers(root = document) {
    const container = root.querySelector('.followers-container');
    if (!container) return;

    const list = container.querySelector('.followers-list');
    const sentinel = container.querySelector('.followers-sentinel');
    const userName = container.dataset.username;

    let page = 1;
    let loading = false;
    let finished = false;

    async function loadPage() {
        if (loading || finished) return;
        loading = true;

        const html = await FollowService.loadFollowers(userName, page);
        if (!html.trim()) {
            finished = true;
            followersObserver?.disconnect();
            return;
        }

        sentinel.insertAdjacentHTML('beforebegin', html);

        initLazyImages();
        initFollowController(list);

        page++;
        loading = false;
    }

    followersObserver = new IntersectionObserver(entries => {
        if (entries[0].isIntersecting) loadPage();
    });

    followersObserver.observe(sentinel);
    loadPage();
}

export function initLazyFollowing(root = document) {
    const container = root.querySelector('.following-container');
    if (!container) return;

    const list = container.querySelector('.following-list');
    const sentinel = container.querySelector('.following-sentinel');
    const userName = container.dataset.username;

    let page = 1;
    let loading = false;
    let finished = false;

    async function loadPage() {
        if (loading || finished) return;
        loading = true;

        const html = await FollowService.loadFollowing(userName, page);
        if (!html.trim()) {
            finished = true;
            followingObserver?.disconnect();
            return;
        }

        sentinel.insertAdjacentHTML('beforebegin', html);

        initLazyImages();
        initFollowController(list);

        page++;
        loading = false;
    }

    followingObserver = new IntersectionObserver(entries => {
        if (entries[0].isIntersecting) loadPage();
    });

    followingObserver.observe(sentinel);
    loadPage();
}
