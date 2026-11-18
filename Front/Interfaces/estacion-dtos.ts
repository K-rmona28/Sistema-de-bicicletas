import { EstadoEstacionEnum } from "./enums";

export interface CrearEstacionRequest {
  nombre: string;
  ubicacion: string;
  capacidad: number;
}

export interface ActualizarEstacionRequest {
  nombre: string;
  ubicacion: string;
  capacidad: number;
  estado: EstadoEstacionEnum | string;
}

export interface AgregarBicicletaEstacionRequest {
  idBicicleta: number;
}

export interface RetirarBicicletaEstacionRequest {
  idBicicleta: number;
  motivo?: string | null;
}

export interface GetEstacionResponse {
  idEstacion: number;
  nombre: string;
  ubicacion: string;
  capacidad: number;
  estado: EstadoEstacionEnum | string;
  bicicletasActuales: number;
  bicicletasDisponibles: number;
}