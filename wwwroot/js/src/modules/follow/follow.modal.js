import { initLazyFollowers, initLazyFollowing } from "./follow.lazy.js";
import { followersObserver, followingObserver } from "./follow.lazy.js";

export function initFollowModal(root = document) {
    const modal = root.querySelector('#follow-modal');
    if (!modal) return;

    const overlay = modal.querySelector('.modal__overlay');
    const closeBtn = modal.querySelector('.modal__close');

    const openFollowers = root.querySelector('.open-followers');
    const openFollowing = root.querySelector('.open-following');

    function resetPane(pane, type) {
        pane.innerHTML = `
            <div class="${type}-list"></div>
            <div class="${type}-sentinel"></div>
        `;
    }

    function openModal(target) {
        modal.classList.add('open');

        const followersPane = modal.querySelector('.followers-container');
        const followingPane = modal.querySelector('.following-container');
        const tabs = modal.querySelectorAll('.follow-tab');

        resetPane(followersPane, "followers");
        resetPane(followingPane, "following");
        
        tabs.forEach(t => t.classList.remove('active'));
        modal.querySelector(`.follow-tab[data-target="${target}"]`).classList.add('active');
        
        followersPane.classList.remove('active');
        followingPane.classList.remove('active');

        followersObserver?.disconnect();
        followingObserver?.disconnect();

        if (target === "followers") {
            followersPane.classList.add('active');
            initLazyFollowers(modal);
        } else {
            followingPane.classList.add('active');
            initLazyFollowing(modal);
        }
    }


    openFollowers.onclick = () => openModal("followers");
    openFollowing.onclick = () => openModal("following");

    overlay.onclick = () => modal.classList.remove('open');
    closeBtn.onclick = () => modal.classList.remove('open');
}
