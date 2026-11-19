import { PagedRequest } from './common-dtos';

export interface ActualizarDatosUsuarioRequest {
  nombre: string;
  apellido: string;
  telefono?: string | null;
  fechaDeNacimiento: string; // YYYY-MM-DD
}

export interface EliminarCuentaRequest {
  pass: string;
}

export interface CambiarEstadoUsuarioRequest {
  estado: 'ACTIVO' | 'BLOQUEADO';
}

export interface FiltrarUsuariosRequest extends PagedRequest {
  texto?: string | null;
  estado?: string | null;
  rol?: string | null;
}

export interface GetUsuarioResponse {
  idUsuario: number;
  nombre: string;
  apellido: string;
  email: string;
  estado: string;
  telefono?: string | null;
  fechaDeNacimiento: string;
  edad: number;
  fechaDeRegistro: string;
  rol: 'Administrador' | 'Cliente';
}