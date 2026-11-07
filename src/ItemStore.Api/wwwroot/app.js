const apiUrl = '/api/items';

async function loadItems() {
    const response = await fetch(apiUrl);
    const items = await response.json();
    render(items);
}

function render(items) {
    const tbody = document.querySelector('#items-table tbody');
    tbody.innerHTML = '';
    for (const item of items) {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${item.name}</td>
            <td>${item.price} ₽</td>
            <td>${item.quantity}</td>
            <td><button onclick="deleteItem(${item.id})">✕</button></td>
        `;
        tbody.appendChild(row);
    }
}

async function addEvent() {
    event.preventDefault();
    const form = document.getElementById('add-form');
    const body = {
        name: document.getElementById('name').value,
        price: Number(document.getElementById('price').value),
        quantity: Number(document.getElementById('quantity').value),
        description: null
    };

    const response = await fetch(apiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });

    if (response.ok) {
        form.reset();
        await loadItems();
    } else {
        alert('Ошибка при добавлении: ' + response.status);
    }
}

async function deleteItem(id) {
    await fetch(`${apiUrl}/${id}`, { method: 'DELETE' });
    await loadItems();
}

document.getElementById('add-form').addEventListener('submit', addEvent);
loadItems();
