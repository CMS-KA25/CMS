import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RoleType } from '../../shared/models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const user = this.authService.getCurrentUser();
    const requiredRoles = route.data['roles'] as RoleType[];

    if (!user) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    if (requiredRoles) {
      const userRole =
        typeof user.role === 'number'
          ? this.mapNumericRole(user.role)
          : user.role;
      if (!requiredRoles.includes(userRole)) {
        this.router.navigate(['/unauthorized']);
        return false;
      }
    }

    return true;
  }

  private mapNumericRole(role: number): RoleType {
    switch (role) {
      case 1: return RoleType.User;
      case 2: return RoleType.Staff;
      case 3: return RoleType.Doctor;
      case 4: return RoleType.Admin;
      case 5: return RoleType.Patient;
      default: return RoleType.User;
    }
  }
}
