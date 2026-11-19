import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    CrearPrestamoRequest,
    FinalizarPrestamoRequest,
    FinalizarPrestamoResponse,
    GetPrestamoResponse,
    IniciarPrestamoResponse
} from '../interfaces';
import { buildHttpParams } from '../utils/api';

@Injectable({ providedIn: 'root' })
export class PrestamosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Prestamos`;

  getPrestamos(filtros: { desde?: string; hasta?: string; estado?: string } = {}): Observable<GetPrestamoResponse[]> {
    const params = buildHttpParams(filtros);
    return this.http.get<GetPrestamoResponse[]>(this.baseUrl, { params });
  }

  crearPrestamo(payload: CrearPrestamoRequest): Observable<IniciarPrestamoResponse> {
    return this.http.post<IniciarPrestamoResponse>(this.baseUrl, payload);
  }

  finalizarPrestamo(id: number, payload: FinalizarPrestamoRequest): Observable<FinalizarPrestamoResponse> {
    return this.http.put<FinalizarPrestamoResponse>(`${this.baseUrl}/${id}/finalizar`, payload);
  }
}