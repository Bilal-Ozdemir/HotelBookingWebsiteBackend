// payment.js
const API_BASE_URL = 'http://localhost:5252/api';
const token = localStorage.getItem('token');

// Utility to read bookingId from URL
function getBookingId() {
  return new URLSearchParams(window.location.search).get('bookingId');
}

// Fetch booking summary (to show user what they're paying for)
async function fetchBooking(id) {
  const res = await fetch(`${API_BASE_URL}/bookings/${id}`, {
    headers: { 'Authorization': 'Bearer ' + token }
  });
  if (!res.ok) throw new Error('Failed to load booking summary.');
  return res.json();
}

// Render booking details above the form
function renderSummary(b) {
  const div = document.getElementById('booking-summary');
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

// Submit fake payment to the API
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
      // if it’s not JSON, grab the raw text
      const text = await res.text();
      throw new Error(text || 'Payment failed (non‑JSON response)');
    }
  
    if (!res.ok) {
      // payload.error or payload.detail should exist now
      throw new Error(payload.error || payload.detail || 'Payment failed');
    }
    return payload;
  }
  

document.addEventListener('DOMContentLoaded', async () => {
  const bookingId = getBookingId();
  if (!bookingId) {
    alert('No booking specified.');
    return;
  }

  let booking;
  try {
    booking = await fetchBooking(bookingId);
    renderSummary(booking);
  } catch (err) {
    alert(err.message);
    return;
  }

  // Handle the card‑details form
  document.getElementById('payment-form').addEventListener('submit', async (e) => {
    e.preventDefault();

    // You could read card fields here; we ignore them for the fake gateway.
    const totalNights = Math.round(
      (new Date(booking.checkOut) - new Date(booking.checkIn)) / (1000*60*60*24)
    );
    const amount = booking.hotelRoom.price * totalNights;

    try {
      const payment = await submitPayment(parseInt(bookingId), amount);

      // Show confirmation message
      const conf = document.getElementById('confirmation');
      conf.style.display = 'block';
      conf.innerHTML = `
        <h2>Payment Successful!</h2>
        <p>Paid <strong>$${payment.amount.toFixed(2)}</strong> on ${new Date(payment.paymentDate).toLocaleString()}</p>
        <p>Your booking is confirmed. Thank you, ${document.getElementById('cardName').value}!</p>
      `;
      // Hide the form
      document.getElementById('payment-form').style.display = 'none';
    } catch (err) {
      alert(err.message);
      console.error(err);
    }
  });
});
