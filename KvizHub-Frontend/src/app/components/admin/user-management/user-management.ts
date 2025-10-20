import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AdminService } from '../../../services/admin.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css'
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  isLoading: boolean = true;

  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.adminService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.isLoading = false;
      }
    });
  }

  formatDate(date: Date | string): string {
    try {
      const dateObj = typeof date === 'string' ? new Date(date) : date;
      return dateObj.toLocaleDateString('sr-RS');
    } catch {
      return 'Nepoznat datum';
    }
  }
  viewUserResults(userId: number): void {
  this.router.navigate(['/admin/results'], { queryParams: { userId: userId } });
 }
 deleteUser(userId: number): void {
      this.adminService.deleteUser(userId).subscribe({
        next: () => {
          alert('Korisnik uspešno obrisan');
          this.loadUsers(); // Osveži listu
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          alert('Greška pri brisanju korisnika: ' + error.error?.message);
        }
      });
    }
}