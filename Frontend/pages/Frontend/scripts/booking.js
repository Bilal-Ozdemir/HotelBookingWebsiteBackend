import { fetchRooms, createBooking } from './api.js';

document.addEventListener('DOMContentLoaded', async () => {
  const form = document.querySelector('form');
  const roomTypeSelect = document.getElementById('roomType');

 
  try {
    const rooms = await fetchRooms();
    roomTypeSelect.innerHTML = rooms
      .map(room => `<option value="${room.id}">${room.roomNumber} - ${room.roomTypes.name}</option>`)
      .join('');
  } catch (err) {
    alert('Failed to load room types');
    console.error(err);
  }

  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const checkIn = document.getElementById('checkIn').value;
    const checkOut = document.getElementById('checkOut').value;
    const roomId = parseInt(roomTypeSelect.value);

    const token = localStorage.getItem('token'); 
    if (!token) {
      alert('You must be logged in to book.');
      return;
    }

    const bookingData = {
      roomId,
      checkIn,
      checkOut
    };

    try {
      const result = await createBooking(bookingData, token);
      alert(result.message || 'Booking successful!');
      form.reset();
    } catch (err) {
      alert(err.message);
      console.error(err);
    }
  });
});
