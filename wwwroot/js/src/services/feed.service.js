import { httpGet } from "../core/http.js";

export const FeedService = {
    async loadFeed(page) {
        return await httpGet(`/Feed/GetFeedPostPage?page=${page}`);
    }
};
