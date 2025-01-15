import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl+'/Auth';

  constructor(private http: HttpClient) {}

  // Login method
  login(email: string, password: string): Observable<LoginResponse> {
    const loginData = { email, password };
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData)
      .pipe(
        map(response => {
          console.log('Login response:', response); // Log pour vérifier la réponse
          const token = response.token;
          if (token) {
            localStorage.setItem('token', token); // Stocke le token dans le localStorage
            console.log('Token stored in localStorage:', token); // Log pour vérifier le stockage
            return response;
          } else {
            throw new Error('No token received');
          }
        }),
        catchError(this.handleError)
      );
  }

  // Register method
  register(userData: { email: string, password: string, prenom: string, nom: string, phoneNumber: string, dateDeNaissance: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData)
      .pipe(catchError(this.handleError));
  }

  // Confirm email method
  confirmEmail(userId: string, token: string): Observable<any> {
    const body = { userId, token };
    return this.http.post(`${this.apiUrl}/confirmemail`, body, {
      headers: { 'Content-Type': 'application/json' }
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Request password reset method
  requestPasswordReset(email: string): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/requestpasswordreset`,
      { email }
    ).pipe(
      catchError(error => {
        const errorMessage = error?.error?.message || error?.message || 'An unexpected error occurred. Please try again later.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  // Reset password method
  resetPassword(email: string, token: string, newPassword: string): Observable<any> {
    const data = { email, token, newPassword };
    return this.http.post(`${this.apiUrl}/resetpassword`, data)
      .pipe(catchError(this.handleError)); 
  }

  // Utility method to decode JWT token
  private decodeToken(token: string): any {
    try {
      return JSON.parse(atob(token.split('.')[1])); // Décode la partie payload du token
    } catch (e) {
      console.error('Error decoding token:', e);
      return null;
    }
  }

  // Check if the token is expired
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    const decodedToken = this.decodeToken(token);
    if (!decodedToken || !decodedToken.exp) return true;

    const expirationDate = new Date(decodedToken.exp * 1000); // Convertit la date d'expiration en millisecondes
    return expirationDate < new Date(); // Vérifie si la date d'expiration est passée
  }

  // Get user name from token
  getUserNameFromToken(): string {
    const token = this.getToken();
    if (!token) {
      return '';
    }
    const decodedToken = this.decodeToken(token);
    return decodedToken ? decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || '' : '';
  }

  // Token management
  private getToken(): string | null {
    return localStorage.getItem('token'); // Récupère le token depuis le localStorage
  }

  // Check if the user is logged in
  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(); // Vérifie si le token est présent et non expiré
  }

  // Logout method
  logout(): void {
    localStorage.removeItem('token'); // Supprime le token lors de la déconnexion
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