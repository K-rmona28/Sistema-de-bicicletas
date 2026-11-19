import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    AplicarPenalizacionRequest,
    GetMensajeResponse,
    GetSancionResponse,
    ModificarSancionRequest,
    RevocarSancionRequest
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class SancionesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Sanciones`;

  getSanciones(): Observable<GetSancionResponse[]> {
    return this.http.get<GetSancionResponse[]>(this.baseUrl);
  }

  aplicarPenalizacion(payload: AplicarPenalizacionRequest): Observable<GetSancionResponse> {
    return this.http.post<GetSancionResponse>(this.baseUrl, payload);
  }

  modificarSancion(id: number, payload: ModificarSancionRequest): Observable<GetSancionResponse> {
    return this.http.put<GetSancionResponse>(`${this.baseUrl}/${id}`, payload);
  }

  revocarSancion(id: number, payload: RevocarSancionRequest): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/revocar`, payload);
  }
}