export const PostsView = {
    getPostData(postElement) {
        return {
            id: postElement.dataset.postId,
            title: postElement.querySelector('.publication-title')?.textContent || '',
            description: postElement.querySelector('.publication-description')?.textContent || '',
            images: Array.from(postElement.querySelectorAll('.post-image')).map(img => ({
                src: img.src
            }))
        };
    },

    updatePostUI(postElement, newData) {
        postElement.querySelector('.publication-title').textContent = newData.title;
        postElement.querySelector('.publication-description').textContent = newData.description;
    }
};
