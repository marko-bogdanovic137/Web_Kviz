import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AdminService } from '../../../services/admin.service';
import { CreateQuizDTO, CreateQuestionDTO, CreateAnswerOptionDTO, Category, QuizDetailDTO } from '../../../models/user.model';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-quiz-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './quiz-form.html',
  styleUrl: './quiz-form.css'
})
export class QuizFormComponent implements OnInit {
  quiz: CreateQuizDTO = {
    title: '',
    description: '',
    timeLimit: 10,
    difficulty: 'Srednje',
    categoryId: 0,
    questions: []
  };

  categories: Category[] = [];
  isLoading: boolean = false;
  isEditMode: boolean = false;
  quizId: number | null = null;

  questionTypes = [
    { value: 'MultipleChoice', label: 'Više ponuđenih odgovora (1 tačan)' },
    { value: 'MultipleResponse', label: 'Više tačnih odgovora' },
    { value: 'TrueFalse', label: 'Tačno/Netačno' },
    { value: 'FillInBlank', label: 'Popuni prazninu' }
  ];

  difficulties = [
    { value: 'Lako', label: 'Lako' },
    { value: 'Srednje', label: 'Srednje' },
    { value: 'Teško', label: 'Teško' }
  ];

  constructor(
    private adminService: AdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.quizId = +params['id'];
        this.loadQuizForEdit(this.quizId);
      }
    });
  }

  loadCategories(): void {
    this.adminService.getAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        if (categories.length > 0 && !this.quiz.categoryId) {
          this.quiz.categoryId = categories[0].id;
        }
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        alert('Greška pri učitavanju kategorija: ' + error.error?.message);
      }
    });
  }

  loadQuizForEdit(quizId: number): void {
    this.isLoading = true;
    this.adminService.getQuizForEdit(quizId).subscribe({
      next: (quizDetail: QuizDetailDTO) => {
        this.quiz = {
          title: quizDetail.title,
          description: quizDetail.description,
          timeLimit: quizDetail.timeLimit,
          difficulty: quizDetail.difficulty,
          categoryId: this.categories.find(c => c.name === quizDetail.categoryName)?.id || 0,
          questions: quizDetail.questions.map(q => ({
            text: q.text,
            type: q.type,
            points: q.points,
            order: q.order,
            answerOptions: q.answerOptions.map(a => ({
              text: a.text,
              isCorrect: a.isCorrect,
              order: a.order
            }))
          }))
        };
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading quiz for edit:', error);
        alert('Greška pri učitavanju kviza: ' + error.error?.message);
        this.isLoading = false;
        this.router.navigate(['/admin/quizzes']);
      }
    });
  }

  addQuestion(): void {
    const newQuestion: CreateQuestionDTO = {
      text: '',
      type: 'MultipleChoice',
      points: 1,
      order: this.quiz.questions.length + 1,
      answerOptions: this.getDefaultAnswerOptions('MultipleChoice')
    };
    this.quiz.questions.push(newQuestion);
  }

  removeQuestion(index: number): void {
      this.quiz.questions.splice(index, 1);
      this.updateQuestionOrders();
  }

  addAnswerOption(questionIndex: number): void {
    const question = this.quiz.questions[questionIndex];
    const newOption: CreateAnswerOptionDTO = {
      text: '',
      isCorrect: false,
      order: question.answerOptions.length + 1
    };
    question.answerOptions.push(newOption);
  }

  removeAnswerOption(questionIndex: number, optionIndex: number): void {
    const question = this.quiz.questions[questionIndex];
    if (question.answerOptions.length > 1) {
      question.answerOptions.splice(optionIndex, 1);
      this.updateAnswerOptionOrders(questionIndex);
    } else {
      alert('Pitanje mora imati barem jedan odgovor!');
    }
  }

 onQuestionTypeChange(questionIndex: number, newType: string): void {
  const question = this.quiz.questions[questionIndex];
  question.type = newType;

  if (newType === 'FillInBlank') {
    question.answerOptions = [{ text: '', isCorrect: true, order: 1 }];
  } else {
    question.answerOptions = this.getDefaultAnswerOptions(newType);
  }
}


  getDefaultAnswerOptions(type: string): CreateAnswerOptionDTO[] {
    switch (type) {
      case 'MultipleChoice':
        return [
          { text: '', isCorrect: true, order: 1 },
          { text: '', isCorrect: false, order: 2 },
          { text: '', isCorrect: false, order: 3 },
          { text: '', isCorrect: false, order: 4 }
        ];
      
      case 'MultipleResponse':
        return [
          { text: '', isCorrect: true, order: 1 },
          { text: '', isCorrect: false, order: 2 },
          { text: '', isCorrect: false, order: 3 },
          { text: '', isCorrect: false, order: 4 }
        ];
      
      case 'TrueFalse':
        return [
          { text: 'Tačno', isCorrect: true, order: 1 },
          { text: 'Netačno', isCorrect: false, order: 2 }
        ];
      
      case 'FillInBlank':
        return [
          { text: '', isCorrect: true, order: 1 }
        ];
      
      default:
        return [];
    }
  }

  onAnswerCorrectChange(questionIndex: number, optionIndex: number): void {
    const question = this.quiz.questions[questionIndex];
    const option = question.answerOptions[optionIndex];
    
    if (question.type === 'MultipleChoice') {
      // Samo jedan odgovor može biti tačan
      question.answerOptions.forEach((opt, idx) => {
        opt.isCorrect = idx === optionIndex;
      });
    }
    // Za MultipleResponse, više odgovora može biti tačno - ne radimo ništa
  }

  moveQuestionUp(index: number): void {
    if (index > 0) {
      [this.quiz.questions[index], this.quiz.questions[index - 1]] = 
      [this.quiz.questions[index - 1], this.quiz.questions[index]];
      this.updateQuestionOrders();
    }
  }

  moveQuestionDown(index: number): void {
    if (index < this.quiz.questions.length - 1) {
      [this.quiz.questions[index], this.quiz.questions[index + 1]] = 
      [this.quiz.questions[index + 1], this.quiz.questions[index]];
      this.updateQuestionOrders();
    }
  }

  updateQuestionOrders(): void {
    this.quiz.questions.forEach((q, i) => q.order = i + 1);
  }

  updateAnswerOptionOrders(questionIndex: number): void {
    this.quiz.questions[questionIndex].answerOptions.forEach((opt, i) => opt.order = i + 1);
  }

  validateForm(): boolean {
    // Osnovna validacija
    if (!this.quiz.title.trim()) {
      alert('Naslov kviza je obavezan!');
      return false;
    }

    if (!this.quiz.categoryId) {
      alert('Odaberite kategoriju!');
      return false;
    }

    if (this.quiz.questions.length === 0) {
      alert('Kviz mora imati barem jedno pitanje!');
      return false;
    }

    // Validacija pitanja
    for (let i = 0; i < this.quiz.questions.length; i++) {
      const question = this.quiz.questions[i];
      
      if (!question.text.trim()) {
        alert(`Pitanje ${i + 1} mora imati tekst!`);
        return false;
      }

      if (question.answerOptions.length === 0) {
        alert(`Pitanje ${i + 1} mora imati barem jedan odgovor!`);
        return false;
      }

      // Proveri prazne opcije
      const emptyOptions = question.answerOptions.filter(opt => !opt.text.trim());
      if (emptyOptions.length > 0) {
        alert(`Pitanje ${i + 1} ima prazne opcije odgovora!`);
        return false;
      }

      // Proveri duplikate opcija
      const optionTexts = question.answerOptions.map(opt => opt.text.trim().toLowerCase());
      const uniqueTexts = new Set(optionTexts);
      if (optionTexts.length !== uniqueTexts.size) {
        alert(`Pitanje ${i + 1} ima duplikate opcija!`);
        return false;
      }

      // Proveri tačne odgovore
      const correctAnswers = question.answerOptions.filter(opt => opt.isCorrect);
      
      if (correctAnswers.length === 0) {
        alert(`Pitanje ${i + 1} mora imati barem jedan tačan odgovor!`);
        return false;
      }

      if (question.type === 'MultipleChoice' && correctAnswers.length !== 1) {
        alert(`Pitanje ${i + 1} (Više ponuđenih odgovora) mora imati tačno jedan tačan odgovor!`);
        return false;
      }
    }

    return true;
  }

  onSubmit(): void {
    if (!this.validateForm()) return;

    this.isLoading = true;

    if (this.isEditMode && this.quizId) {
      let success = false;
      this.adminService.updateQuiz(this.quizId, this.quiz)
        .pipe(finalize(() => {
          this.isLoading = false;
          if (success) {
            // Navigate after request fully finished
            console.log('Navigating to /admin/quizzes after update');
            this.router.navigate(['/admin/quizzes']);
          }
        }))
        .subscribe({
          next: (res) => {
            success = true;
            console.log('Quiz updated successfully', res);
            alert('Kviz uspešno ažuriran!');
          },
          error: (error: any) => {
            console.error('Error updating quiz:', error);
            alert('Greška pri ažuriranju kviza: ' + (error.error?.message || error.message));
          }
        });
    } else {
      let success = false;
      this.adminService.createQuiz(this.quiz)
        .pipe(finalize(() => {
          this.isLoading = false;
          if (success) {
            console.log('Navigating to /admin/quizzes after create');
            this.router.navigate(['/admin/quizzes']);
          }
        }))
        .subscribe({
          next: (res) => {
            success = true;
            console.log('Quiz created successfully', res);
            alert('Kviz uspešno kreiran!');
          },
          error: (error: any) => {
            console.error('Error creating quiz:', error);
            alert('Greška pri kreiranju kviza: ' + (error.error?.message || error.message));
          }
        });
    }
  }
}