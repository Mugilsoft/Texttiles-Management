import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    constructor(private http: HttpClient) { }

    get<T>(endpoint: string): Observable<T[]> {
        return this.http.get<T[]>(`${environment.apiUrl}/${endpoint}`);
    }

    getSingle<T>(endpoint: string): Observable<T> {
        return this.http.get<T>(`${environment.apiUrl}/${endpoint}`);
    }

    getById<T>(endpoint: string, id: number): Observable<T> {
        return this.http.get<T>(`${environment.apiUrl}/${endpoint}/${id}`);
    }

    post<T>(endpoint: string, data: any): Observable<T> {
        return this.http.post<T>(`${environment.apiUrl}/${endpoint}`, data);
    }

    put<T>(endpoint: string, id: number, data: any): Observable<T> {
        return this.http.put<T>(`${environment.apiUrl}/${endpoint}/${id}`, data);
    }

    delete(endpoint: string, id: number): Observable<any> {
        return this.http.delete(`${environment.apiUrl}/${endpoint}/${id}`);
    }
}
