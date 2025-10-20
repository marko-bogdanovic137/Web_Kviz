import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminService } from '../../../services/admin.service';
import { AdminStatsDTO } from '../../../models/user.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboardComponent implements OnInit {
  stats: AdminStatsDTO | null = null;
  isLoading: boolean = true;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    console.log('🎯 ADMIN DASHBOARD COMPONENT LOADED!');
    this.loadStats();
  }

  loadStats(): void {
    this.adminService.getAdminStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading admin stats:', error);
        this.isLoading = false;
      }
    });
  }

  formatNumber(num: number): string {
    return num.toLocaleString('sr-RS');
  }

  formatPercentage(num: number | undefined): string {
    if (num === undefined || num === null) return '0%';
    return `${Math.round(num)}%`;
  }
}