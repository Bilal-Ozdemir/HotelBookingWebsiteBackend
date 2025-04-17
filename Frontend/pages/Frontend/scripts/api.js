
export const API_BASE_URL = 'http://localhost:5252/api';

export async function fetchRooms() {
  const res = await fetch(`${API_BASE_URL}/hotelroom`);
  if (!res.ok) throw new Error('Failed to load rooms');
  return res.json();
}

export async function createBooking(data, token) {
  const res = await fetch(`${API_BASE_URL}/bookings`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    },
    body: JSON.stringify(data)
  });
  const json = await res.json();
  if (!res.ok) throw new Error(json.error || json.message || 'Booking failed');
  return json;
}
