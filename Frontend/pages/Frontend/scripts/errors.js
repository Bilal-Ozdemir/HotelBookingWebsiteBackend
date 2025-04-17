// Frontend/pages/Frontend/scripts/errors.js
export function showError(message) {
    let c = document.getElementById('errorMessages');
    if (!c) {
      // dynamically inject container once
      c = document.createElement('div');
      c.id = 'errorMessages';
      c.className = 'error-messages';
      document.body.prepend(c);
    }
    c.innerHTML = `<p>${message}</p>`;
  }
  
  export function clearErrors() {
    const c = document.getElementById('errorMessages');
    if (c) c.innerHTML = '';
  }
  