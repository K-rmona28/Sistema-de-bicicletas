import { EstadoBicicletaEnum } from "./enums";

export interface CrearBicicletaRequest {
  idEstacion?: number | null;
  tipo: string;
  tamano: string;
  especificacion?: string | null;
  marca: string;
  modelo: string;
  serial: string;
  valor: number;
  imagenUrl?: string | null;
}

export interface ActualizarBicicletaRequest {
  tipo: string;
  tamano: string;
  especificacion?: string | null;
  marca: string;
  modelo: string;
  valor: number;
  imagenUrl?: string | null;
}

export interface CambiarEstadoBicicletaRequest {
  nuevoEstado: EstadoBicicletaEnum | string;
  idEstacion?: number | null;
}

export interface GetBicicletaResponse {
  idBicicleta: number;
  idEstacion?: number | null;
  nombreEstacion?: string | null;
  tipo: string;
  tamano: string;
  especificacion?: string | null;
  estado: EstadoBicicletaEnum | string;
  marca: string;
  modelo: string;
  serial: string;
  valor: number;
  imagenUrl?: string | null;
}