export interface RegistrarMovimientoRequest {
  idEstacionOrigen: number;
  idEstacionDestino: number;
  motivo: string;
  idsBicicletas: number[];
}

export interface FiltroMovimientosRequest {
  desde?: string | null;
  hasta?: string | null;
  idEstacion?: number | null;
}

export interface GetDetalleMovimientoResponse {
  idBicicleta: number;
  serial: string;
  marca: string;
  modelo: string;
}

export interface GetMovimientoResponse {
  idMovimiento: number;
  idUsuario: number;
  nombreAdministrador: string;
  idEstacionOrigen: number;
  nombreEstacionOrigen: string;
  idEstacionDestino: number;
  nombreEstacionDestino: string;
  fechaHora: string;
  motivo: string;
  bicicletas: GetDetalleMovimientoResponse[];
}