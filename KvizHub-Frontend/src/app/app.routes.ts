import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
import { authGuard } from './guards/auth.guard';
import { DashboardComponent } from './components/dashboard/dashboard';
import { QuizListComponent } from './components/quiz/quiz-list/quiz-list';
import { QuizDetailComponent } from './components/quiz/quiz-detail/quiz-detail';
import { QuizSolvingComponent } from './components/quiz/quiz-solving/quiz-solving';
import { LeaderboardComponent } from './components/leaderboard/leaderboard/leaderboard';
import { UserProfileComponent } from './components/profile/user-profile/user-profile';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'quizzes', component: QuizListComponent, canActivate: [authGuard] },
   { path: 'quiz/:id', component: QuizDetailComponent, canActivate: [authGuard] },
   { path: 'quiz-solving/:id', component: QuizSolvingComponent, canActivate: [authGuard] },
   { path: 'leaderboard', component: LeaderboardComponent, canActivate: [authGuard] },
   { path: 'profile', component: UserProfileComponent, canActivate: [authGuard] },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' }, 
  { path: '**', redirectTo: '/dashboard' }
];