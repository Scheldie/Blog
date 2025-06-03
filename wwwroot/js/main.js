import { initPostManager } from './postManager.js';
import { initCommentManager } from './commentManager.js';
import { initLikeManager } from './likeManager.js';
import { initModalManager } from './modalManager.js';
import { initImageManager } from './imageManager.js';

document.addEventListener('DOMContentLoaded', () => {
    const modalManager = initModalManager();
    const imageManager = initImageManager();
    const commentManager = initCommentManager();
    const likeManager = initLikeManager();
    
    initPostManager(modalManager, imageManager, commentManager, likeManager);
});