document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('userSearch');
    const searchResults = document.getElementById('searchResults');
    const searchResultsList = document.getElementById('searchResultsList');
    const showAllResults = document.getElementById('showAllResults');

    let searchTimeout;
    let currentSearchTerm = '';

    searchInput.addEventListener('input', function (e) {
        currentSearchTerm = e.target.value.trim();

        clearTimeout(searchTimeout);

        if (currentSearchTerm.length < 2) {
            searchResults.classList.remove('show');
            return;
        }

        searchTimeout = setTimeout(() => {
            searchUsers(currentSearchTerm, false);
        }, 300);
    });

    searchInput.addEventListener('focus', function () {
        if (currentSearchTerm.length >= 2) {
            searchResults.classList.add('show');
        }
    });

    document.addEventListener('click', function (e) {
        if (!searchResults.contains(e.target) && e.target !== searchInput) {
            searchResults.classList.remove('show');
        }
    });

    showAllResults.addEventListener('click', function () {
        if (currentSearchTerm.length >= 2) {
            window.location.href = `/Search/SearchResults?query=${encodeURIComponent(currentSearchTerm)}`;
        }
    });

    function searchUsers(query, showAll = false) {
        fetch(`/Search/SearchUsers?query=${encodeURIComponent(query)}&limit=${showAll ? 0 : 10}`)
            .then(response => response.json())
            .then(data => {
                if (data.length > 0) {
                    renderSearchResults(data);
                    searchResults.classList.add('show');
                } else {
                    searchResultsList.innerHTML = '<div class="search-result-item">Ничего не найдено</div>';
                    searchResults.classList.add('show');
                }
            })
            .catch(error => {
                console.error('Ошибка поиска:', error);
            });
    }

    function renderSearchResults(users) {
        searchResultsList.innerHTML = '';

        users.forEach(user => {
            const userElement = document.createElement('div');
            userElement.className = 'search-result-item';
            userElement.innerHTML = `
                <img src="${user.avatarPath || '/img/profile.png'}" alt="${user.userName}">
                <div class="user-info">
                    <div class="user-name">${user.userName}</div>
                    <div class="user-details">
                        ${user.commonFriends > 0 ? `${user.commonFriends} общих друзей` : ''}
                        ${user.city ? `, ${user.city}` : ''}
                    </div>
                </div>
            `;

            userElement.addEventListener('click', function () {
                window.location.href = `/Profile/Users/${user.id}`;
            });

            searchResultsList.appendChild(userElement);
        });
    }
});