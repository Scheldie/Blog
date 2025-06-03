export function initLikeManager(setupPostEvents) {
    const likedPosts = new Set();

    function setupLikes(postElement) {
        const likeIcon = postElement.querySelector('.like-icon');
        const likeCount = postElement.querySelector('.like-count');
        const postId = postElement.id;
        
        likeIcon.addEventListener('click', function() {
            if (likedPosts.has(postId)) {
                likeCount.textContent = parseInt(likeCount.textContent) - 1;
                likedPosts.delete(postId);
                this.src = '../img/Heart.png';
            } else {
                likeCount.textContent = parseInt(likeCount.textContent) + 1;
                likedPosts.add(postId);
                this.src = '../img/Heart Active.png';
            }
            
            this.style.transform = 'scale(1.2)';
            setTimeout(() => this.style.transform = 'scale(1)', 300);
        });
    }

    return {
        setupLikes
    };
}