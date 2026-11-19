import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter, map, startWith } from 'rxjs';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  styleUrl: './main-layout.scss',
  templateUrl: './main-layout.html',
})
export class MainLayout {
  private router = inject(Router);
  private authService = inject(AuthService);

  // URL actual, para saber si la barra va sobre un fondo oscuro
  private url = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => e.urlAfterRedirects),
      startWith(this.router.url),
    ),
    { initialValue: this.router.url },
  );

  readonly dark = computed(() => this.url().startsWith('/home'));

  readonly userName = computed(() => {
    const u = this.authService.currentUser();
    return u ? `${u.nombre} ${u.apellido}`.trim() : 'Usuario';
  });

  readonly links = [
    { label: 'Inicio',   path: '/home' },
    { label: 'Alquiler', path: '/alquiler' },
    { label: 'Perfil',   path: '/perfil' },
  ];

  logout() {
    this.authService.logout();
  }
}