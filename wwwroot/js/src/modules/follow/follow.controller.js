import { FollowService } from "../../services/follow.service.js";

export function initFollowController(root = document) {

    root.querySelectorAll('.follow-btn').forEach(btn => {
        if (btn.dataset.bound) return;
        btn.dataset.bound = "1";

        btn.onclick = async () => {
            const userName = btn.dataset.userName;
            const isFollowing = btn.classList.contains('following');

            const ok = isFollowing
                ? await FollowService.unfollow(userName)
                : await FollowService.follow(userName);

            if (!ok) return;

            btn.classList.toggle('following');
            btn.textContent = isFollowing ? 'Подписаться' : 'Отписаться';
        };
    });
}
import { followersObserver, followingObserver } from "./follow.lazy.js";
import { initLazyFollowers, initLazyFollowing } from "./follow.lazy.js";

export function initFollowTabs(root = document) {
    const tabs = root.querySelectorAll('.follow-tab');
    const followersPane = root.querySelector('.followers-container');
    const followingPane = root.querySelector('.following-container');

    if (!tabs.length || !followersPane || !followingPane) return;

    tabs.forEach(tab => {
        if (tab.dataset.bound) return;
        tab.dataset.bound = "1";

        tab.onclick = () => {
            const target = tab.dataset.target;

            tabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');

            followersPane.classList.remove('active');
            followingPane.classList.remove('active');

            followersObserver?.disconnect();
            followingObserver?.disconnect();

            if (target === "followers") {
                followersPane.classList.add('active');
                initLazyFollowers(root);
            } else {
                followingPane.classList.add('active');
                initLazyFollowing(root);
            }
        };
    });
}



