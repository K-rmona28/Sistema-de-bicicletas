import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    FinalizarMantenimientoRequest,
    GetMantenimientoResponse,
    GetMensajeResponse,
    RegistrarMantenimientoRequest
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class MantenimientoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Mantenimiento`;

  getMantenimientos(): Observable<GetMantenimientoResponse[]> {
    return this.http.get<GetMantenimientoResponse[]>(this.baseUrl);
  }

  registrarMantenimiento(payload: RegistrarMantenimientoRequest): Observable<GetMantenimientoResponse> {
    return this.http.post<GetMantenimientoResponse>(this.baseUrl, payload);
  }

  iniciarMantenimiento(id: number): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/iniciar`, {});
  }

  finalizarMantenimiento(id: number, payload?: FinalizarMantenimientoRequest): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/finalizar`, payload ?? {});
  }
}