import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  QuizAdminDTO, 
  CreateQuizDTO, 
  Category, 
  AdminStatsDTO,
  QuizDetailDTO,
  User 
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Admin`;

  // Korisnici
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  // Kvizovi
 getAllQuizzes(includeInactive: boolean = false): Observable<QuizAdminDTO[]> {
  return this.http.get<QuizAdminDTO[]>(`${this.apiUrl}/quizzes?includeInactive=${includeInactive}`);
}


  createQuiz(quizData: CreateQuizDTO): Observable<any> {
    return this.http.post(`${this.apiUrl}/quizzes`, quizData);
  }

  updateQuiz(quizId: number, quizData: CreateQuizDTO): Observable<any> {
    return this.http.put(`${this.apiUrl}/quizzes/${quizId}`, quizData);
  }

  toggleQuizStatus(quizId: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/quizzes/${quizId}/toggle`, {});
  }

  // Kategorije
  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }

  createCategory(category: Category): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/categories`, category);
  }

  updateCategory(categoryId: number, category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/categories/${categoryId}`, category);
  }

  deleteCategory(categoryId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/categories/${categoryId}`);
  }

  // Rezultati
  getAllResults(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/results`);
  }

  getResultsByQuiz(quizId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/results/quiz/${quizId}`);
  }

  getResultsByUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/results/user/${userId}`);
  }

  // Statistika
  getAdminStats(): Observable<AdminStatsDTO> {
    return this.http.get<AdminStatsDTO>(`${this.apiUrl}/stats`);
  }

  getQuizForEdit(quizId: number): Observable<QuizDetailDTO> {
  return this.http.get<QuizDetailDTO>(`${environment.apiUrl}/Quizzes/${quizId}`);
  }

  deleteUser(userId: number): Observable<any> {
  return this.http.delete(`${this.apiUrl}/users/${userId}`);
}

// Dodaj u AdminService klasu
deleteQuiz(quizId: number): Observable<any> {
  return this.http.delete(`${this.apiUrl}/quizzes/${quizId}`);
}
}