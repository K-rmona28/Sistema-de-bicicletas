import { GetUsuarioResponse } from './usuario-dtos';

export interface RegistrarClienteRequest {
  nombre: string;
  apellido: string;
  email: string;
  pass: string;
  telefono?: string | null;
  fechaDeNacimiento: string;
  nombreContactoEmergencia: string;
  telContactoEmergencia: string;
}

export interface ActualizarContactoEmergenciaRequest {
  nombreContactoEmergencia: string;
  telContactoEmergencia: string;
}

export interface ConsultarDisponibilidadRequest {
  idEstacion?: number | null;
  tipo?: string | null;
}

export interface GetClienteResponse {
  usuario: GetUsuarioResponse;
  nombreContactoEmergencia: string;
  telContactoEmergencia: string;
  maximoPrestamos: number;
}

export interface GetDisponibilidadEstacionResponse {
  idEstacion: number;
  nombre: string;
  ubicacion: string;
  capacidad: number;
  bicicletasDisponibles: number;
}