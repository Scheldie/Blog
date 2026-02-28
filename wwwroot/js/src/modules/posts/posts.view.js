export const PostsView = {
    getPostData(postElement) {
        return {
            id: postElement.dataset.postId,
            title: postElement.querySelector('.publication-title')?.textContent || '',
            descriptionRaw: postElement.dataset.descriptionRaw || '',
            images: Array.from(postElement.querySelectorAll('.post-image')).map(img => ({
                src: img.src
            }))
        };
    },


    updatePostUI(postElement, newData) {
        postElement.querySelector('.publication-title').textContent = newData.title;
        postElement.dataset.descriptionRaw = newData.description; 

        postElement.querySelector('.publication-description').innerHTML = newData.descriptionHtml;

    }
};
