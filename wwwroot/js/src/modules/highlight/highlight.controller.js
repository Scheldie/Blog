export function initHighlightController() {
    if (typeof hljs === 'undefined') {
        console.warn('highlight.js не найден');
        return;
    }
    
    const highlightAll = () => {
        document.querySelectorAll('pre code').forEach(block => {
            hljs.highlightElement(block);
        });
    };
    
    highlightAll();
    
    const observer = new MutationObserver(mutations => {
        for (const mutation of mutations) {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(node => {
                    if (!(node instanceof HTMLElement)) return;

                    
                    if (node.matches?.('pre code')) {
                        hljs.highlightElement(node);
                    }

                    node.querySelectorAll?.('pre code').forEach(block => {
                        hljs.highlightElement(block);
                    });
                });
            }
        }
    });
    
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
}
