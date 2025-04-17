// Frontend/pages/Frontend/scripts/userProfile.js
import { clearErrors, showError } from './errors.js';

const API_BASE_URL = 'http://localhost:5252/api';

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();

  // 1) Ensure token and decode it
  const token = localStorage.getItem('token');
  if (!token) {
    window.location.href = 'userLogin.html?redirect=userProfileView.html';
    return;
  }

  let payload;
  try {
    payload = JSON.parse(atob(token.split('.')[1]));
  } catch {
    showError('Invalid session – please log in again.');
    localStorage.removeItem('token');
    return;
  }

  // numeric userId from the JWT
  const userId = Number(payload.nameid ?? payload.sub);
  if (!userId) {
    showError('Invalid user ID in token.');
    return;
  }

  // 2) Prefill the profile form
  document.getElementById('editName').value  = payload.unique_name || '';
  document.getElementById('editEmail').value = payload.email       || '';

  // 3) Handle profile updates (unchanged)
  document
    .getElementById('editProfileForm')
    .addEventListener('submit', async e => {
      e.preventDefault();
      clearErrors();

      const newName  = document.getElementById('editName').value.trim();
      const newEmail = document.getElementById('editEmail').value.trim();
      try {
        const res = await fetch(`${API_BASE_URL}/users/${userId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
          },
          body: JSON.stringify({ username: newName, email: newEmail })
        });
        if (!res.ok) {
          const err = await res.json().catch(() => ({}));
          throw new Error(err.error || 'Failed to update profile');
        }
        showError('Profile updated successfully.');
      } catch (err) {
        showError(err.message);
        console.error(err);
      }
    });

  // 4) Fetch and filter bookings
  let allBookings = [];
  try {
    const res = await fetch(`${API_BASE_URL}/bookings`, {
      headers: { 'Authorization': 'Bearer ' + token }
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.error || 'Failed to load bookings');

    // *** Filter for this user only ***
    allBookings = data.filter(b => Number(b.userId) === userId);
  } catch (err) {
    showError(err.message);
    console.error(err);
  }

  // 5) Render the filtered bookings
  const tbody = document.getElementById('bookings-body');
  tbody.innerHTML = '';
  if (allBookings.length === 0) {
    tbody.innerHTML = `<tr><td colspan="7"><em>No bookings found.</em></td></tr>`;
  } else {
    allBookings.forEach(b => {
      const paid = (b.payments && b.payments.length > 0) ? 'Yes' : 'No';
      tbody.insertAdjacentHTML('beforeend', `
        <tr data-id="${b.id}">
          <td>${b.id}</td>
          <td>${b.hotelRoom.roomTypes?.name || '—'}</td>
          <td>${b.hotelRoom.roomNumber}</td>
          <td>${new Date(b.checkIn).toLocaleDateString()}</td>
          <td>${new Date(b.checkOut).toLocaleDateString()}</td>
          <td>${paid}</td>
          <td><button class="cancel-booking btn">Cancel</button></td>
        </tr>
      `);
    });
  }

  // 6) Delegate Cancel clicks
  tbody.addEventListener('click', async e => {
    if (!e.target.classList.contains('cancel-booking')) return;
    const row       = e.target.closest('tr');
    const bookingId = row.dataset.id;
    if (!confirm(`Cancel booking #${bookingId}?`)) return;

    clearErrors();
    try {
      const res = await fetch(`${API_BASE_URL}/bookings/${bookingId}`, {
        method: 'DELETE',
        headers: { 'Authorization': 'Bearer ' + token }
      });
      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.error || 'Failed to cancel booking');
      }
      row.remove();
    } catch (err) {
      showError(err.message);
      console.error(err);
    }
  });
});
