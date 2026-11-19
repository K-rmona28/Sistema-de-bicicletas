export interface CalcularValorServicioRequest {
  duracionMinutos: number;
  idTarifa?: number | null;
}

export interface CrearTarifaRequest {
  valorHora: number;
  vigenciaInicio: string;
  vigenciaFin?: string | null;
}

export interface ModificarTarifaRequest {
  valorHora: number;
  estado: string;
  vigenciaInicio: string;
  vigenciaFin?: string | null;
}

export interface GetTarifaResponse {
  idTarifa: number;
  valorHora: number;
  estado: string;
  vigenciaInicio: string;
  vigenciaFin?: string | null;
}

export interface GetValorServicioResponse {
  idTarifa: number;
  valorHora: number;
  duracionMinutos: number;
  valorTotal: number;
}