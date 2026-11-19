import { EstadoPagoEnum } from "./enums";

export interface RegistrarPagoRequest {
  idPrestamo: number;
  metodoPago: string;
}

export interface CancelarPagoRequest {
  motivo?: string | null;
}

export interface GetPagoResponse {
  idPago: number;
  idPrestamo: number;
  metodoPago: string;
  monto: number;
  fechaHoraPago: string;
  estado: EstadoPagoEnum | string;
}