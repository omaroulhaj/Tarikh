import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private apiUrl = environment.apiUrl+'/Tasks';

  constructor(private http: HttpClient) {}

  getUserTasksRecentToOld(): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http
      .get(`${this.apiUrl}/tasks/recent-to-old`, { headers })
      .pipe(catchError(this.handleError));
  }

  createTask(taskData: { date: string; title: string; status: string }, token: string): Observable<any> {
    const headers = this.getAuthHeaders(token);
    return this.http
      .post(`${this.apiUrl}/create-task`, taskData, { headers })
      .pipe(catchError(this.handleError));
  }

  getTasksByMonthAndYear(year: number, month: number): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http
      .get(`${this.apiUrl}/tasks/${year}/${month}`, { headers })
      .pipe(catchError(this.handleError));
  }

  updateTask(id: number, taskData: { date: string; title: string; status: string }): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http
      .put(`${this.apiUrl}/update-task/${id}`, taskData, { headers })
      .pipe(catchError(this.handleError));
  }

  deleteTask(id: number): Observable<any> {
    const headers = this.getAuthHeaders();
    return this.http
      .delete(`${this.apiUrl}/delete-task/${id}`, { headers })
      .pipe(catchError(this.handleError));
  }

  private getAuthHeaders(token?: string): HttpHeaders {
    const authToken = token || localStorage.getItem('token');
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    if (authToken) {
      headers = headers.set('Authorization', `Bearer ${authToken}`);
    }
    return headers;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Une erreur inattendue est survenue.';
    if (error.error) {
      if (error.status === 400) {
        errorMessage = 'Données invalides. Veuillez vérifier votre saisie.';
      } else if (error.status === 500) {
        errorMessage = 'Erreur interne du serveur.';
      } else if (error.error.message) {
        errorMessage = error.error.message;
      }
    }
    return throwError(() => new Error(errorMessage));
  }
}
