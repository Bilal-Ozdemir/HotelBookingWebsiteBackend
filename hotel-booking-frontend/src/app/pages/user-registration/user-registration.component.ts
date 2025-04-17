import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-registration',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.css']
})
export class UserRegistrationComponent {
  onSubmit(form: NgForm) {
    console.log('Registration form submitted:', form.value);
  }
}
