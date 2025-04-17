import { clearErrors, showError } from './errors.js';

const API_BASE_URL = 'http://localhost:5252/api';
const token        = localStorage.getItem('token');

async function fetchWithAuth(url, options = {}) {
  const headers       = options.headers || {};
  headers["Authorization"] = "Bearer " + token;
  options.headers     = headers;
  const res           = await fetch(url, options);
  if (res.status === 401 || res.status === 403) {
    clearErrors();
    showError("Unauthorized or session expired. Please log in as admin.");
    window.location.href = 'adminLogin.html';
    throw new Error("Unauthorized");
  }
  return res;
}

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();

  if (!token) {
    window.location.href = 'adminLogin.html';
    return;
  }

  try {
    const statsRes = await fetchWithAuth(`${API_BASE_URL}/admin/stats`);
    const stats    = await statsRes.json();
    document.getElementById('total-bookings').innerText    = stats.totalBookings;
    document.getElementById('occupancy-rate').innerText    = stats.occupancyRate + "%";
    document.getElementById('rooms-available').innerText   = stats.roomsAvailable;
    document.getElementById('total-revenue').innerText     =
      stats.totalRevenue.toLocaleString('en-US', {
        style: 'currency',
        currency: 'USD',
        maximumFractionDigits: 0
      });

    const bookingsRes = await fetchWithAuth(`${API_BASE_URL}/admin/bookings`);
    populateBookingsTable(await bookingsRes.json());

    const roomsRes = await fetch(`${API_BASE_URL}/hotelroom`);
    const rooms    = await roomsRes.json();
    populateRoomsTable(rooms);
    populateRoomTypeOptions(rooms);
  } catch (err) {
    showError("Error loading admin data.");
    console.error(err);
  }
});


