import { GetUsuarioResponse } from './usuario-dtos';

export interface LoginRequest {
  email: string;
  pass: string;
}

export interface SolicitarRestablecerAccesoRequest {
  email: string;
}

export interface RestablecerAccesoRequest {
  token: string;
  nuevaPass: string;
}

export interface GetLoginResponse {
  token: string;
  expiracion: string;
  usuario: GetUsuarioResponse;
}