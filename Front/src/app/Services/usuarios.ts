import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
  ActualizarDatosUsuarioRequest,
  CambiarEstadoUsuarioRequest,
  EliminarCuentaRequest,
  FiltrarUsuariosRequest,
  GetMensajeResponse,
  GetPagedResponse,
  GetUsuarioResponse
} from '../interfaces';
import { buildHttpParams } from '../utils/api';

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/usuarios`;

  obtenerMiPerfil(): Observable<GetUsuarioResponse> {
    return this.http.get<GetUsuarioResponse>(`${this.baseUrl}/me`);
  }

  actualizarMisDatos(payload: ActualizarDatosUsuarioRequest): Observable<GetUsuarioResponse> {
    return this.http.put<GetUsuarioResponse>(`${this.baseUrl}/me`, payload);
  }

  eliminarMiCuenta(payload: EliminarCuentaRequest): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/me`, { body: payload });
  }

  filtrar(filtros: FiltrarUsuariosRequest = {}): Observable<GetPagedResponse<GetUsuarioResponse>> {
    const params = buildHttpParams(filtros as Record<string, unknown>);
    return this.http.get<GetPagedResponse<GetUsuarioResponse>>(this.baseUrl, { params });
  }

  obtenerPorId(id: number): Observable<GetUsuarioResponse> {
    return this.http.get<GetUsuarioResponse>(`${this.baseUrl}/${id}`);
  }

  actualizarUsuario(
    id: number,
    payload: {
      nombre: string;
      apellido: string;
      email: string;
      telefono: string | null;
      fechaDeNacimiento: string;
      estado: string;
      nuevaPass?: string | null;
    }
  ): Observable<GetUsuarioResponse> {
    return this.http.put<GetUsuarioResponse>(`${this.baseUrl}/${id}`, payload);
  }

  cambiarEstado(id: number, payload: CambiarEstadoUsuarioRequest): Observable<GetMensajeResponse> {
    return this.http.put<GetMensajeResponse>(`${this.baseUrl}/${id}/estado`, payload);
  }
}