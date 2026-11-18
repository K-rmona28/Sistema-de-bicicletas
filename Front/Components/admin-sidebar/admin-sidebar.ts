import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth';

interface NavItem {
  path: string;
  label: string;
  icon: string;
  delay: number;
  count?: number;
}

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-sidebar.html',
  styleUrl: './admin-sidebar.scss'
})
export class AdminSidebarComponent {
  private readonly authService = inject(AuthService);

  collapsed = signal(false);
  readonly logoutDelay = 0.28;

  readonly groups: { label: string; items: NavItem[] }[] = [
    {
      label: 'General',
      items: [
        { path: '/admin/dashboard', label: 'Dashboard', icon: 'fa-chart-pie', delay: 0.04 },
        { path: '/admin/bicicletas', label: 'Bicicletas', icon: 'fa-bicycle', delay: 0.08 },
        { path: '/admin/estaciones', label: 'Estaciones', icon: 'fa-location-dot', delay: 0.12 }
      ]
    },
    {
      label: 'Operación',
      items: [
        { path: '/admin/mantenimiento', label: 'Mantenimiento', icon: 'fa-screwdriver-wrench', delay: 0.16 },
        { path: '/admin/reportes', label: 'Reportes', icon: 'fa-triangle-exclamation', delay: 0.2 },
        { path: '/admin/usuarios', label: 'Usuarios', icon: 'fa-users', delay: 0.24 }
      ]
    }
  ];

  readonly system: NavItem[] = [
    { path: '/home', label: 'Vista usuario', icon: 'fa-house', delay: 0.26 }
  ];

  toggle(): void {
    this.collapsed.update((v) => !v);
  }

  logout(): void {
    this.authService.logout();
  }
}