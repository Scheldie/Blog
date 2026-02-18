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
