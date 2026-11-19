import { environment } from '../../environments/environment';

export const API_CONFIG = {
  API_URL: environment.apiUrl,
  BASE_URL: environment.apiUrl.replace(/\/api\/?$/, ''),
  DEFAULT_BIKE_IMAGE: '/uploads/bicicletas/default-bike.png'
} as const;

export const STORAGE_KEYS = {
  TOKEN: 'cycleon_token',
  EXPIRACION: 'cycleon_token_exp',
  USUARIO: 'cycleon_usuario'
} as const;

export const ROLES = {
  ADMINISTRADOR: 'Administrador',
  CLIENTE: 'Cliente'
} as const;

export const ESTADOS_BICICLETA = [
  'DISPONIBLE',
  'RESERVADA',
  'EN_PRESTAMO',
  'EN_MANTENIMIENTO',
  'DADA_DE_BAJA'
] as const;

export const ESTADOS_USUARIO = ['ACTIVO', 'BLOQUEADO', 'ELIMINADO'] as const;