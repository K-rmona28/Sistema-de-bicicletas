import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ROLES } from '../constants/api';
import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    return router.createUrlTree(['/login']);
  }

  const expectedRole = route.data?.['role'] as string | undefined;
  const userRole = authService.currentUser()?.rol;

  if (!expectedRole || userRole === expectedRole) {
    return true;
  }

  return router.createUrlTree([userRole === ROLES.ADMINISTRADOR ? '/admin' : '/home']);
};