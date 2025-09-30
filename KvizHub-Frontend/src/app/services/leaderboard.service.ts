import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LeaderboardService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Leaderboard`;

  getGlobalLeaderboard(timeFrame: string = 'all'): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/global?timeFrame=${timeFrame}`).pipe(
      catchError(error => {
        console.error('Error loading leaderboard:', error);
        // Vrati prazan array umesto mock podataka
        return of([]);
      })
    );
  }

  getQuizLeaderboard(quizId: number, timeFrame: string = 'all'): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/quiz/${quizId}?timeFrame=${timeFrame}`).pipe(
      catchError(error => {
        console.error('Error loading quiz leaderboard:', error);
        return of([]);
      })
    );
  }

  getUserRank(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/my-rank`).pipe(
      catchError(error => {
        console.error('Error loading user rank:', error);
        return of(null);
      })
    );
  }
}