import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../services/admin.service';
import { QuizAdminDTO, Category } from '../../../models/user.model';

@Component({
  selector: 'app-quiz-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './quiz-management.html',
  styleUrls: ['./quiz-management.css']
})
export class QuizManagementComponent implements OnInit {
  quizzes: QuizAdminDTO[] = [];
  categories: Category[] = [];
  isLoading: boolean = true;
  showInactive: boolean = false;
  selectedDifficulty: string = '';

  constructor(private adminService: AdminService, private router: Router) {}

  ngOnInit(): void {
    this.loadQuizzes();
    this.loadCategories();
  }

  loadQuizzes(): void {
    this.isLoading = true;
    this.adminService.getAllQuizzes(this.showInactive).subscribe({
      next: (quizzes) => {
        this.quizzes = quizzes;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Greška pri učitavanju kvizova:', error);
        this.isLoading = false;
        alert('❌ Greška pri učitavanju kvizova');
      }
    });
  }

  loadCategories(): void {
    this.adminService.getAllCategories().subscribe({
      next: (categories) => (this.categories = categories),
      error: (error) => {
        console.error('Greška pri učitavanju kategorija:', error);
        alert('❌ Greška pri učitavanju kategorija');
      }
    });
  }

  toggleQuizStatus(quizId: number): void {
    this.adminService.toggleQuizStatus(quizId).subscribe({
      next: () => {
        const quiz = this.quizzes.find(q => q.id === quizId);
        if (quiz) quiz.isActive = !quiz.isActive;
      },
      error: (error) => {
        console.error('Greška pri promeni statusa kviza:', error);
        alert('❌ Greška pri promeni statusa kviza');
      }
    });
  }

  deleteQuiz(quizId: number): void {
  console.log('Pozvan delete za quizId:', quizId);

  this.adminService.deleteQuiz(quizId).subscribe({
    next: () => {
      console.log('Uspešno obrisan kviz sa id:', quizId);
      this.quizzes = this.quizzes.filter(q => q.id !== quizId);
      alert('✅ Kviz uspešno obrisan!');
    },
    error: (error) => {
      console.error('Greška pri brisanju kviza:', error);
      alert('❌ Greška pri brisanju kviza: ' + (error.error?.message || error.message));
    }
  });
}


  getDifficultyBadgeClass(difficulty: string): string {
    switch (difficulty.toLowerCase()) {
      case 'lako': return 'badge-easy';
      case 'srednje': return 'badge-medium';
      case 'teško': return 'badge-hard';
      default: return 'badge-default';
    }
  }

  formatTime(minutes: number): string {
    return `${minutes} min`;
  }

  get filteredQuizzes(): QuizAdminDTO[] {
    return this.quizzes
      .filter(quiz => !this.selectedDifficulty || quiz.difficulty.toLowerCase() === this.selectedDifficulty.toLowerCase());
  }

  onFiltersChange(): void {
    this.loadQuizzes();
  }

}
