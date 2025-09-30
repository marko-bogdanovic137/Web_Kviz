import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { QuizService } from '../../../services/quiz.service';

@Component({
  selector: 'app-quiz-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quiz-detail.html',
  styleUrl: './quiz-detail.css'
})
export class QuizDetailComponent implements OnInit {
  quiz: any = null;
  isLoading: boolean = true;
  quizId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private quizService: QuizService
  ) {}

  ngOnInit(): void {
    this.quizId = +this.route.snapshot.paramMap.get('id')!;
    this.loadQuiz();
  }

  loadQuiz(): void {
    this.quizService.getQuizById(this.quizId).subscribe({
      next: (quiz) => {
        this.quiz = quiz;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading quiz:', error);
        this.isLoading = false;
      }
    });
  }

  startQuiz(): void {
    this.router.navigate(['/quiz-solving', this.quizId]);
  }

  goBack(): void {
    this.router.navigate(['/quizzes']);
  }
}