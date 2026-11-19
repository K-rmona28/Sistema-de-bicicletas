import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
  ActualizarEstacionRequest,
  AgregarBicicletaEstacionRequest,
  CrearEstacionRequest,
  GetBicicletaResponse,
  GetEstacionResponse,
  GetMensajeResponse,
  RetirarBicicletaEstacionRequest
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class EstacionesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Estaciones`;

  getEstaciones(): Observable<GetEstacionResponse[]> {
    return this.http.get<GetEstacionResponse[]>(this.baseUrl);
  }

  verInformacion(id: number): Observable<GetEstacionResponse> {
    return this.http.get<GetEstacionResponse>(`${this.baseUrl}/${id}`);
  }

  verBicicletasActivas(id: number): Observable<GetBicicletaResponse[]> {
    return this.http.get<GetBicicletaResponse[]>(`${this.baseUrl}/${id}/bicicletas-activas`);
  }

  crearEstacion(payload: CrearEstacionRequest): Observable<GetEstacionResponse> {
    return this.http.post<GetEstacionResponse>(this.baseUrl, payload);
  }

  actualizarEstacion(id: number, payload: ActualizarEstacionRequest): Observable<GetEstacionResponse> {
    return this.http.put<GetEstacionResponse>(`${this.baseUrl}/${id}`, payload);
  }

  agregarBicicleta(id: number, payload: AgregarBicicletaEstacionRequest): Observable<GetMensajeResponse> {
    return this.http.post<GetMensajeResponse>(`${this.baseUrl}/${id}/bicicletas`, payload);
  }

  retirarBicicleta(id: number, payload: RetirarBicicletaEstacionRequest): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/retirar-bicicleta`, payload);
  }
}