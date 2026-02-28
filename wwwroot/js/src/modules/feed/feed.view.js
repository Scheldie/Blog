export const FeedView = {
    container: document.getElementById("feed-posts-container"),

    append(html) {
        const temp = document.createElement("div");
        temp.innerHTML = html;

        [...temp.children].forEach(el => {
            this.container.insertBefore(el, document.getElementById("feed-sentinel"));
        });
    }
};
