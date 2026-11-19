import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
  ActualizarBicicletaRequest,
  CrearBicicletaRequest,
  GetBicicletaResponse
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class BicicletasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Bicicletas`;

  getBicicletas(): Observable<GetBicicletaResponse[]> {
    return this.http.get<GetBicicletaResponse[]>(this.baseUrl);
  }

  getBicicleta(id: number): Observable<GetBicicletaResponse> {
    return this.http.get<GetBicicletaResponse>(`${this.baseUrl}/${id}`);
  }

  crearBicicleta(payload: CrearBicicletaRequest): Observable<GetBicicletaResponse> {
    return this.http.post<GetBicicletaResponse>(this.baseUrl, payload);
  }

  actualizarBicicleta(id: number, payload: ActualizarBicicletaRequest): Observable<GetBicicletaResponse> {
    return this.http.put<GetBicicletaResponse>(`${this.baseUrl}/${id}`, payload);
  }

  subirImagen(archivo: File): Observable<{ imagenUrl: string }> {
    const formData = new FormData();
    formData.append('archivo', archivo);
    return this.http.post<{ imagenUrl: string }>(`${this.baseUrl}/imagen`, formData);
  }

  subirImagenBicicleta(id: number, archivo: File): Observable<{ mensaje: string; imagenUrl: string }> {
    const formData = new FormData();
    formData.append('archivo', archivo);
    return this.http.post<{ mensaje: string; imagenUrl: string }>(`${this.baseUrl}/${id}/imagen`, formData);
  }

  cambiarEstadoBicicleta(id: number, body: { nuevoEstado: string; idEstacion?: number | null }) {
    return this.http.put<{ mensaje: string }>(`${this.baseUrl}/${id}/estado`, body);
  }

  eliminarBicicleta(id: number) {
    return this.http.delete<{ mensaje: string }>(`${this.baseUrl}/${id}`);
  }
}