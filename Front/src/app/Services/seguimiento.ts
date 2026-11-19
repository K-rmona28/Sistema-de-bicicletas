import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import { GetUbicacionResponse, RegistrarUbicacionRequest } from '../interfaces';

@Injectable({ providedIn: 'root' })
export class SeguimientoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Seguimiento`;

  registrarUbicacion(payload: RegistrarUbicacionRequest): Observable<GetUbicacionResponse> {
    return this.http.post<GetUbicacionResponse>(this.baseUrl, payload);
  }

  consultarUbicacionActual(idPrestamo: number): Observable<GetUbicacionResponse> {
    return this.http.get<GetUbicacionResponse>(`${this.baseUrl}/prestamo/${idPrestamo}/actual`);
  }
} 