import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../constants/api';
import {
    EnviarNotificacionEmailRequest,
    EnviarNotificacionWhatsAppRequest,
    GetNotificacionEmailResponse,
    GetNotificacionWhatsAppResponse
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class NotificacionesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.API_URL}/Notificaciones`;

  enviarEmail(payload: EnviarNotificacionEmailRequest): Observable<GetNotificacionEmailResponse> {
    return this.http.post<GetNotificacionEmailResponse>(`${this.baseUrl}/email`, payload);
  }

  enviarWhatsApp(payload: EnviarNotificacionWhatsAppRequest): Observable<GetNotificacionWhatsAppResponse> {
    return this.http.post<GetNotificacionWhatsAppResponse>(`${this.baseUrl}/whatsapp`, payload);
  }
}