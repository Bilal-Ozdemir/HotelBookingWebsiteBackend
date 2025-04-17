// File: adminDashboard.js
const API_BASE_URL = 'http://localhost:5252/api';
const token = localStorage.getItem('token');

// Utility: protected fetch that includes the JWT token
async function fetchWithAuth(url, options = {}) {
  const headers = options.headers || {};
  headers["Authorization"] = "Bearer " + token;
  options.headers = headers;
  const res = await fetch(url, options);
  if (res.status === 401 || res.status === 403) {
    // Unauthorized – token missing or not admin
    localStorage.removeItem('token');
    alert("Unauthorized or session expired. Please log in as admin.");
    window.location.href = 'adminLogin.html';
    throw new Error("Unauthorized");
  }
  return res;
}

// 1. On page load, ensure admin is logged in
document.addEventListener('DOMContentLoaded', async () => {
  if (!token) {
    // No token, redirect to admin login
    window.location.href = 'adminLogin.html';
    return;
  }

  try {
    // Fetch admin stats for the dashboard cards
    const statsRes = await fetchWithAuth(`${API_BASE_URL}/admin/stats`);
    const stats = await statsRes.json();
    document.getElementById('total-bookings').innerText = stats.totalBookings;
    document.getElementById('occupancy-rate').innerText = stats.occupancyRate + "%";
    document.getElementById('rooms-available').innerText = stats.roomsAvailable;
    // Format revenue as currency with commas
    const revenueElem = document.getElementById('total-revenue');
    revenueElem.innerText = stats.totalRevenue.toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });

    // Fetch all bookings (admin endpoint) and populate bookings table
    const bookingsRes = await fetchWithAuth(`${API_BASE_URL}/admin/bookings`);
    const bookings = await bookingsRes.json();
    populateBookingsTable(bookings);

    // Fetch all rooms (public endpoint is fine for listing)
    const roomsRes = await fetch(`${API_BASE_URL}/hotelroom`);
    const rooms = await roomsRes.json();
    populateRoomsTable(rooms);
    populateRoomTypeOptions(rooms);  // fill the Room Type dropdown in the form
  } catch (err) {
    console.error("Error loading admin data:", err);
  }
});

// 2. Populate Bookings Table
function populateBookingsTable(bookings) {
  const tbody = document.getElementById('bookings-body');
  tbody.innerHTML = "";  // clear loading or previous data
  bookings.forEach(b => {
    const paidStatus = (b.payments && b.payments.length > 0) ? "Yes" : "No";
    const row = document.createElement('tr');
    row.innerHTML = `
      <td>${b.id}</td>
      <td>${b.user ? b.user.email : ""}</td>
      <td>${b.hotelRoom ? (b.hotelRoom.roomTypes ? b.hotelRoom.roomTypes.name : "") + " #" + (b.hotelRoom.roomNumber || "") : ""}</td>
      <td>${new Date(b.checkIn).toLocaleDateString()}</td>
      <td>${new Date(b.checkOut).toLocaleDateString()}</td>
      <td>${paidStatus}</td>
    `;
    tbody.appendChild(row);
  });
  if (bookings.length === 0) {
    tbody.innerHTML = "<tr><td colspan='6'><em>No bookings found.</em></td></tr>";
  }
}

// 3. Populate Rooms Table
function populateRoomsTable(rooms) {
  const tbody = document.getElementById('rooms-body');
  tbody.innerHTML = "";
  rooms.forEach(room => {
    const row = document.createElement('tr');
    row.innerHTML = `
      <td>${room.roomNumber}</td>
      <td>${room.roomTypes ? room.roomTypes.name : room.roomTypeId}</td>
      <td>$${room.price.toFixed(2)}</td>
      <td>
        <button class="edit-room btn-small" data-id="${room.id}">Edit</button>
        <button class="delete-room btn-small" data-id="${room.id}">Delete</button>
      </td>
    `;
    tbody.appendChild(row);
  });
  if (rooms.length === 0) {
    tbody.innerHTML = "<tr><td colspan='4'><em>No rooms found.</em></td></tr>";
  }
}

// Helper: populate Room Type dropdown from room list (unique types)
function populateRoomTypeOptions(rooms) {
  const typeSelect = document.getElementById('room-type');
  typeSelect.innerHTML = "";
  // Extract unique room type ID->Name mapping
  const uniqueTypes = {};
  rooms.forEach(r => {
    if (r.roomTypes) {
      uniqueTypes[r.roomTypes.id] = r.roomTypes.name;
    }
  });
  // Create options
  for (const [typeId, typeName] of Object.entries(uniqueTypes)) {
    const opt = document.createElement('option');
    opt.value = typeId;
    opt.textContent = typeName;
    typeSelect.appendChild(opt);
  }
}

// 4. Handle navigation links (Dashboard, Bookings, Rooms)
document.getElementById('nav-dashboard').addEventListener('click', () => {
  document.getElementById('dashboard-cards').style.display = 'grid';
  document.getElementById('bookings-panel').style.display = 'none';
  document.getElementById('rooms-panel').style.display = 'none';
});
document.getElementById('nav-bookings').addEventListener('click', () => {
  document.getElementById('dashboard-cards').style.display = 'none';
  document.getElementById('bookings-panel').style.display = 'block';
  document.getElementById('rooms-panel').style.display = 'none';
});
document.getElementById('nav-rooms').addEventListener('click', () => {
  document.getElementById('dashboard-cards').style.display = 'none';
  document.getElementById('bookings-panel').style.display = 'none';
  document.getElementById('rooms-panel').style.display = 'block';
});

