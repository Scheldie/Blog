import { FeedService } from "../../services/feed.service.js";
import { FeedView } from "./feed.view.js";
import { initLikesForNewPosts } from "../likes/likesLazyLoading.js";
import { initCommentsForNewPosts } from "../comments/commentsLazyLoading.js";
import { initGalleryForNewPosts } from "../gallery/galleryLazyLoading.js";
import { initLazyImages } from "../../ui/lazyLoader.js";

let page = 1;
let loading = false;
let hasMore = true;

export async function initFeedController() {
    const container = document.getElementById("feed-posts-container");
    if (!container) return;

    await loadNextPage();

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

    initLikesForNewPosts();
    initCommentsForNewPosts();
    initGalleryForNewPosts();
    initLazyImages();

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
