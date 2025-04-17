document.addEventListener('DOMContentLoaded', () => {
    const adminLinkSpan = document.getElementById('adminLink');
    const authSpan      = document.getElementById('authLinks');
    const token         = localStorage.getItem('token');
  
    
    if (token) {
      authSpan.innerHTML = `<a href="#" id="logoutLink">Logout</a>`;
      document.getElementById('logoutLink').addEventListener('click', e => {
        e.preventDefault();
        localStorage.removeItem('token');
        window.location.href = 'homePage.html';
      });
    } else {
      authSpan.innerHTML = `
        <a href="userLogin.html">Login</a>
        <a href="userRegistration.html">Register</a>
      `;
    }
  
   
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        if (payload.role === 'Admin') {
          adminLinkSpan.innerHTML = `<a href="adminDashBoard.html">Dashboard</a>`;
        } else {
          adminLinkSpan.innerHTML = '';
        }
      } catch {
        adminLinkSpan.innerHTML = '';
      }
    } else {
      adminLinkSpan.innerHTML = '';
    }
  });
  