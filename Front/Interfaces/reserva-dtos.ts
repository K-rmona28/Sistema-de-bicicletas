import { EstadoReservaEnum } from "./enums";

export interface CrearReservaRequest {
  idEstacion: number;
  idBicicleta: number;
}

export interface CancelarReservaRequest {
  motivo?: string | null;
}

export interface GetReservaResponse {
  idReserva: number;
  idUsuario: number;
  idEstacion: number;
  nombreEstacion: string;
  idBicicleta: number;
  fechaHoraReserva: string;
  estado: EstadoReservaEnum | string;
}