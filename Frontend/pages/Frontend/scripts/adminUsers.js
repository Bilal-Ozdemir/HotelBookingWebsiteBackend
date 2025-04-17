// Frontend/pages/Frontend/scripts/adminUsers.js
import { clearErrors, showError } from './errors.js';

// Replace with whatever port your API is actually running on
const API_BASE_URL = 'http://localhost:5252/api/admin/users';
const token        = localStorage.getItem('token');

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();
  if (!token) {
    window.location.href = 'adminLogin.html';
    return;
  }

  const tableBody = document.getElementById('users-body');
  if (!tableBody) {
    console.error('adminUsers.js: missing #users-body');
    return;
  }
  tableBody.innerHTML = '';

  // 1) Load users
  let users = [];
  try {
    const res = await fetch(API_BASE_URL, {
      headers: { 'Authorization': 'Bearer ' + token }
    });
    if (!res.ok) throw new Error(`Failed to load users (${res.status})`);
    users = await res.json();
  } catch (err) {
    showError(err.message);
    console.error(err);
    return;
  }

  // 2) Render rows
  if (!users.length) {
    tableBody.innerHTML = `<tr><td colspan="4"><em>No users.</em></td></tr>`;
  } else {
    tableBody.innerHTML = users.map(u => `
      <tr data-id="${u.id}">
        <td>${u.id}</td>
        <td>${u.username}</td>
        <td>${u.email}</td>
        <td>
          <button class="edit-user btn">Edit</button>
          <button class="delete-user btn">Delete</button>
        </td>
      </tr>
    `).join('');
  }

  // 3) Delegate click actions
  tableBody.addEventListener('click', async e => {
    const row = e.target.closest('tr');
    const id  = row?.dataset.id;
    if (!id) return;

    // a) Edit
    if (e.target.classList.contains('edit-user')) {
      const newName  = prompt('New username:', row.children[1].textContent);
      const newEmail = prompt('New email:',    row.children[2].textContent);
      if (!newName || !newEmail) return;

      clearErrors();
      try {
        const res = await fetch(`${API_BASE_URL}/${id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
          },
          body: JSON.stringify({ username: newName, email: newEmail })
        });
        if (!res.ok) {
          const err = await res.json().catch(() => ({}));
          throw new Error(err.error || `Update failed (${res.status})`);
        }
        // update row
        row.children[1].textContent = newName;
        row.children[2].textContent = newEmail;
      } catch (err) {
        showError(err.message);
        console.error(err);
      }
    }

    // b) Delete
    if (e.target.classList.contains('delete-user')) {
      if (!confirm('Delete user ID ' + id + ' ?')) return;
      clearErrors();
      try {
        const res = await fetch(`${API_BASE_URL}/${id}`, {
          method: 'DELETE',
          headers: { 'Authorization': 'Bearer ' + token }
        });
        if (!res.ok) {
          const err = await res.json().catch(() => ({}));
          throw new Error(err.error || `Delete failed (${res.status})`);
        }
        row.remove();
      } catch (err) {
        showError(err.message);
        console.error(err);
      }
    }
  });
});
