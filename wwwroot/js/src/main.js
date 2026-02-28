import { initPostsController } from './modules/posts/posts.controller.js';
import { initProfileController } from './modules/profile/profile.controller.js';
import { initCommentsController } from './modules/comments/comments.controller.js';
import { initLikesController } from './modules/likes/likes.controller.js';
import { initGalleryController } from './modules/gallery/gallery.controller.js';
import { initSearchController } from './modules/search/search.controller.js';
import {initCarousel, initPhotoView} from './modules/carousel/carousel.controller.js';
import { Modal } from './ui/modal.js';
import { initFeedController } from "./modules/feed/feed.controller.js";

document.addEventListener('DOMContentLoaded', () => {
    initPostsController();
    initCommentsController();
    initLikesController();
    initProfileController();
    initGalleryController();
    initSearchController();
    initCarousel();
    initPhotoView();
    initFeedController();
    Modal.init();
});
