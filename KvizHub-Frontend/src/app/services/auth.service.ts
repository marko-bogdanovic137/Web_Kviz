import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, RegisterRequest, User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  login(loginData: LoginRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, loginData)
      .pipe(
        tap(response => {
          if (response.token) {
            this.setSession(response);
          }
        })
      );
  }

  register(registerData: RegisterRequest): Observable<any> {
  const { confirmPassword, ...dataToSend } = registerData;
  
  return this.http.post<any>(`${this.apiUrl}/register`, dataToSend)
    .pipe(
      tap(response => {
        if (response.token) {
          this.setSession(response);
        }
      })
    );
}

  private setSession(authData: any): void {
    localStorage.setItem('token', authData.token);
    
    const user: User = {
      id: authData.id,
      username: authData.username,
      email: authData.email,
      profileImage: authData.profileImage,
       createdAt: authData.createdAt ? new Date(authData.createdAt) : new Date()
    };
    
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
}