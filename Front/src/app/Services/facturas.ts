import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import { GenerarFacturaRequest, GetFacturaResponse } from '../interfaces';

@Injectable({ providedIn: 'root' })
export class FacturasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Facturas`;

  getFacturas(): Observable<GetFacturaResponse[]> {
    return this.http.get<GetFacturaResponse[]>(this.baseUrl);
  }

  consultarFactura(id: number): Observable<GetFacturaResponse> {
    return this.http.get<GetFacturaResponse>(`${this.baseUrl}/${id}`);
  }

  generarFactura(payload: GenerarFacturaRequest): Observable<GetFacturaResponse> {
    return this.http.post<GetFacturaResponse>(this.baseUrl, payload);
  }

  exportarFactura(id: number): Observable<{ mensaje: string; detalle: GetFacturaResponse }> {
    return this.http.get<{ mensaje: string; detalle: GetFacturaResponse }>(`${this.baseUrl}/${id}/exportar`, {
      params: { formato: 'PDF' }
    });
  }
}