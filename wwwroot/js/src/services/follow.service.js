export const FollowService = {

    async loadFollowers(userName, page) {
        const res = await fetch(`/Follow/Followers?userName=${userName}&page=${page}`);
        return await res.text();
    },

    async loadFollowing(userName, page) {
        const res = await fetch(`/Follow/Following?userName=${userName}&page=${page}`);
        return await res.text();
    },

    async follow(userName) {
        const res = await fetch(`/Follow/Follow?userName=${userName}`, { method: "POST" });
        if (!res.ok) return false;
        const data = await res.json();
        return data.success === true;
    },

    async unfollow(userName) {
        const res = await fetch(`/Follow/Unfollow?userName=${userName}`, { method: "POST" });
        if (!res.ok) return false;
        const data = await res.json();
        return data.success === true;
    }
};
