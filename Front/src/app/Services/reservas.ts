import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import { CrearReservaRequest, GetMensajeResponse, GetReservaResponse } from '../interfaces';

@Injectable({ providedIn: 'root' })
export class ReservasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Reservas`;

  getReservas(): Observable<GetReservaResponse[]> {
    return this.http.get<GetReservaResponse[]>(this.baseUrl);
  }

  crearReserva(payload: CrearReservaRequest): Observable<GetReservaResponse> {
    return this.http.post<GetReservaResponse>(this.baseUrl, payload);
  }

  cancelarReserva(id: number): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/cancelar`, {});
  }

  terminarReserva(id: number): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/terminar`, {});
  }
}