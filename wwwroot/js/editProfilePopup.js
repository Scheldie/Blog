document.addEventListener('DOMContentLoaded', function() {

    const editProfileBtn = document.getElementById('editProfileBtn');

    const editProfilePopup = document.getElementById('editProfilePopup');

    const closeBtn = document.querySelector('.close-btn');

    const cancelBtn = document.querySelector('.cancel-btn');

    const editProfileForm = document.getElementById('editProfileForm');

    const avatarInput = document.getElementById('Avatar');

    const avatarPreview = document.getElementById('avatarPreview');

    const fileNameSpan = document.querySelector('.file-name');

    const profileAvatar = document.querySelector('.profile-avatar');

    const profileName = document.querySelector('.profile-name');

    const profileStatus = document.querySelector('.profile-status');

    const profileAbout = document.querySelector('.profile-about');



    // Открытие попапа

    editProfileBtn.addEventListener('click', function() {

        editProfilePopup.style.display = 'flex';

        document.body.style.overflow = 'hidden';

    });



    // Закрытие попапа

    function closePopup() {

        editProfilePopup.style.display = 'none';

        document.body.style.overflow = 'auto';

    }



    closeBtn.addEventListener('click', closePopup);

    cancelBtn.addEventListener('click', closePopup);



    // Закрытие при клике вне попапа

    editProfilePopup.addEventListener('click', function(e) {

        if (e.target === editProfilePopup) {

            closePopup();

        }

    });



    // Обработка выбора аватара

    avatarInput.addEventListener('change', function(e) {

        if (e.target.files && e.target.files[0]) {

            const file = e.target.files[0];

            const reader = new FileReader();

            

            reader.onload = function(event) {

                avatarPreview.src = event.target.result;

                fileNameSpan.textContent = file.name;

            };

            

            reader.readAsDataURL(file);

        }

    });


    // Отправка формы

    editProfileForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        try {
            // Создаём FormData из формы
            const formData = new FormData(editProfileForm);

            // Добавляем токен CSRF
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            formData.append('__RequestVerificationToken', token);

            // Отправляем данные на сервер
            const response = await fetch('/Profile/EditProfile', {
                method: 'POST',
                body: formData,
                headers: {
                    'Accept': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Ошибка сервера');
            }

            const result = await response.json();

            if (result.success) {
                // Обновляем UI
                profileName.textContent = document.getElementById('UserName').value;
                profileAbout.textContent = document.getElementById('Bio').value;

                if (result.avatarPath) {
                    profileAvatar.src = result.avatarPath;
                    avatarPreview.src = result.avatarPath;
                }

                // Закрываем попап
                closePopup();

                // Показываем уведомление
                alert('Изменения сохранены успешно!');
            }
        } catch (error) {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при сохранении профиля');
        }
    });

});