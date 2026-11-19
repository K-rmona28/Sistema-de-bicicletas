import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { API_CONFIG, ROLES, STORAGE_KEYS } from '../constants/api';
import {
  GetAdministradorResponse,
  GetClienteResponse,
  GetLoginResponse,
  GetMensajeResponse,
  GetUsuarioResponse,
  LoginRequest,
  RegistrarAdministradorRequest,
  RegistrarClienteRequest,
  RestablecerAccesoRequest,
  SolicitarRestablecerAccesoRequest
} from '../interfaces';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${API_CONFIG.API_URL}/auth`;

  private readonly _currentUser = signal<GetUsuarioResponse | null>(this.loadStoredUser());
  readonly currentUser = this._currentUser.asReadonly();
  readonly isAdmin = computed(() => this._currentUser()?.rol === ROLES.ADMINISTRADOR);

  login(payload: LoginRequest): Observable<GetLoginResponse> {
    return this.http.post<GetLoginResponse>(`${this.baseUrl}/login`, payload).pipe(
      tap((res) => {
        localStorage.setItem(STORAGE_KEYS.TOKEN, res.token);
        localStorage.setItem(STORAGE_KEYS.EXPIRACION, res.expiracion);
        localStorage.setItem(STORAGE_KEYS.USUARIO, JSON.stringify(res.usuario));
        this._currentUser.set(res.usuario);
      })
    );
  }

  registroCliente(payload: RegistrarClienteRequest): Observable<GetClienteResponse> {
    return this.http.post<GetClienteResponse>(`${this.baseUrl}/registro`, payload);
  }

  registroAdministrador(payload: RegistrarAdministradorRequest): Observable<GetAdministradorResponse> {
    return this.http.post<GetAdministradorResponse>(`${this.baseUrl}/registro-administrador`, payload);
  }

  solicitarRestablecimiento(payload: SolicitarRestablecerAccesoRequest): Observable<GetMensajeResponse> {
    return this.http.post<GetMensajeResponse>(`${this.baseUrl}/restablecer-acceso/solicitar`, payload);
  }

  confirmarRestablecimiento(payload: RestablecerAccesoRequest): Observable<GetMensajeResponse> {
    return this.http.post<GetMensajeResponse>(`${this.baseUrl}/restablecer-acceso/confirmar`, payload);
  }

  updateStoredUser(usuario: GetUsuarioResponse): void {
    localStorage.setItem(STORAGE_KEYS.USUARIO, JSON.stringify(usuario));
    this._currentUser.set(usuario);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEYS.TOKEN);
    localStorage.removeItem(STORAGE_KEYS.EXPIRACION);
    localStorage.removeItem(STORAGE_KEYS.USUARIO);
    this._currentUser.set(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
    const exp = localStorage.getItem(STORAGE_KEYS.EXPIRACION);
    if (!token) return false;
    if (exp && new Date(exp).getTime() <= Date.now()) {
      this.logout();
      return false;
    }
    return true;
  }

  private loadStoredUser(): GetUsuarioResponse | null {
    const raw = localStorage.getItem(STORAGE_KEYS.USUARIO);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as GetUsuarioResponse;
    } catch {
      return null;
    }
  }
}