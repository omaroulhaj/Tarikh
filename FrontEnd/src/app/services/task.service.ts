import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'https://localhost:7200/api/Tasks';

  constructor(private http: HttpClient) {}

  // Récupérer toutes les tâches de l'utilisateur (du plus récent au plus ancien)
  getUserTasksRecentToOld(): Observable<any> {
    return this.http.get(`${this.apiUrl}/tasks/recent-to-old`)
      .pipe(catchError(this.handleError));
  }

  // Créer une nouvelle tâche
  createTask(taskData: { date: string, title: string, status: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/create-task`, taskData)
      .pipe(catchError(this.handleError));
  }

  // Récupérer les tâches par mois et année
  getTasksByMonthAndYear(annee: number, mois: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/tasks/${annee}/${mois}`)
      .pipe(catchError(this.handleError));
  }

  // Mettre à jour une tâche existante
  updateTask(id: number, taskData: { date: string, title: string, status: string }): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-task/${id}`, taskData)
      .pipe(catchError(this.handleError));
  }

  // Supprimer une tâche
  deleteTask(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-task/${id}`)
      .pipe(catchError(this.handleError));
  }

  // Gestion des erreurs
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Une erreur inattendue est survenue. Veuillez réessayer plus tard.';

    if (error.error) {
      if (error.status === 401) {
        errorMessage = 'Utilisateur non connecté ou token expiré.';
      } else if (error.status === 500) {
        errorMessage = 'Erreur serveur. Veuillez contacter le support si ce problème persiste.';
      } else if (error.error.message) {
        errorMessage = error.error.message;
      }
    }

    return throwError(() => new Error(errorMessage));
  }
}
