import { Routes } from '@angular/router';
import { ROLES } from './constants/api';
import { authGuard } from './guards/auth-guard';
import { roleGuard } from './guards/role-guard';
import { MainLayout } from './layout/main-layout/main-layout';
import { AdminBikesPage } from './pages/admin-bikes-page/admin-bikes-page';
import { AdminDashboardPage } from './pages/admin-dashboard-page/admin-dashboard-page';
import { AdminMaintenancePage } from './pages/admin-maintenance-page/admin-maintenance-page';
import { AdminPage } from './pages/admin-page/admin-page';
import { AdminReportsPage } from './pages/admin-reports-page/admin-reports-page';
import { AdminStationsPage } from './pages/admin-stations-page/admin-stations-page';
import { AdminUsersPage } from './pages/admin-users-page/admin-users-page';
import { AlquilerPage } from './pages/alquiler-page/alquiler-page';
import { LoginPage } from './pages/login-page/login-page';
import { ProfilePage } from './pages/profile-page/profile-page';
import { UserHomePage } from './pages/user-home-page/user-home-page';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginPage},

  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      { path: 'home', component: UserHomePage },
      { path: 'alquiler', component: AlquilerPage },
      { path: 'perfil', component: ProfilePage }
    ]
  },

  {
    path: 'admin',
    component: AdminPage,
    canActivate: [roleGuard],
    data: { role: ROLES.ADMINISTRADOR },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardPage },
      { path: 'bicicletas', component: AdminBikesPage },
      { path: 'estaciones', component: AdminStationsPage },
      { path: 'mantenimiento', component: AdminMaintenancePage },
      { path: 'reportes', component: AdminReportsPage },
      { path: 'usuarios', component: AdminUsersPage },
      { path: '**', redirectTo: 'dashboard' }
    ]
  }
];