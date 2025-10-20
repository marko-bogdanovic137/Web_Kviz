import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../services/admin.service';

@Component({
  selector: 'app-results-overview',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './results-overview.html',
  styleUrl: './results-overview.css'
})
export class ResultsOverviewComponent implements OnInit {
  results: any[] = [];
  filteredResults: any[] = [];
  isLoading: boolean = true;
  filterType: string = 'all';
  searchTerm: string = '';

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadAllResults();
  }

  loadAllResults(): void {
    this.adminService.getAllResults().subscribe({
      next: (results) => {
        this.results = results;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading results:', error);
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    let filtered = this.results;

    // Filter by type
    if (this.filterType === 'today') {
      const today = new Date().toDateString();
      filtered = filtered.filter(result => 
        new Date(result.completedAt).toDateString() === today
      );
    } else if (this.filterType === 'week') {
      const weekAgo = new Date();
      weekAgo.setDate(weekAgo.getDate() - 7);
      filtered = filtered.filter(result => 
        new Date(result.completedAt) >= weekAgo
      );
    }

    // Filter by search
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(result =>
        result.user?.username?.toLowerCase().includes(term) ||
        result.quiz?.title?.toLowerCase().includes(term)
      );
    }

    this.filteredResults = filtered;
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  formatDate(date: Date | string): string {
    try {
      const dateObj = typeof date === 'string' ? new Date(date) : date;
      return dateObj.toLocaleDateString('sr-RS', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return 'Nepoznat datum';
    }
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    
    if (minutes > 0) {
      return `${minutes}m ${remainingSeconds}s`;
    }
    return `${remainingSeconds}s`;
  }

  getScoreColor(percentage: number): string {
    if (percentage >= 80) return 'score-high';
    if (percentage >= 60) return 'score-medium';
    return 'score-low';
  }
}