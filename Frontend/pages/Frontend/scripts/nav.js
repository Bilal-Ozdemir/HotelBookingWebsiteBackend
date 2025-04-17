
document.addEventListener('DOMContentLoaded', () => {
  const authLinks   = document.getElementById('authLinks');
  const adminLink   = document.getElementById('adminLink');
  const profileLink = document.getElementById('userProfileLink');

  if (!authLinks || !adminLink || !profileLink) {
    console.warn('nav.js: missing required nav elements');
    return;
  }


  profileLink.hidden = true;

  const token = localStorage.getItem('token');
  if (!token) {
  
    authLinks.innerHTML = `
      <a href="userLogin.html">Login</a>
      <a href="userRegistration.html">Register</a>
    `;
    adminLink.innerHTML = '';
    return;
  }


  authLinks.innerHTML = `<a href="#" id="logoutLink">Logout</a>`;
  document
    .getElementById('logoutLink')
    .addEventListener('click', e => {
      e.preventDefault();
      localStorage.removeItem('token');
      window.location.href = 'homePage.html';
    });

 
  let role = null;
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    role = payload.role;
  } catch {
    console.warn('nav.js: invalid token payload');
  }

  if (role === 'Admin') {
   
    adminLink.innerHTML = `
      <a href="adminDashBoard.html">Dashboard</a>
      <a href="adminUsers.html">Users</a>
    `;
  } else {
    
    profileLink.hidden = false;
    profileLink.innerHTML = `<a href="userProfileView.html">My Bookings</a>`;
    adminLink.innerHTML = '';
  }
});
