// Frontend/pages/Frontend/scripts/nav.js
document.addEventListener('DOMContentLoaded', () => {
  const authSpan      = document.getElementById('authLinks');
  const adminLinkSpan = document.getElementById('adminLink');
  const token         = localStorage.getItem('token');

  // Build auth links
  if (token) {
    // Logged in
    authSpan.innerHTML = `<a href="#" id="logoutLink">Logout</a>`;
    document
      .getElementById('logoutLink')
      .addEventListener('click', e => {
        e.preventDefault();
        localStorage.removeItem('token');
        window.location.href = 'homePage.html';
      });
  } else {
    // Not logged in
    authSpan.innerHTML = `
      <a href="userLogin.html">Login</a>
      <a href="userRegistration.html">Register</a>
    `;
  }

  // Show Admin Dashboard link only if JWT contains role=Admin
  if (token) {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      if (payload.role === 'Admin') {
        adminLinkSpan.innerHTML = `<a href="adminDashBoard.html">Dashboard</a>`;
      }
    } catch {
      adminLinkSpan.innerHTML = '';
    }
  }
});
