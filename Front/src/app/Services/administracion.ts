import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    FiltroHistorialRequest,
    FiltroPrestamosActivosRequest,
    GenerarReporteGeneralRequest,
    GenerarReportePorUsuarioRequest,
    GetMonitoreoBicicletaResponse,
    GetPagedResponse,
    GetPrestamoResponse,
    GetReporteGeneralResponse,
    GetReportePorUsuarioResponse
} from '../interfaces';
import { buildHttpParams } from '../utils/api';

@Injectable({ providedIn: 'root' })
export class AdministracionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/admin`;

  monitorearBicicleta(idBicicleta: number): Observable<GetMonitoreoBicicletaResponse> {
    return this.http.get<GetMonitoreoBicicletaResponse>(`${this.baseUrl}/bicicletas/${idBicicleta}/monitoreo`);
  }

  monitorearPrestamosActivos(filtros: FiltroPrestamosActivosRequest = {}): Observable<GetPagedResponse<GetPrestamoResponse>> {
    const params = buildHttpParams(filtros as Record<string, unknown>);
    return this.http.get<GetPagedResponse<GetPrestamoResponse>>(`${this.baseUrl}/prestamos-activos`, { params });
  }

  generarReporteGeneral(filtros: GenerarReporteGeneralRequest): Observable<GetReporteGeneralResponse> {
    const params = buildHttpParams(filtros as unknown as Record<string, unknown>);
    return this.http.get<GetReporteGeneralResponse>(`${this.baseUrl}/reportes/general`, { params });
  }

  consultarHistorialUsuario(
    idUsuario: number,
    filtros: FiltroHistorialRequest = {}
  ): Observable<GetPagedResponse<GetPrestamoResponse>> {
    const params = buildHttpParams(filtros as Record<string, unknown>);
    return this.http.get<GetPagedResponse<GetPrestamoResponse>>(`${this.baseUrl}/usuarios/${idUsuario}/historial`, {
      params
    });
  }

  generarReportePorUsuario(
    idUsuario: number,
    filtros: GenerarReportePorUsuarioRequest
  ): Observable<GetReportePorUsuarioResponse> {
    const params = buildHttpParams(filtros as unknown as Record<string, unknown>);
    return this.http.get<GetReportePorUsuarioResponse>(`${this.baseUrl}/usuarios/${idUsuario}/reporte`, { params });
  }
}