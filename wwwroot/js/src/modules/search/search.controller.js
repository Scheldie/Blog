import { SearchService } from '../../services/search.service.js';
import { SearchView } from './search.view.js';

export function initSearchController() {
    const input = document.getElementById('userSearch');
    const resultsBox = document.getElementById('searchResults');
    const resultsList = document.getElementById('searchResultsList');
    const showAllBtn = document.getElementById('showAllResults');

    if (!input || !resultsBox || !resultsList) return;

    let timeout = null;
    let currentTerm = '';

    input.addEventListener('input', e => {
        currentTerm = e.target.value.trim();

        clearTimeout(timeout);

        if (currentTerm.length < 2) {
            SearchView.hide(resultsBox);
            return;
        }

        timeout = setTimeout(async () => {
            const users = await SearchService.searchUsers(currentTerm, 10);
            SearchView.renderResults(resultsList, users);
            SearchView.show(resultsBox);
        }, 300);
    });

    input.addEventListener('focus', () => {
        if (currentTerm.length >= 2) {
            SearchView.show(resultsBox);
        }
    });

    document.addEventListener('click', e => {
        if (!resultsBox.contains(e.target) && e.target !== input) {
            SearchView.hide(resultsBox);
        }
    });

    if (showAllBtn) {
        showAllBtn.onclick = () => {
            if (currentTerm.length >= 2) {
                window.location.href = `/Search/SearchResults?query=${encodeURIComponent(currentTerm)}`;
            }
        };
    }
}
