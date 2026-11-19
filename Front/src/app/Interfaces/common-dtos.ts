export interface PagedRequest {
  page?: number;
  pageSize?: number;
}

export interface RangoFechasRequest {
  desde?: string | null;
  hasta?: string | null;
}

export interface GetMensajeResponse {
  mensaje: string;
}

export interface GetPagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}