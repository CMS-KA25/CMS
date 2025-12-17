import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RoleType } from '../../shared/models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const user = this.authService.getCurrentUser();
    const requiredRoles = route.data['roles'] as RoleType[];

    if (!user) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    if (requiredRoles && !requiredRoles.includes(user.role)) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}