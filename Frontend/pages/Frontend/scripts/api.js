const API_BASE_URL = 'http://localhost:5252/api';

export async function fetchRooms() {
  const res = await fetch(`${API_BASE_URL}/hotelroom`);
  if (!res.ok) throw new Error('Failed to fetch rooms');
  return await res.json();
}

export async function createBooking(bookingData, token) {
  const response = await fetch(`${API_BASE_URL}/bookings`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    },
    body: JSON.stringify(bookingData)
  });

  const contentType = response.headers.get('content-type') || '';
  const isJson = contentType.includes('application/json');
  const result = isJson ? await response.json().catch(() => null) : await response.text();

  if (!response.ok) {
    console.error("Booking failed", response.status, result);
    throw new Error(result?.message || result || 'Booking failed');
  }

  return result;
}
