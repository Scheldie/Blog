export const SearchService = {
    async searchUsers(query, limit = 10) {
        const res = await fetch(`/Search/SearchUsers?query=${encodeURIComponent(query)}&limit=${limit}`);
        return res.json();
    }
};
