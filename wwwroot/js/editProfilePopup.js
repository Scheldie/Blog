document.addEventListener('DOMContentLoaded', function() {

    const editProfileBtn = document.getElementById('editProfileBtn');

    const editProfilePopup = document.getElementById('editProfilePopup');

    const closeBtn = document.querySelector('.close-btn');

    const cancelBtn = document.querySelector('.cancel-btn');

    const editProfileForm = document.getElementById('editProfileForm');

    const avatarInput = document.getElementById('avatar');

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

    editProfileForm.addEventListener('submit', function(e) {

        e.preventDefault();

        

        // Обновляем данные профиля

        const firstName = document.getElementById('firstName').value;


        profileName.textContent = `${firstName}`;

        profileStatus.textContent = document.getElementById('status').value;

        profileAbout.textContent = document.getElementById('about').value;

        

        // Обновляем аватар, если выбран новый

        if (avatarInput.files && avatarInput.files[0]) {

            const reader = new FileReader();

            reader.onload = function(e) {

                profileAvatar.src = e.target.result;

            };

            reader.readAsDataURL(avatarInput.files[0]);

        }

        

        // Здесь можно добавить AJAX-запрос для сохранения на сервере

        

        // Закрываем попап

        closePopup();

        

        // Уведомление об успешном сохранении

        alert('Изменения профиля сохранены!');

    });

});