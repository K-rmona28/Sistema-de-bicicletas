import { HttpErrorResponse, HttpParams } from '@angular/common/http';
import { API_CONFIG } from '../constants/api';

export function buildImageUrl(imagenUrl?: string | null): string {
  if (!imagenUrl) return `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_BIKE_IMAGE}`;
  if (imagenUrl.startsWith('http://') || imagenUrl.startsWith('https://')) return imagenUrl;
  const path = imagenUrl.startsWith('/') ? imagenUrl : `/${imagenUrl}`;
  return `${API_CONFIG.BASE_URL}${path}`;
}

export function buildHttpParams(filters: Record<string, unknown>): HttpParams {
  let params = new HttpParams();
  for (const [key, value] of Object.entries(filters)) {
    if (value !== null && value !== undefined && value !== '') {
      params = params.set(key, String(value));
    }
  }
  return params;
}

export function extractErrorMessage(err: unknown, fallback = 'Ocurrió un error inesperado.'): string {
  if (err instanceof HttpErrorResponse) {
    const body = err.error;
    if (typeof body === 'string' && body.trim()) return body;
    if (body?.mensaje) return body.mensaje;
    if (body?.detail) return body.detail;
    if (body?.title) return body.title;
    if (body?.errors) {
      const firstKey = Object.keys(body.errors)[0];
      if (firstKey && Array.isArray(body.errors[firstKey])) {
        return body.errors[firstKey][0];
      }
    }
  }
  return fallback;
}

export function formatCop(valor?: number | null): string {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0
  }).format(valor ?? 0);
}