// userProfile.js
const API_BASE_URL = 'http://localhost:5252/api';

document.addEventListener('DOMContentLoaded', async () => {
  const token = localStorage.getItem('token');
  if (!token) {
    // not logged in → send back to login
    window.location.href = 'userLogin.html';
    return;
  }

  // Decode JWT to get user info
  const payload = JSON.parse(atob(token.split('.')[1]));
  const userName  = payload.unique_name || payload.email;
  const userEmail = payload.email;

  document.getElementById('user-name').textContent  = userName;
  document.getElementById('user-email').textContent = userEmail;

  // Fetch the user's bookings
  let bookings;
  try {
    const res = await fetch(`${API_BASE_URL}/bookings`, {
      headers: { 'Authorization': 'Bearer ' + token }
    });
    if (!res.ok) throw new Error('Failed to load your bookings');
    bookings = await res.json();
  } catch (err) {
    console.error(err);
    document.getElementById('bookings-body').innerHTML =
      '<tr><td colspan="6">Error loading bookings</td></tr>';
    return;
  }

  // Populate bookings table
  const tbody = document.getElementById('bookings-body');
  tbody.innerHTML = ''; // clear the "Loading..." row
  bookings.forEach(b => {
    const paid = (b.payments && b.payments.length > 0) ? 'Yes' : 'No';
    tbody.insertAdjacentHTML('beforeend', `
      <tr>
        <td>${b.id}</td>
        <td>${b.hotelRoom.roomTypes.name}</td>
        <td>${b.hotelRoom.roomNumber}</td>
        <td>${new Date(b.checkIn).toLocaleDateString()}</td>
        <td>${new Date(b.checkOut).toLocaleDateString()}</td>
        <td>${paid}</td>
      </tr>
    `);
  });

  // Logout handler
  document.getElementById('logout-link').addEventListener('click', () => {
    localStorage.removeItem('token');
    window.location.href = 'userLogin.html';
  });
});
