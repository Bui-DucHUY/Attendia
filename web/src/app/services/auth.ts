import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth`;
  
  // Signal to keep track of auth state reactively
  public isAuthenticated = signal<boolean>(this.hasToken());
  public instructorName = signal<string | null>(localStorage.getItem('instructorName'));

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(res => {
        if (res.token) {
          localStorage.setItem('jwt_token', res.token);
          localStorage.setItem('instructorName', res.instructorName);
          this.isAuthenticated.set(true);
          this.instructorName.set(res.instructorName);
        }
      }),
      catchError(err => throwError(() => new Error(err.error || 'Login failed')))
    );
  }

  register(instructor: any) {
    return this.http.post<any>(`${this.apiUrl}/register`, instructor).pipe(
      catchError(err => throwError(() => new Error(err.error || 'Registration failed')))
    );
  }

  logout() {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('instructorName');
    this.isAuthenticated.set(false);
    this.instructorName.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('jwt_token');
  }
}