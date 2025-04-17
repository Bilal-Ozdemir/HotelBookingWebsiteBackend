import { fetchRooms, createBooking } from './api.js';
import { clearErrors, showError }   from './errors.js';

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();
  const form = document.getElementById('bookingForm');
  const roomTypeSelect = document.getElementById('roomType');

  
  try {
    const rooms  = await fetchRooms();
    const unique = {};
    rooms.forEach(r => {
      if (r.roomTypes && !unique[r.roomTypes.id]) {
        unique[r.roomTypes.id] = r.roomTypes.name;
      }
    });
    roomTypeSelect.innerHTML = Object.entries(unique)
      .map(([id, name]) => `<option value="${id}">${name}</option>`)
      .join('');
  } catch (err) {
    return showError(err.message);
  }

  
  form.addEventListener('submit', async e => {
    e.preventDefault();
    clearErrors();

    const checkIn    = document.getElementById('checkIn').value;
    const checkOut   = document.getElementById('checkOut').value;
    const roomTypeId = +roomTypeSelect.value;
    const token      = localStorage.getItem('token');

   
    if (!token) {
      return showError('Please login first.');
    }
    if (!checkIn || !checkOut || new Date(checkIn) >= new Date(checkOut)) {
      return showError('Select valid dates.');
    }

    try {
      const { bookingId } = await createBooking(
        { roomTypeId, checkIn, checkOut },
        token
      );
      window.location.href = `payment.html?bookingId=${bookingId}`;
    } catch (err) {
      showError(err.message);
      console.error(err);
    }
  });
});
