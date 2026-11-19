import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    CalcularValorServicioRequest,
    CrearTarifaRequest,
    GetTarifaResponse,
    GetValorServicioResponse,
    ModificarTarifaRequest
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class TarifasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Tarifas`;

  getTarifas(): Observable<GetTarifaResponse[]> {
    return this.http.get<GetTarifaResponse[]>(this.baseUrl);
  }

  mostrarTarifa(id: number): Observable<GetTarifaResponse> {
    return this.http.get<GetTarifaResponse>(`${this.baseUrl}/${id}`);
  }

  crearTarifa(payload: CrearTarifaRequest): Observable<GetTarifaResponse> {
    return this.http.post<GetTarifaResponse>(this.baseUrl, payload);
  }

  modificarTarifa(id: number, payload: ModificarTarifaRequest): Observable<GetTarifaResponse> {
    return this.http.put<GetTarifaResponse>(`${this.baseUrl}/${id}`, payload);
  }

  calcularValorServicio(payload: CalcularValorServicioRequest): Observable<GetValorServicioResponse> {
    return this.http.post<GetValorServicioResponse>(`${this.baseUrl}/calcular`, payload);
  }
}