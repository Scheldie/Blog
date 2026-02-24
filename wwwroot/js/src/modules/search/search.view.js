export const SearchView = {
    renderResults(container, users) {
        container.innerHTML = '';

        if (!users.length) {
            container.innerHTML = `<div class="search-result-item">Ничего не найдено</div>`;
            return;
        }

        users.forEach(user => {
            const el = document.createElement('div');
            el.className = 'search-result-item';
            el.innerHTML = `
                <img src="${user.avatarPath || '/img/profile.png'}" alt="${user.userName}">
                <div class="user-info">
                    <div class="user-name">${user.userName}</div>
                    <div class="user-details">
                        ${user.commonFriends > 0 ? `${user.commonFriends} общих друзей` : ''}
                        ${user.city ? `, ${user.city}` : ''}
                    </div>
                </div>
            `;

            el.onclick = () => {
                window.location.href = `/Profile/User?name=${user.userName}`;
            };

            container.appendChild(el);
        });
    },

    show(container) {
        container.classList.add('show');
    },

    hide(container) {
        container.classList.remove('show');
    }
};
