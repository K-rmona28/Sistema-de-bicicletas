export interface GenerarFacturaRequest {
  idPago: number;
}

export interface ExportarFacturaRequest {
  formato?: 'PDF';
}

export interface GetFacturaResponse {
  idFactura: number;
  idUsuario: number;
  nombreCliente: string;
  idPago: number;
  idPrestamo: number;
  fechaEmision: string;
  montoTotal: number;
  codigoFactura: string;
}