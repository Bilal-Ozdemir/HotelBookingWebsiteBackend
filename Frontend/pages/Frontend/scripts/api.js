const API_BASE_URL = 'http://localhost:5000/api';  

export async function fetchRooms() {
  const res = await fetch(`${API_BASE_URL}/hotelroom`);
  if (!res.ok) throw new Error('Failed to fetch rooms');
  return await res.json();
}

export async function createBooking(bookingData, token) {
  const res = await fetch(`${API_BASE_URL}/bookings/create`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(bookingData),
  });

  const responseData = await res.json();
  if (!res.ok) {
    throw new Error(responseData.message || 'Booking failed');
  }

  return responseData;
}
