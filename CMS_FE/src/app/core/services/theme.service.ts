import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private isDarkMode = new BehaviorSubject<boolean>(false);
  public isDarkMode$ = this.isDarkMode.asObservable();

  constructor() {
    this.initializeTheme();
    this.watchSystemTheme();
  }

  private initializeTheme(): void {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    this.setTheme(prefersDark);
  }

  private watchSystemTheme(): void {
    window.matchMedia('(prefers-color-scheme: dark)')
      .addEventListener('change', (e) => {
        this.setTheme(e.matches);
      });
  }

  private setTheme(isDark: boolean): void {
    this.isDarkMode.next(isDark);
    document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');
  }
}