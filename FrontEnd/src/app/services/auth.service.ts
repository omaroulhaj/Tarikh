import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `https://localhost:7200/api/Auth`;

  constructor(private http: HttpClient) {}

  // Login method remains the same
  login(email: string, password: string): Observable<LoginResponse> {
    const loginData = { email, password };
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData)
      .pipe(
        map(response => {
          const token = response.token;
          if (token) {
            localStorage.setItem('token', token);
            return response;
          } else {
            throw new Error('No token received');
          }
        }),
        catchError(this.handleError)
      );
  }

  // Register method adapted for backend registration
  register(userData: { email: string, password: string, prenom: string, nom: string, phoneNumber: string, dateDeNaissance: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData)
      .pipe(catchError(this.handleError));
  }

  // Password reset methods
  requestPasswordReset(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/requestpasswordreset`, { email })
      .pipe(catchError(this.handleError));
  }

  resetPassword(email: string, token: string, newPassword: string): Observable<any> {
    const data = { email, token, newPassword };
    return this.http.post(`${this.apiUrl}/resetpassword`, data)
      .pipe(catchError(this.handleError));
  }

  // Utility methods for token handling
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // Error handling
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred. Please try again later.';

    if (error.error) {
        if (error.status === 401) {
            errorMessage = 'Invalid email or password. Please try again.';
        } else if (error.status === 500) {
            errorMessage = 'Server error. Please contact support if this issue persists.';
        } else if (typeof error.error === 'string') {
            errorMessage = error.error;
        } else if (error.error.errors) {
            // Extract specific messages from structured backend errors
            errorMessage = error.error.errors.email || error.error.errors.role || errorMessage;
        } else if (error.error.error) {
            // Catch any other custom 'error' field provided by the backend
            errorMessage = error.error.error;
        }
    }

    return throwError(() => new Error(errorMessage));
}
}
