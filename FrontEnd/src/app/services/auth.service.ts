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

  login(email: string, password: string): Observable<LoginResponse> {   
    const loginData = { email, password }; 
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData)
      .pipe(
        map(response => {
          const token = response.token;
          if (token) {
            localStorage.setItem('token', token); // Save token to localStorage
            return response;
          } else {
            throw new Error('No token received');
          }
        }),
        catchError(this.handleError)
      );
  }

  register(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/registre`, userData);
  }

  requestPasswordReset(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/requestpasswordreset`, { email });
  }

  resetPassword(email: string, token: string, newPassword: string): Observable<any> {
    const data = { email, token, newPassword };
    return this.http.post(`${this.apiUrl}/resetpassword`, data);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Something went wrong; please try again later.';
    if (error.status === 200 && typeof error.error === 'object') {
      console.error('Expected token missing in response:', error.error);
      errorMessage = 'Login response format is incorrect.';
    } else if (error.error instanceof ErrorEvent) {
      errorMessage = `An error occurred: ${error.error.message}`;
    } else {
      errorMessage = `${error.error.error}`;
    }
    return throwError(() => new Error(errorMessage));
  }
}
