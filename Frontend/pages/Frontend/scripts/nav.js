// nav.js
document.addEventListener('DOMContentLoaded', () => {
  const authSpan        = document.getElementById('authLinks');
  const adminSpan       = document.getElementById('adminLink');
  const profileSpan     = document.getElementById('userProfileLink');
  const token           = localStorage.getItem('token');

  if (token) {
    // Show Logout
    authSpan.innerHTML = `<a href="#" id="logoutLink">Logout</a>`;
    document.getElementById('logoutLink')
      .addEventListener('click', e => {
        e.preventDefault();
        localStorage.removeItem('token');
        window.location.href = 'homePage.html';
      });

    // Show My Bookings
    profileSpan.innerHTML = `<a href="userProfileView.html">Profile</a>`;
  } else {
    // Show Login/Register
    authSpan.innerHTML = `
      <a href="userLogin.html">Login</a>
      <a href="userRegistration.html">Register</a>
    `;
    profileSpan.innerHTML = '';  // hide Bookings link
  }

  // Show Admin Dashboard if JWT role=Admin
  if (token) {
    try {
      const { role } = JSON.parse(atob(token.split('.')[1]));
      if (role === 'Admin') {
        adminSpan.innerHTML = `<a href="adminDashBoard.html">Dashboard</a>`;
      }
    } catch {
      adminSpan.innerHTML = '';
    }
  }
});
