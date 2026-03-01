import { FeedService } from "../../services/feed.service.js";
import { FeedView } from "./feed.view.js";

import { initLikesForNewPosts } from "../likes/likesLazyLoading.js";
import { initCommentsForNewPosts } from "../comments/commentsLazyLoading.js";
import { initCommentsController } from "../comments/comments.controller.js";
import { initGalleryForNewPosts } from "../gallery/galleryLazyLoading.js";

import { initLazyImages } from "../../ui/lazyLoader.js";

let page = 1;
let loading = false;
let hasMore = true;

export async function initFeedController() {
    const container = document.getElementById("feed-posts-container");
    if (!container) return;

    await loadNextPage();
    initDescriptionExpand(container);

    initInfiniteScroll();
}

async function loadNextPage() {
    if (loading || !hasMore) return;

    loading = true;

    let html;
    try {
        html = await FeedService.loadFeed(page);
    } catch (e) {
        console.error("Feed load error:", e);
        loading = false;
        return;
    }

    if (!html || html.trim() === "") {
        hasMore = false;
        loading = false;
        return;
    }

    FeedView.append(html);
    
    initLazyImages();
    initLikesForNewPosts();
    initGalleryForNewPosts();
    initCommentsForNewPosts();
    initCommentsController();
    initDescriptionExpand();
    

    page++;
    loading = false;
}

function initInfiniteScroll() {
    const sentinel = document.getElementById("feed-sentinel");
    if (!sentinel) return;

    const observer = new IntersectionObserver(async entries => {
        if (entries[0].isIntersecting) {
            await loadNextPage();
        }
    });

    observer.observe(sentinel);
}


const COLLAPSED_HEIGHT = 100;

function initDescriptionExpand(root = document) {
    root.querySelectorAll('.publication').forEach(pub => {

        const wrapper = pub.querySelector('.publication-description-wrapper');
        const btn = pub.querySelector('.show-more-btn');

        if (!wrapper || !btn) return;
        if (btn.dataset.bound) return;
        btn.dataset.bound = "1";
        
        if (wrapper.scrollHeight <= COLLAPSED_HEIGHT + 10) {
            btn.style.display = 'none';
            wrapper.classList.remove('fade');
            wrapper.style.maxHeight = 'none';
            return;
        }
        
        wrapper.style.maxHeight = COLLAPSED_HEIGHT + 'px';

        btn.onclick = () => {
            const expanded = wrapper.classList.contains('expanded');

            if (expanded) {
                wrapper.classList.remove('expanded');
                wrapper.classList.add('fade');
                wrapper.style.maxHeight = COLLAPSED_HEIGHT + 'px';
                btn.textContent = 'Показать полностью';
            } else {
                wrapper.classList.add('expanded');
                wrapper.classList.remove('fade');
                wrapper.style.maxHeight = wrapper.scrollHeight + 'px';
                btn.textContent = 'Скрыть';
            }
        };
    });
}


