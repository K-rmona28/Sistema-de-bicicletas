import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import { GetEstadisticaResponse, GetVelocidadMediaResponse } from '../interfaces';

@Injectable({ providedIn: 'root' })
export class EstadisticasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Estadisticas`;

  consultarHistorial(): Observable<GetEstadisticaResponse[]> {
    return this.http.get<GetEstadisticaResponse[]>(this.baseUrl);
  }

  verVelocidadMedia(): Observable<GetVelocidadMediaResponse> {
    return this.http.get<GetVelocidadMediaResponse>(`${this.baseUrl}/velocidad-media`);
  }
}