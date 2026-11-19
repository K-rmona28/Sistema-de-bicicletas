import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import { FiltroMovimientosRequest, GetMovimientoResponse, RegistrarMovimientoRequest } from '../interfaces';
import { buildHttpParams } from '../utils/api';

@Injectable({ providedIn: 'root' })
export class MovimientosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Movimientos`;

  consultaHistorialMovimiento(filtros: FiltroMovimientosRequest = {}): Observable<GetMovimientoResponse[]> {
    const params = buildHttpParams(filtros as Record<string, unknown>);
    return this.http.get<GetMovimientoResponse[]>(this.baseUrl, { params });
  }

  redistribuirBicicleta(payload: RegistrarMovimientoRequest): Observable<{ mensaje: string; idMovimiento: number }> {
    return this.http.post<{ mensaje: string; idMovimiento: number }>(`${this.baseUrl}/redistribuir`, payload);
  }
}