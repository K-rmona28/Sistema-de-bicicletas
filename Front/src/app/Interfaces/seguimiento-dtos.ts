export interface RegistrarUbicacionRequest {
  idPrestamo: number;
  latitud: number;
  longitud: number;
}

export interface GetUbicacionResponse {
  idSeguimiento: number;
  idPrestamo: number;
  latitud: number;
  longitud: number;
  fechaHora: string;
}