
const API_BASE_URL = 'http://localhost:5252/api';

document.addEventListener('DOMContentLoaded', async () => {
  const token = localStorage.getItem('token');
  if (!token) {
    window.location.href = 'userLogin.html';
    return;
  }
  const payload = JSON.parse(atob(token.split('.')[1]));
  document.getElementById('user-name').textContent  = payload.unique_name || payload.email;
  document.getElementById('user-email').textContent = payload.email;

  try {
    const res = await fetch(`${API_BASE_URL}/bookings`, {
      headers: { 'Authorization': 'Bearer ' + token }
    });
    if (!res.ok) throw new Error('Failed to load your bookings');
    const bookings = await res.json();
    // … populate table …
  } catch (err) {
    console.error(err);
    document.getElementById('bookings-body').innerHTML =
      '<tr><td colspan="6">Error loading bookings</td></tr>';
  }

  document.getElementById('logout-link').addEventListener('click', () => {
    localStorage.removeItem('token');
    window.location.href = 'userLogin.html';
  });
});
