export const Modal = {
    open(id) {
        const modal = document.getElementById(id);
        if (!modal) return;

        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    },

    close(id) {
        const modal = document.getElementById(id);
        if (!modal) return;

        modal.classList.remove('active');
        document.body.style.overflow = '';
    },

    init() {
        document.addEventListener('click', e => {
            const closeBtn = e.target.closest('.modal__close');
            const overlay = e.target.classList.contains('modal__overlay');

            if (closeBtn) {
                const modal = closeBtn.closest('.modal');
                Modal.close(modal.id);
            }

            if (overlay) {
                const modal = e.target.closest('.modal');
                Modal.close(modal.id);
            }
        });
    }
};
