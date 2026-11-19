import { EstadoReporteEnum } from './enums';
export { EstadoReporteEnum } from './enums';

export interface RegistrarReporteRequest {
  tipo: string;
  descripcion: string;
  idEstacion?: number | null;
  idBicicleta?: number | null;
}

export interface ActualizarEstadoReporteRequest {
  estado: EstadoReporteEnum;
}

export interface FiltroReportesRequest {
  estado?: EstadoReporteEnum | string | null;
  tipo?: string | null;
}

export interface GetReporteResponse {
  idReporte: number;
  tipo: string;
  descripcion: string;
  fechaHora: string;
  idEstacion?: number | null;
  idUsuario: number;
  idBicicleta?: number | null;
  estado: EstadoReporteEnum | string;
  nombreUsuario?: string;
  serialBicicleta?: string | null;
  modeloBicicleta?: string | null;
}