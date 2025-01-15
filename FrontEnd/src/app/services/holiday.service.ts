import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HolidayService {
  private apiUrl = environment.apiUrl+'/Admin';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    let headers = new HttpHeaders();
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('token');
      if (token) {
        headers = headers.set('Authorization', `Bearer ${token}`);
      }
    }
    return headers;
  }

  // Méthode pour obtenir les jours fériés
  getHolidays(): Observable<any[]> {
    const headers = this.getAuthHeaders(); // Inclut l'en-tête d'authentification
    return this.http.get<any[]>(`${this.apiUrl}/create-jour-ferie`, { headers });
  }

  // Méthode pour ajouter un jour férié
  addHoliday(holiday: any): Observable<any> {
    const headers = this.getAuthHeaders(); // Inclut l'en-tête d'authentification
    return this.http.post<any>(`${this.apiUrl}/create-jour-ferie`, holiday, { headers });
  }

  // Méthode pour supprimer un jour férié
  deleteHoliday(id: number): Observable<void> {
    const headers = this.getAuthHeaders(); // Inclut l'en-tête d'authentification
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

  // Méthode pour mettre à jour un jour férié
  updateHoliday(id: number, holiday: any): Observable<any> {
    const headers = this.getAuthHeaders(); // Inclut l'en-tête d'authentification
    return this.http.put<any>(`${this.apiUrl}/${id}`, holiday, { headers });
  }
}
