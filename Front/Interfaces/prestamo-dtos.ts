import { PagedRequest } from './common-dtos';

export type EstadoPrestamo = 'ACEPTADO' | 'EN_PROCESO' | 'FINALIZADO' | 'CANCELADO' | 'VENCIDO';

export interface CrearPrestamoRequest {
  idReserva?: number | null;
  idBicicleta: number;
  estacionOrigen: number;
}

export interface FinalizarPrestamoRequest {
  estacionDestino: number;
}

export interface FiltroHistorialRequest extends PagedRequest {
  desde?: string | null;
  hasta?: string | null;
  estado?: EstadoPrestamo | string | null;
}

export interface GetPrestamoResponse {
  idPrestamo: number;
  idUsuario: number;
  idReserva?: number | null;
  idTarifa: number;
  valorHora: number;
  idBicicleta: number;
  serial: string;
  fechaHoraInicio: string;
  fechaHoraFin?: string | null;
  costoTotal?: number | null;
  estado: EstadoPrestamo | string;
  tiempoUso?: number | null;
  estacionOrigen: number;
  nombreEstacionOrigen: string;
  estacionDestino?: number | null;
  nombreEstacionDestino?: string | null;
}

export interface IniciarPrestamoResponse {
  idPrestamo: number;
  estado: string;
  fechaHoraInicio: string;
}

export interface FinalizarPrestamoResponse {
  mensaje: string;
  idPrestamo: number;
  costoTotal: number;
  tiempoUso: number;
}