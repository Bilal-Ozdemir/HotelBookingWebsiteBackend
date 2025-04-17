// Frontend/pages/Frontend/scripts/userProfile.js
import { clearErrors, showError } from './errors.js';

const API_BASE_URL = 'http://localhost:5252/api';

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();

  const token = localStorage.getItem('token');
  if (!token) {
    // Safety‐net; the inline script already redirects
    window.location.href = 'userLogin.html?redirect=userProfileView.html';
    return;
  }

  // Decode JWT
  let payload;
  try {
    payload = JSON.parse(atob(token.split('.')[1]));
  } catch {
    showError('Invalid session, please log in again.');
    localStorage.removeItem('token');
    return;
  }

  // Display user info
  document.getElementById('user-name').textContent  = payload.unique_name || payload.email;
  document.getElementById('user-email').textContent = payload.email;

  // Fetch bookings
  try {
    const res = await fetch(`${API_BASE_URL}/bookings`, {
      headers: { 'Authorization': 'Bearer ' + token }
    });
    if (!res.ok) throw new Error('Failed to load your bookings');
    const bookings = await res.json();

    // Populate table
    const tbody = document.getElementById('bookings-body');
    tbody.innerHTML = '';  // clear the Loading… row
    if (bookings.length === 0) {
      tbody.innerHTML = `<tr><td colspan="6"><em>No bookings found.</em></td></tr>`;
    } else {
      bookings.forEach(b => {
        const paid = (b.payments && b.payments.length > 0) ? 'Yes' : 'No';
        tbody.insertAdjacentHTML('beforeend', `
          <tr>
            <td>${b.id}</td>
            <td>${b.hotelRoom.roomTypes?.name || '—'}</td>
            <td>${b.hotelRoom.roomNumber}</td>
            <td>${new Date(b.checkIn).toLocaleDateString()}</td>
            <td>${new Date(b.checkOut).toLocaleDateString()}</td>
            <td>${paid}</td>
          </tr>
        `);
      });
    }
  } catch (err) {
    showError(err.message);
    console.error(err);
    document.getElementById('bookings-body').innerHTML =
      '<tr><td colspan="6">Error loading bookings</td></tr>';
  }

  // Logout link (rendered by nav.js as `#logoutLink`)
  // But if you want a fallback here:
  const logout = document.getElementById('logout-link');
  if (logout) {
    logout.addEventListener('click', e => {
      e.preventDefault();
      localStorage.removeItem('token');
      window.location.href = 'userLogin.html';
    });
  }
});
