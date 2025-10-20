import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const currentUser = authService.getCurrentUser();
  
  console.log('🔐 ADMIN GUARD CHECK:');
  console.log(' - Logged in:', authService.isLoggedIn());
  console.log(' - Current user:', currentUser);
  console.log(' - Is admin:', currentUser?.isAdmin);
  
  if (authService.isLoggedIn() && currentUser?.isAdmin) {
    console.log('✅ ADMIN ACCESS GRANTED');
    return true;
  } else {
    console.log('❌ ADMIN ACCESS DENIED - Redirecting to dashboard');
    router.navigate(['/dashboard']);
    return false;
  }
};