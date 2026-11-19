export interface EnviarNotificacionEmailRequest {
  idUsuario: number;
  mensaje: string;
}

export interface GetNotificacionEmailResponse {
  idNotificacion: number;
  idUsuario: number;
  mensaje: string;
  fechaHora: string;
  estado: string;
}

export interface EnviarNotificacionWhatsAppRequest {
  idUsuario: number;
  mensaje: string;
}

export interface GetNotificacionWhatsAppResponse {
  idNotificacion: number;
  idUsuario: number;
  mensaje: string;
  fechaHora: string;
  estado: string;
}