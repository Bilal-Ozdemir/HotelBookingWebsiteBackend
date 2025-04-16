import { fetchRooms, createBooking } from './api.js';

document.addEventListener('DOMContentLoaded', async () => {
  const form = document.getElementById('bookingForm');
  const roomTypeSelect = document.getElementById('roomType');

  try {
    const rooms = await fetchRooms();
    const uniqueTypes = {};

    rooms.forEach(room => {
      const type = room.roomTypes;
      if (type && !uniqueTypes[type.id]) {
        uniqueTypes[type.id] = type.name;
      }
    });

    roomTypeSelect.innerHTML = Object.entries(uniqueTypes)
      .map(([id, name]) => `<option value="${id}">${name}</option>`)
      .join('');
  } catch (err) {
    alert('Failed to load room types.');
    console.error(err);
  }

  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const checkIn = document.getElementById('checkIn').value;
    const checkOut = document.getElementById('checkOut').value;
    const roomTypeId = parseInt(roomTypeSelect.value);
    const token = localStorage.getItem('token');

    if (!token) {
      alert('You must be logged in.');
      return;
    }

    const bookingData = { roomTypeId, checkIn, checkOut };

    try {
      const result = await createBooking(bookingData, token);
      alert(result.message || 'Booking successful!');
      form.reset();
    } catch (err) {
      alert('Booking failed. Check console.');
      console.error(err);
    }
  });
});
