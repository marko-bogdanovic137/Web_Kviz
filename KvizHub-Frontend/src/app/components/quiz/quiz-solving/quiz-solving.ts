import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { QuizSolvingService } from '../../../services/quiz-solving.service';

@Component({
  selector: 'app-quiz-solving',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './quiz-solving.html',
  styleUrl: './quiz-solving.css'
})
export class QuizSolvingComponent implements OnInit, OnDestroy {
  quiz: any = null;
  currentQuestionIndex: number = 0;
  userAnswers: any[] = []; // Čuva sve odgovore tokom kviza
  selectedAnswers: number[] = [];
  fillInAnswer: string = '';
  timeLeft: number = 0;
  timer: any;
  isLoading: boolean = true;
  quizCompleted: boolean = false;
  quizResult: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private quizSolvingService: QuizSolvingService
  ) {}

  ngOnInit(): void {
    const quizId = +this.route.snapshot.paramMap.get('id')!;
    this.loadQuiz(quizId);
  }

  loadQuiz(quizId: number): void {
    this.quizSolvingService.startQuiz(quizId).subscribe({
      next: (quiz) => {
        this.quiz = quiz;
        this.timeLeft = quiz.timeLimit * 60;
        
        // Inicijalizuj userAnswers za sva pitanja
        this.userAnswers = quiz.questions.map((question: any) => ({
          questionId: question.id,
          selectedAnswerIds: [],
          answerText: ''
        }));
        
        this.startTimer();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading quiz:', error);
        this.isLoading = false;
      }
    });
  }

  startTimer(): void {
    this.timer = setInterval(() => {
      this.timeLeft--;
      
      if (this.timeLeft <= 0) {
        this.submitQuiz();
      }
    }, 1000);
  }

  get currentQuestion() {
    return this.quiz?.questions[this.currentQuestionIndex];
  }

  get progress() {
    return ((this.currentQuestionIndex + 1) / this.quiz.questions.length) * 100;
  }

  selectAnswer(answerId: number): void {
    if (this.currentQuestion.type === 'MultipleChoice') {
      this.selectedAnswers = [answerId];
    } else if (this.currentQuestion.type === 'MultipleAnswer') {
      const index = this.selectedAnswers.indexOf(answerId);
      if (index > -1) {
        this.selectedAnswers.splice(index, 1);
      } else {
        this.selectedAnswers.push(answerId);
      }
    } else if (this.currentQuestion.type === 'TrueFalse') {
      this.selectedAnswers = [answerId];
    }
  }

  isAnswerSelected(answerId: number): boolean {
    return this.selectedAnswers.includes(answerId);
  }

  nextQuestion(): void {
    this.saveCurrentAnswer();
    
    if (this.currentQuestionIndex < this.quiz.questions.length - 1) {
      this.currentQuestionIndex++;
      this.loadCurrentAnswer();
    } else {
      this.submitQuiz();
    }
  }

  previousQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.saveCurrentAnswer();
      this.currentQuestionIndex--;
      this.loadCurrentAnswer();
    }
  }

  saveCurrentAnswer(): void {
    // Sačuvaj trenutni odgovor u userAnswers
    this.userAnswers[this.currentQuestionIndex] = {
      questionId: this.currentQuestion.id,
      selectedAnswerIds: [...this.selectedAnswers],
      answerText: this.fillInAnswer
    };
  }

  loadCurrentAnswer(): void {
    // Učitaj prethodno sačuvan odgovor
    const currentAnswer = this.userAnswers[this.currentQuestionIndex];
    this.selectedAnswers = currentAnswer?.selectedAnswerIds ? [...currentAnswer.selectedAnswerIds] : [];
    this.fillInAnswer = currentAnswer?.answerText || '';
  }

  submitQuiz(): void {
    clearInterval(this.timer);
    
    // Sačuvaj poslednji odgovor
    this.saveCurrentAnswer();

    const submission = {
      quizId: this.quiz.id,
      answers: this.userAnswers,
      timeSpent: (this.quiz.timeLimit * 60) - this.timeLeft
    };

    console.log('Submitting quiz:', submission);

    this.quizSolvingService.submitQuiz(submission).subscribe({
      next: (result) => {
        console.log('Quiz result:', result);
        this.quizCompleted = true;
        this.quizResult = {
          score: result.score,
          totalQuestions: this.quiz.questions.length,
          percentage: result.percentage,
          timeSpent: submission.timeSpent
        };
      },
      error: (error) => {
        console.error('Error submitting quiz:', error);
        alert('Došlo je do greške pri slanju kviza. Pokušajte ponovo.');
      }
    });
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes}:${secs < 10 ? '0' : ''}${secs}`;
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  viewDetails(): void {
    this.router.navigate(['/quiz-history']);
  }

  ngOnDestroy(): void {
    if (this.timer) {
      clearInterval(this.timer);
    }
  }
}