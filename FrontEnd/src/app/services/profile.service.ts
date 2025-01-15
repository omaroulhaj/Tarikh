import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {environment} from '../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private baseUrl = environment.apiUrl+'/Statistiques';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); // Récupère le token avec la clé 'token'
    console.log('Token retrieved from localStorage:', token); // Log pour vérifier le token
    return new HttpHeaders({
      'Authorization': `Bearer ${token}` // Ajoute le token dans l'en-tête
    });
  }

  getUserProfile(): Observable<any> {
    const headers = this.getHeaders();
    return this.http.get(`${this.baseUrl}/GetUserProfile`, { headers });
  }

  updateUserProfile(profileData: any): Observable<any> {
    const headers = this.getHeaders();
    return this.http.post(`${this.baseUrl}/UpdateUserProfile`, profileData, { headers });
  }

  changePassword(passwordData: any): Observable<any> {
    const headers = this.getHeaders();
    return this.http.post(`${this.baseUrl}/change-password`, passwordData, { headers });
  }

  deleteUserAccount(password: string): Observable<any> {
    const headers = this.getHeaders();
    return this.http.delete(`${this.baseUrl}/DeleteUserAccount`, { 
      headers,
      params: { password } 
    });
  }
}