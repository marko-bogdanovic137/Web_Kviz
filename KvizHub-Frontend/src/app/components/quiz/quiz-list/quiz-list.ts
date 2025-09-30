import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { QuizService } from '../../../services/quiz.service';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-quiz-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './quiz-list.html',
  styleUrl: './quiz-list.css'
})
export class QuizListComponent implements OnInit {
  quizzes: any[] = [];
  filteredQuizzes: any[] = [];
  categories: string[] = [];
  difficulties: string[] = ['Sve', 'Lako', 'Srednje', 'Teško'];
  searchTerm: string = '';
  selectedCategory: string = 'Sve';
  selectedDifficulty: string = 'Sve';
  isLoading: boolean = true;

  constructor(private quizService: QuizService) {}

  ngOnInit(): void {
  this.loadQuizzes();
  
  // Debounce pretraga - čeka 300ms nakon zadnjeg unosa
  this.searchSubject.pipe(
    debounceTime(300),
    distinctUntilChanged()
  ).subscribe(searchTerm => {
    this.searchTerm = searchTerm;
    this.filterQuizzes();
  });
}

  loadQuizzes(): void {
    this.quizService.getAllQuizzes().subscribe({
      next: (quizzes) => {
        this.quizzes = quizzes;
        this.filteredQuizzes = quizzes;
        this.extractCategories();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading quizzes:', error);
        this.isLoading = false;
      }
    });
  }

  extractCategories(): void {
  if (!this.quizzes || this.quizzes.length === 0) return;
  
  const categories = new Set(this.quizzes.map(quiz => quiz.categoryName).filter(Boolean));
  this.categories = ['Sve', ...Array.from(categories)];
}

  filterQuizzes(): void {
  if (!this.quizzes || this.quizzes.length === 0) {
    this.filteredQuizzes = [];
    return;
  }

  this.filteredQuizzes = this.quizzes.filter(quiz => {
    // Provera pretrage
    const matchesSearch = !this.searchTerm || 
      quiz.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      (quiz.description && quiz.description.toLowerCase().includes(this.searchTerm.toLowerCase()));
    
    // Provera kategorije
    const matchesCategory = this.selectedCategory === 'Sve' || 
      quiz.categoryName === this.selectedCategory;
    
    // Provera težine
    const matchesDifficulty = this.selectedDifficulty === 'Sve' || 
      quiz.difficulty === this.selectedDifficulty;
    
    return matchesSearch && matchesCategory && matchesDifficulty;
  });
}

  onSearchChange(searchTerm: string): void {
  this.searchTerm = searchTerm;
  this.filterQuizzes();
}

  onCategoryChange(): void {
    this.filterQuizzes();
  }

  onDifficultyChange(): void {
    this.filterQuizzes();
  }

  onFilterChange(): void {
  this.filterQuizzes();
  }

  private searchSubject = new Subject<string>();
}