// 5. Logout functionality
document.getElementById('logout-link').addEventListener('click', () => {
  localStorage.removeItem('token');
  window.location.href = 'adminLogin.html';
});

// 6. Add/Edit Room form handling
let editRoomId = null;  // track if we are editing an existing room

document.getElementById('room-form').addEventListener('submit', async (e) => {
  e.preventDefault();
  const roomNumber = document.getElementById('room-number').value.trim();
  const roomTypeId = document.getElementById('room-type').value;
  const price = parseFloat(document.getElementById('room-price').value);
  if (!roomNumber || !roomTypeId || isNaN(price)) {
    alert("Please fill out all room fields.");
    return;
  }
  const roomData = { roomNumber, roomTypeId: parseInt(roomTypeId), price };
  try {
    if (editRoomId == null) {
      // Create new room
      const res = await fetchWithAuth(`${API_BASE_URL}/hotelroom`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(roomData)
      });
      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.error || "Failed to add room");
      }
      alert("Room added successfully.");
    } else {
      // Update existing room
      roomData.id = editRoomId;  // include ID in body for PUT
      const res = await fetchWithAuth(`${API_BASE_URL}/hotelroom/${editRoomId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(roomData)
      });
      if (!res.ok) {
        const err = await res.text();
        throw new Error(err || "Failed to update room");
      }
      alert("Room updated successfully.");
    }
    // Refresh room list and stats after changes
    const newRoomsRes = await fetch(`${API_BASE_URL}/hotelroom`);
    const newRooms = await newRoomsRes.json();
    populateRoomsTable(newRooms);
    populateRoomTypeOptions(newRooms);
    const newStatsRes = await fetchWithAuth(`${API_BASE_URL}/admin/stats`);
    const newStats = await newStatsRes.json();
    document.getElementById('rooms-available').innerText = newStats.roomsAvailable;
    document.getElementById('occupancy-rate').innerText = newStats.occupancyRate + "%";
    document.getElementById('total-bookings').innerText = newStats.totalBookings;
    document.getElementById('total-revenue').innerText = newStats.totalRevenue.toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
  } catch(err) {
    alert(err.message);
    console.error(err);
  } finally {
    // Reset form for next use
    editRoomId = null;
    document.getElementById('room-form-title').innerText = "Add New Room";
    document.getElementById('room-form-submit').innerText = "Add Room";
    document.getElementById('room-form').reset();
  }
});

// 7. Edit/Delete room button handlers (using event delegation on rooms table)
document.getElementById('rooms-panel').addEventListener('click', async (e) => {
  if (e.target.classList.contains('edit-room')) {
    // Edit button clicked
    const roomId = e.target.getAttribute('data-id');
    const roomRows = document.querySelectorAll('#rooms-body tr');
    // find the room data from the table row (or alternatively, fetch single room from API)
    let roomNumber, roomTypeName, price;
    e.target.closest('tr').querySelectorAll('td').forEach((td, idx) => {
      if (idx === 0) roomNumber = td.textContent;
      if (idx === 1) roomTypeName = td.textContent;
      if (idx === 2) price = td.textContent.replace('$','');
    });
    // Set form for editing
    editRoomId = parseInt(roomId);
    document.getElementById('room-number').value = roomNumber;
    document.getElementById('room-price').value = parseFloat(price);
    // Set the dropdown to the correct type based on name
    const typeSelect = document.getElementById('room-type');
    for (let option of typeSelect.options) {
      if (option.text === roomTypeName) {
        option.selected = true;
        break;
      }
    }
    document.getElementById('room-form-title').innerText = "Edit Room";
    document.getElementById('room-form-submit').innerText = "Update Room";
  }
  if (e.target.classList.contains('delete-room')) {
    // Delete button clicked
    const roomId = e.target.getAttribute('data-id');
    if (!confirm("Are you sure you want to delete room #" + roomId + "?")) {
      return;
    }
    try {
      const res = await fetchWithAuth(`${API_BASE_URL}/hotelroom/${roomId}`, {
        method: "DELETE"
      });
      if (!res.ok) {
        throw new Error("Failed to delete room");
      }
      alert("Room deleted.");
      // Refresh room list and stats
      const newRoomsRes = await fetch(`${API_BASE_URL}/hotelroom`);
      const newRooms = await newRoomsRes.json();
      populateRoomsTable(newRooms);
      populateRoomTypeOptions(newRooms);
      const newStatsRes = await fetchWithAuth(`${API_BASE_URL}/admin/stats`);
      const newStats = await newStatsRes.json();
      document.getElementById('rooms-available').innerText = newStats.roomsAvailable;
      document.getElementById('occupancy-rate').innerText = newStats.occupancyRate + "%";
    } catch(err) {
      alert(err.message);
      console.error(err);
    }
  }
});
