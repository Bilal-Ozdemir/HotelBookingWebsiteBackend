// Frontend/pages/Frontend/scripts/nav.js
document.addEventListener('DOMContentLoaded', () => {
  const authSpan         = document.getElementById('authLinks');
  const adminSpan        = document.getElementById('adminLink');
  const profileSpan      = document.getElementById('userProfileLink');
  if (!authSpan || !adminSpan || !profileSpan) {
    console.warn('nav.js: missing #authLinks, #adminLink or #userProfileLink');
    return;
  }

  const token = localStorage.getItem('token');
  if (token) {
    // Show Logout
    authSpan.innerHTML = `<a href="#" id="logoutLink">Logout</a>`;
    document
      .getElementById('logoutLink')
      .addEventListener('click', e => {
        e.preventDefault();
        localStorage.removeItem('token');
        window.location.href = 'homePage.html';
      });

    // Show My Bookings
    profileSpan.innerHTML = `<a href="userProfileView.html">My Bookings</a>`;

    // Parse JWT payload safely
    let payload = null;
    try {
      payload = JSON.parse(atob(token.split('.')[1]));
    } catch (err) {
      console.error('nav.js: invalid token payload', err);
    }

    // Show Admin links if role=Admin
    if (payload && payload.role === 'Admin') {
      adminSpan.innerHTML = `
        <a href="adminDashBoard.html">Dashboard</a>
        <a href="adminUsers.html">Users</a>
      `;
    } else {
      adminSpan.innerHTML = '';
    }
  } else {
    // Guest: show Login/Register
    authSpan.innerHTML = `
      <a href="userLogin.html">Login</a>
      <a href="userRegistration.html">Register</a>
    `;
    profileSpan.innerHTML = '';
    adminSpan.innerHTML   = '';
  }
});
