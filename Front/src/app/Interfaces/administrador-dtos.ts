import { PagedRequest } from './common-dtos';
import { EstadoBicicletaEnum } from './enums';
import { GetUsuarioResponse } from './usuario-dtos';

export interface RegistrarAdministradorRequest {
  nombre: string;
  apellido: string;
  email: string;
  pass: string;
  telefono?: string | null;
  fechaDeNacimiento: string;
  cargo: string;
}

export interface FiltroPrestamosActivosRequest extends PagedRequest {
  idEstacion?: number | null;
  idUsuario?: number | null;
}

export interface GenerarReporteGeneralRequest {
  desde: string;
  hasta: string;
}

export interface GenerarReportePorUsuarioRequest {
  desde: string;
  hasta: string;
}

export interface GetAdministradorResponse {
  usuario: GetUsuarioResponse;
  cargo: string;
}

export interface GetMonitoreoBicicletaResponse {
  idBicicleta: number;
  serial: string;
  estado: EstadoBicicletaEnum | string;
  idEstacion?: number | null;
  nombreEstacion?: string | null;
  idPrestamoActivo?: number | null;
  ultimaLatitud?: number | null;
  ultimaLongitud?: number | null;
  ultimaActualizacion?: string | null;
}

export interface GetReporteGeneralResponse {
  desde: string;
  hasta: string;
  totalPrestamos: number;
  prestamosFinalizados: number;
  prestamosCancelados: number;
  ingresosTotales: number;
  tiempoPromedioUsoMinutos: number;
  totalSanciones: number;
  reportesAbiertos: number;
  bicicletasEnMantenimiento: number;
}

export interface GetReportePorUsuarioResponse {
  idUsuario: number;
  nombreCompleto: string;
  desde: string;
  hasta: string;
  totalPrestamos: number;
  tiempoTotalUsoMinutos: number;
  gastoTotal: number;
  totalSanciones: number;
  totalReportes: number;
}