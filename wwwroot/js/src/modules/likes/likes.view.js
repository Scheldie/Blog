export const LikesView = {
    updatePostLike(element, isLiked, count) {
        const icon = element.querySelector('.like-icon');
        const counter = element.querySelector('.like-count');

        icon.src = isLiked ? '/img/Heart Active.png' : '/img/Heart.png';
        counter.textContent = count;

        icon.classList.toggle('active', isLiked);
    }
    ,

    updateCommentLike(button, isLiked, count) {
        button.classList.toggle('active', isLiked);
        button.querySelector('.like-count').textContent = count;
    }
};
