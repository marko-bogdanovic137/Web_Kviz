import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { LoginRequest } from '../../../models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  loginData: LoginRequest = {
    usernameOrEmail: '',
    password: ''
  };
  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
  this.isLoading = true;
  this.errorMessage = '';

  this.authService.login(this.loginData).subscribe({
    next: (response) => {
      this.isLoading = false;
      // Proveri da li postoji token (uspešan login)
      if (response && response.token) {
        this.router.navigate(['/dashboard']);
      } else {
        // Ako backend vraća error message direktno
        this.errorMessage = response.message || 'Pogrešni podaci za prijavu';
      }
    },
    error: (error) => {
      this.isLoading = false;
      // Backend vraća error u error.error
      this.errorMessage = error.error?.message || error.message || 'Došlo je do greške';
      console.error('Login error:', error);
    }
  });
}
}