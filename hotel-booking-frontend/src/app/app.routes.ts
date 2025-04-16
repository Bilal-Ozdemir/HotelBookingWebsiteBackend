// src/app/app.routes.ts
import { Routes } from '@angular/router';

// Import all your page components:
import { HomePageComponent } from './pages/home-page/home-page.component';
import { RoomViewPageComponent } from './pages/room-view-page/room-view-page.component';
import { BookingFormComponent } from './pages/booking-form/booking-form.component';
import { ContactPageComponent } from './pages/contact-page/contact-page.component';
import { UserLoginComponent } from './pages/user-login/user-login.component';
import { UserRegistrationComponent } from './pages/user-registration/user-registration.component';
import { UserProfileLoginComponent } from './pages/user-profile-login/user-profile-login.component';
import { AdminLoginComponent } from './pages/admin-login/admin-login.component';
import { AdminDashBoardComponent } from './pages/admin-dash-board/admin-dash-board.component';

export const routes: Routes = [
  { path: '', component: HomePageComponent, title: 'Home' },
  { path: 'rooms', component: RoomViewPageComponent, title: 'Our Rooms' },
  { path: 'booking', component: BookingFormComponent, title: 'Book Your Stay' },
  { path: 'contact', component: ContactPageComponent, title: 'Contact Us' },
  { path: 'login', component: UserLoginComponent, title: 'Login' },
  { path: 'register', component: UserRegistrationComponent, title: 'Register' },
  { path: 'profile', component: UserProfileLoginComponent, title: 'My Profile' },
  { path: 'admin/login', component: AdminLoginComponent, title: 'Admin Login' },
  { path: 'admin/dashboard', component: AdminDashBoardComponent, title: 'Admin Dashboard' },
  // Catch-all: redirect any unknown paths back to home
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
