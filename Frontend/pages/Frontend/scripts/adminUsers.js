import { clearErrors, showError } from './errors.js';

const API_BASE_URL = 'http://localhost:5252/api/admin/users';
const token = localStorage.getItem('token');

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();

  if (!token) {
    window.location.href = 'adminLogin.html';
    return;
  }

  const tableBody = document.getElementById('users-body');
  if (!tableBody) {
    console.error('adminUsers.js: Missing #users-body');
    return;
  }

  tableBody.innerHTML = '';

  // Fetch users from the API
  let users = [];
  try {
    const res = await fetch(API_BASE_URL, {
      headers: { Authorization: 'Bearer ' + token },
    });
    if (!res.ok) throw new Error(`Failed to load users (${res.status})`);
    users = await res.json();
  } catch (err) {
    showError(err.message);
    console.error(err);
    return;
  }

  if (!users.length) {
    tableBody.innerHTML = `<tr><td colspan="4"><em>No users found.</em></td></tr>`;
  } else {
    tableBody.innerHTML = users
      .map(
        (u) => `
      <tr data-id="${u.id}">
        <td>${u.id}</td>
        <td><input type="text" value="${u.username}" disabled class="input-username"/></td>
        <td><input type="email" value="${u.email}" disabled class="input-email"/></td>
        <td>
          <button class="edit-user btn">Edit</button>
          <button class="save-user btn" hidden>Save</button>
          <button class="cancel-edit btn" hidden>Cancel</button>
          <button class="delete-user btn">Delete</button>
        </td>
      </tr>
    `
      )
      .join('');
  }

  tableBody.addEventListener('click', async (e) => {
    const row = e.target.closest('tr');
    const id = row?.dataset.id;
    if (!id) return;

    const usernameInput = row.querySelector('.input-username');
    const emailInput = row.querySelector('.input-email');
    const editBtn = row.querySelector('.edit-user');
    const saveBtn = row.querySelector('.save-user');
    const cancelBtn = row.querySelector('.cancel-edit');

    const originalUsername = usernameInput.value;
    const originalEmail = emailInput.value;

    // Edit mode
    if (e.target.classList.contains('edit-user')) {
      usernameInput.disabled = false;
      emailInput.disabled = false;
      editBtn.hidden = true;
      saveBtn.hidden = false;
      cancelBtn.hidden = false;
    }

    
    if (e.target.classList.contains('save-user')) {
      clearErrors();

      try {
        const res = await fetch(`${API_BASE_URL}/${id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: 'Bearer ' + token,
          },
          body: JSON.stringify({
            username: usernameInput.value.trim(),
            email: emailInput.value.trim(),
          }),
        });

        if (!res.ok) {
          const err = await res.json().catch(() => ({}));
          throw new Error(err.error || `Update failed (${res.status})`);
        }

        usernameInput.disabled = true;
        emailInput.disabled = true;
        editBtn.hidden = false;
        saveBtn.hidden = true;
        cancelBtn.hidden = true;
      } catch (err) {
        showError(err.message);
        console.error(err);
      }
    }

    
    if (e.target.classList.contains('cancel-edit')) {
      usernameInput.value = originalUsername;
      emailInput.value = originalEmail;
      usernameInput.disabled = true;
      emailInput.disabled = true;
      editBtn.hidden = false;
      saveBtn.hidden = true;
      cancelBtn.hidden = true;
    }

    
    if (e.target.classList.contains('delete-user')) {
      if (!confirm(`Are you sure you want to delete user ID ${id}?`)) return;
      clearErrors();

      try {
        const res = await fetch(`${API_BASE_URL}/${id}`, {
          method: 'DELETE',
          headers: { Authorization: 'Bearer ' + token },
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
