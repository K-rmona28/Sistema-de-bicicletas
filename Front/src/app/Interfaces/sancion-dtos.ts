import { EstadoSancionEnum } from "./enums";

export interface AplicarPenalizacionRequest {
  idUsuario: number;
  idPrestamo?: number | null;
  idReporte?: number | null;
  motivo: string;
  descripcion?: string | null;
  tipoDePenalizacion: string;
  dias?: number | null;
}

export interface ModificarSancionRequest {
  motivo: string;
  descripcion?: string | null;
  tipoDePenalizacion: string;
}

export interface RevocarSancionRequest {
  motivo: string;
}

export interface GetSancionResponse {
  idSancion: number;
  idUsuario: number;
  idPrestamo?: number | null;
  idReporte?: number | null;
  motivo: string;
  fechaHora: string;
  estado: EstadoSancionEnum | string; 
  descripcion?: string | null;
  tipoDePenalizacion: string;
}