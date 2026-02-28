export async function httpGet(url) {
    const res = await fetch(url, {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    });

    if (!res.ok) throw new Error(await res.text());
    return res.text(); // важно: фид возвращает HTML, не JSON
}

export async function httpPost(url, data) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const options = {
        method: 'POST',
        headers: { 'RequestVerificationToken': token }
    };

    if (data instanceof FormData) {
        options.body = data;
    } else {
        options.headers['Content-Type'] = 'application/json';
        options.body = JSON.stringify(data);
    }

    const res = await fetch(url, options);
    if (!res.ok) throw new Error(await res.text());
    return res.json();
}

