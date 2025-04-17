import { clearErrors, showError } from './errors.js';

const API_BASE_URL = 'http://localhost:5252/api';
const token        = localStorage.getItem('token');

function getBookingId() {
  return new URLSearchParams(window.location.search).get('bookingId');
}

async function fetchBooking(id) {
  const res = await fetch(`${API_BASE_URL}/bookings/${id}`, {
    headers: { 'Authorization': 'Bearer ' + token }
  });
  if (!res.ok) throw new Error('Failed to load booking summary.');
  return res.json();
}

function renderSummary(b) {
  const div    = document.getElementById('booking-summary');
  const nights = Math.round((new Date(b.checkOut) - new Date(b.checkIn)) / (1000*60*60*24));
  const total  = (b.hotelRoom.price * nights).toFixed(2);
  div.innerHTML = `
    <p><strong>Booking #${b.id}</strong></p>
    <p>Room: ${b.hotelRoom.roomTypes.name} (No. ${b.hotelRoom.roomNumber})</p>
    <p>Dates: ${new Date(b.checkIn).toLocaleDateString()} – ${new Date(b.checkOut).toLocaleDateString()}</p>
    <p>Nights: ${nights}</p>
    <p><strong>Total: $${total}</strong></p>
  `;
}

async function submitPayment(bookingId, amount) {
  const res = await fetch(`${API_BASE_URL}/payment`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    },
    body: JSON.stringify({ bookingId, amount })
  });

  let payload;
  try {
    payload = await res.json();
  } catch {
    const text = await res.text();
    throw new Error(text || 'Payment failed (non‑JSON response)');
  }
  if (!res.ok) {
    throw new Error(payload.error || payload.detail || 'Payment failed');
  }
  return payload;
}

document.addEventListener('DOMContentLoaded', async () => {
  clearErrors();

  const bookingId = getBookingId();
  if (!bookingId) {
    showError('No booking specified.');
    return;
  }

  let booking;
  try {
    booking = await fetchBooking(bookingId);
    renderSummary(booking);
  } catch (err) {
    showError(err.message);
    return;
  }

  document
    .getElementById('payment-form')
    .addEventListener('submit', async e => {
      e.preventDefault();
      clearErrors();

      const totalNights = Math.round(
        (new Date(booking.checkOut) - new Date(booking.checkIn)) / (1000*60*60*24)
      );
      const amount = booking.hotelRoom.price * totalNights;

      try {
        const payment = await submitPayment(parseInt(bookingId), amount);
        const conf = document.getElementById('confirmation');
        conf.style.display = 'block';
        conf.innerHTML = `
          <h2>Payment Successful!</h2>
          <p>Paid <strong>$${payment.amount.toFixed(2)}</strong> on ${new Date(payment.paymentDate).toLocaleString()}</p>
          <p>Your booking is confirmed. Thank you, ${document.getElementById('cardName').value}!</p>
        `;
        document.getElementById('payment-form').style.display = 'none';
      } catch (err) {
        showError(err.message);
        console.error(err);
      }
    });
});
