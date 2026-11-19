import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import { CancelarPagoRequest, GetMensajeResponse, GetPagoResponse, RegistrarPagoRequest } from '../interfaces';

@Injectable({ providedIn: 'root' })
export class PagosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Pagos`;

  getPagos(): Observable<GetPagoResponse[]> {
    return this.http.get<GetPagoResponse[]>(this.baseUrl);
  }

  registrarPago(payload: RegistrarPagoRequest): Observable<GetPagoResponse> {
    return this.http.post<GetPagoResponse>(this.baseUrl, payload);
  }

  cancelarPago(id: number, payload?: CancelarPagoRequest): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/cancelar`, payload ?? {});
  }

  procesarPagos(): Observable<GetMensajeResponse> {
    return this.http.post<GetMensajeResponse>(`${this.baseUrl}/procesar`, {});
  }
}