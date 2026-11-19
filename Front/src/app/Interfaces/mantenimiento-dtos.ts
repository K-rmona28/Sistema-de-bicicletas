import { EstadoMantenimientoEnum } from "./enums";

export interface RegistrarMantenimientoRequest {
  idBicicleta: number;
  tipo: string;
  fechaProgramada: string;
  fechaHoraInicio?: string | null;
  descripcion?: string | null;
}

export interface FinalizarMantenimientoRequest {
  fechaFin?: string | null;
  descripcion?: string | null;
}

export interface GetMantenimientoResponse {
  idMantenimiento: number;
  idBicicleta: number;
  serial?: string;
  modelo?: string;
  tipo: string;
  fechaProgramada: string;
  fechaHoraInicio?: string | null;
  fechaFin?: string | null;
  descripcion?: string | null;
  estado: EstadoMantenimientoEnum | string;
}