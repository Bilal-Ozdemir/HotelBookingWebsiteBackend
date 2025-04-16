import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-profile-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './user-profile-login.component.html',
  styleUrls: ['./user-profile-login.component.css']
})
export class UserProfileLoginComponent {
  phoneNumber: string = '+1 (555) 123-4567';
  address: string = '123 Hotel Street, City, Country';
}
