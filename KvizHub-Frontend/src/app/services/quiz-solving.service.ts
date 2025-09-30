import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class QuizSolvingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/QuizSolving`;

  startQuiz(quizId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/start/${quizId}`);
  }

  submitQuiz(submission: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/submit`, submission);
  }

  getQuizResult(resultId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/result/${resultId}`);
  }

  getUserQuizHistory(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history`).pipe(
      catchError(error => {
        console.error('Error loading quiz history:', error);
        return of([]);
      })
    );
  }
}