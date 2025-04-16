// booking.js
import { fetchRooms, createBooking } from './api.js';

document.addEventListener('DOMContentLoaded', async () => {
  const form           = document.getElementById('bookingForm');
  const roomTypeSelect = document.getElementById('roomType');

  // populate room types (unchanged)
  try {
    const rooms = await fetchRooms();
    const unique = {};
    rooms.forEach(r => {
      const t = r.roomTypes;
      if (t && !unique[t.id]) unique[t.id] = t.name;
    });
    roomTypeSelect.innerHTML = Object.entries(unique)
      .map(([id, name]) => `<option value="${id}">${name}</option>`)
      .join('');
  } catch (err) {
    alert(err.message);
    console.error(err);
  }

  form.addEventListener('submit', async e => {
    e.preventDefault();
    const checkIn    = document.getElementById('checkIn').value;
    const checkOut   = document.getElementById('checkOut').value;
    const roomTypeId = +roomTypeSelect.value;
    const token      = localStorage.getItem('token');

    if (!token) {
      alert('You must be logged in to book.');
      return;
    }
    if (!checkIn || !checkOut || new Date(checkIn) >= new Date(checkOut)) {
      alert('Please select valid check‑in and check‑out dates.');
      return;
    }

    try {
      const { bookingId } = await createBooking(
        { roomTypeId, checkIn, checkOut },
        token
      );
      // redirect to payment flow
      window.location.href = `payment.html?bookingId=${bookingId}`;
    } catch (err) {
      // err is now an Error, so err.message is your backend's "error" text
      alert(err.message);
      console.error('Booking error:', err);
    }
  });
});
