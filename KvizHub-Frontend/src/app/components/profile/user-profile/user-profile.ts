import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { QuizSolvingService } from '../../../services/quiz-solving.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.css'
})
export class UserProfileComponent implements OnInit {
  currentUser: User | null = null;
  quizHistory: any[] = [];
  stats: any = {};
  isLoading: boolean = true;

  constructor(
    private authService: AuthService,
    private quizSolvingService: QuizSolvingService
  ) {}

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.currentUser = this.authService.getCurrentUser();
    
    if (this.currentUser) {
      this.loadQuizHistory();
    } else {
      this.isLoading = false;
    }
  }

  loadQuizHistory(): void {
    this.quizSolvingService.getUserQuizHistory(this.currentUser!.id).subscribe({
      next: (history) => {
        this.quizHistory = history;
        this.calculateStats();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading quiz history:', error);
        this.isLoading = false;
      }
    });
  }

  calculateStats(): void {
    if (this.quizHistory.length === 0) {
      this.stats = {
        totalQuizzes: 0,
        totalScore: 0,
        averagePercentage: 0,
        totalTimeSpent: 0
      };
      return;
    }

    const totalScore = this.quizHistory.reduce((sum, quiz) => sum + quiz.score, 0);
    const totalPercentage = this.quizHistory.reduce((sum, quiz) => sum + quiz.percentage, 0);
    const totalTime = this.quizHistory.reduce((sum, quiz) => sum + quiz.timeSpent, 0);

    this.stats = {
      totalQuizzes: this.quizHistory.length,
      totalScore: totalScore,
      averagePercentage: Math.round(totalPercentage / this.quizHistory.length),
      totalTimeSpent: totalTime
    };
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (hours > 0) {
      return `${hours}h ${remainingMinutes}m`;
    }
    return `${minutes}m`;
  }

  formatDate(dateInput: Date | string | undefined): string {
    if (!dateInput) {
      return 'Nepoznat datum';
    }
    
    try {
      const date = typeof dateInput === 'string' ? new Date(dateInput) : dateInput;
      return date.toLocaleDateString('sr-RS');
    } catch (error) {
      console.error('Error formatting date:', error);
      return 'Nevažeći datum';
    }
  }

  // Specijalna metoda za createdAt jer može biti Date ili string
  getFormattedCreatedAt(): string {
    if (!this.currentUser?.createdAt) {
      return 'Nepoznat datum';
    }
    
    return this.formatDate(this.currentUser.createdAt);
  }
}