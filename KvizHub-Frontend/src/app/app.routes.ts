import { Routes } from '@angular/router';
import { adminGuard } from './guards/admin.guard';
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
import { authGuard } from './guards/auth.guard';
import { DashboardComponent } from './components/dashboard/dashboard';
import { QuizListComponent } from './components/quiz/quiz-list/quiz-list';
import { QuizDetailComponent } from './components/quiz/quiz-detail/quiz-detail';
import { QuizSolvingComponent } from './components/quiz/quiz-solving/quiz-solving';
import { LeaderboardComponent } from './components/leaderboard/leaderboard/leaderboard';
import { UserProfileComponent } from './components/profile/user-profile/user-profile';
import { AdminDashboardComponent } from './components/admin/admin-dashboard/admin-dashboard';
import { UserManagementComponent } from './components/admin/user-management/user-management';
import { QuizManagementComponent } from './components/admin/quiz-management/quiz-management';
import { CategoryManagementComponent } from './components/admin/category-management/category-management';
import { ResultsOverviewComponent } from './components/admin/results-overview/results-overview';
import { QuizFormComponent } from './components/admin/quiz-form/quiz-form';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'quizzes', component: QuizListComponent, canActivate: [authGuard] },
  { path: 'quiz/:id', component: QuizDetailComponent, canActivate: [authGuard] },
  { path: 'quiz-solving/:id', component: QuizSolvingComponent, canActivate: [authGuard] },
  { path: 'leaderboard', component: LeaderboardComponent, canActivate: [authGuard] },
  { path: 'profile', component: UserProfileComponent, canActivate: [authGuard] },
  
  { path: 'admin', loadComponent: () => import('./components/admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent), canActivate: [adminGuard] },
  { path: 'admin/users', loadComponent: () => import('./components/admin/user-management/user-management').then(m => m.UserManagementComponent), canActivate: [adminGuard] },
  { path: 'admin/quizzes', loadComponent: () => import('./components/admin/quiz-management/quiz-management').then(m => m.QuizManagementComponent), canActivate: [adminGuard] },
  { path: 'admin/categories', loadComponent: () => import('./components/admin/category-management/category-management').then(m => m.CategoryManagementComponent), canActivate: [adminGuard] },
  { path: 'admin/results', loadComponent: () => import('./components/admin/results-overview/results-overview').then(m => m.ResultsOverviewComponent), canActivate: [adminGuard] },
  { path: 'admin/quizzes/new', loadComponent: () => import('./components/admin/quiz-form/quiz-form').then(m => m.QuizFormComponent), canActivate: [adminGuard] },

  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' }
];