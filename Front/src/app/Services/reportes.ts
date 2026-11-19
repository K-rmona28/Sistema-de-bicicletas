import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    ActualizarEstadoReporteRequest,
    FiltroReportesRequest,
    GetMensajeResponse,
    GetReporteResponse,
    RegistrarReporteRequest
} from '../interfaces';
import { buildHttpParams } from '../utils/api';

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Reportes`;

  getReportes(filtros: FiltroReportesRequest = {}): Observable<GetReporteResponse[]> {
    const params = buildHttpParams(filtros as Record<string, unknown>);
    return this.http.get<GetReporteResponse[]>(this.baseUrl, { params });
  }

  registrarReporte(payload: RegistrarReporteRequest): Observable<GetReporteResponse> {
    return this.http.post<GetReporteResponse>(this.baseUrl, payload);
  }

  actualizarEstado(id: number, payload: ActualizarEstadoReporteRequest): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/estado`, payload);
  }
}