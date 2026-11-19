import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { HistoryDialogComponent } from '../../components/history-dialog/history-dialog';
import { ReportDialogComponent } from '../../components/report-dialog/report-dialog';
import { UserFormDialogComponent } from '../../components/user-form-dialog/user-form-dialog';
import {
  GetBicicletaResponse,
  GetEstacionResponse,
  GetPrestamoResponse,
  GetReservaResponse
} from '../../interfaces';
import { AuthService } from '../../services/auth';
import { BicicletasService } from '../../services/bike';
import { PagosService } from '../../services/pagos';
import { PrestamosService } from '../../services/prestamos';
import { ReservasService } from '../../services/reservas';
import { EstacionesService } from '../../services/station';
import { UsuariosService } from '../../services/usuarios';
import { buildImageUrl, extractErrorMessage } from '../../utils/api';

export interface ActiveRentalView {
  bike: { image: string; name: string; icon: string };
  category: string;
  sede: string;
  timeLeft: string;
  raw: GetPrestamoResponse;
}

export interface ActiveReservationView {
  bike: { image: string; name: string; icon: string };
  category: string;
  sede: string;
  fechaHora: string;
  raw: GetReservaResponse;
}

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile-page.html',
  styleUrl: './profile-page.scss'
})
export class ProfilePage implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly usuariosService = inject(UsuariosService);
  private readonly prestamosService = inject(PrestamosService);
  private readonly reservasService = inject(ReservasService);
  private readonly bicicletasService = inject(BicicletasService);
  private readonly estacionesService = inject(EstacionesService);
  private readonly pagosService = inject(PagosService);
  private readonly dialog = inject(MatDialog);

  private timerId: ReturnType<typeof setInterval> | null = null;
  private prestamosHistorial: GetPrestamoResponse[] = [];
  private bicicletasLista: GetBicicletaResponse[] = [];

  estacionesLista = signal<GetEstacionResponse[]>([]);
  estacionDestinoId = signal<number>(0);
  processing = signal(false);
  actionError = signal<string | null>(null);

  rental = signal<ActiveRentalView | null>(null);
  reservation = signal<ActiveReservationView | null>(null);

  get user() {
    const u = this.authService.currentUser();
    return {
      fullName: u ? `${u.nombre} ${u.apellido}` : 'Usuario CycleOn',
      role: u?.rol ?? 'Cliente',
      email: u?.email ?? '',
      phone: u?.telefono ?? 'No registrado',
      id: u?.idUsuario ?? 0
    };
  }

  ngOnInit(): void {
    this.usuariosService.obtenerMiPerfil().subscribe((u) => this.authService.updateStoredUser(u));
    this.cargarDatos();
  }

  ngOnDestroy(): void {
    if (this.timerId) clearInterval(this.timerId);
  }

  cargarDatos(): void {
    if (this.timerId) clearInterval(this.timerId);
    this.rental.set(null);
    this.estacionDestinoId.set(0);
    this.actionError.set(null);

    forkJoin({
      prestamos: this.prestamosService.getPrestamos(),
      reservas: this.reservasService.getReservas(),
      bicis: this.bicicletasService.getBicicletas(),
      estaciones: this.estacionesService.getEstaciones()
    }).subscribe(({ prestamos, reservas, bicis, estaciones }) => {
      this.prestamosHistorial = prestamos;
      this.bicicletasLista = bicis;
      this.estacionesLista.set(estaciones);

      // 1. Verificar si hay un préstamo activo
      const activo = prestamos.find((p) => p.estado === 'ACEPTADO' || p.estado === 'EN_PROCESO');
      if (activo) {
        this.reservation.set(null);
        if (!this.estacionDestinoId()) {
          this.estacionDestinoId.set(activo.estacionOrigen);
        }

        const bici = bicis.find((b) => b.idBicicleta === activo.idBicicleta);
        const actualizarTiempo = () => {
          const diffSec = Math.max(0, Math.floor((Date.now() - new Date(activo.fechaHoraInicio).getTime()) / 1000));
          const mins = Math.floor(diffSec / 60);
          const secs = diffSec % 60;
          const tiempoFormateado = `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')} min`;

          this.rental.set({
            bike: {
              image: buildImageUrl(bici?.imagenUrl),
              name: bici ? `${bici.marca} ${bici.modelo} (${bici.serial})` : activo.serial,
              icon: 'fa-bicycle'
            },
            category: bici?.tipo ?? 'Urbana',
            sede: activo.nombreEstacionOrigen,
            timeLeft: tiempoFormateado,
            raw: activo
          });
        };

        actualizarTiempo();
        if (this.timerId) clearInterval(this.timerId);
        this.timerId = setInterval(actualizarTiempo, 1000);
        return;
      }

      // No hay préstamo activo
      if (this.timerId) clearInterval(this.timerId);
      this.rental.set(null);

      // 2. Verificar si hay una reserva vigente pendiente de retirar
      const vigente = reservas.find((r) => r.estado === 'VIGENTE');
      if (vigente) {
        const bici = bicis.find((b) => b.idBicicleta === vigente.idBicicleta);
        this.reservation.set({
          bike: {
            image: buildImageUrl(bici?.imagenUrl),
            name: bici ? `${bici.marca} ${bici.modelo} (${bici.serial})` : `Bicicleta #${vigente.idBicicleta}`,
            icon: 'fa-bicycle'
          },
          category: bici?.tipo ?? 'Urbana',
          sede: vigente.nombreEstacion,
          fechaHora: vigente.fechaHoraReserva,
          raw: vigente
        });
      } else {
        this.reservation.set(null);
      }
    });
  }

  iniciarViajeDesdeReserva(): void {
    const res = this.reservation();
    if (!res || this.processing()) return;

    this.processing.set(true);
    this.actionError.set(null);

    this.prestamosService
      .crearPrestamo({
        idBicicleta: res.raw.idBicicleta,
        estacionOrigen: res.raw.idEstacion,
        idReserva: res.raw.idReserva
      })
      .subscribe({
        next: () => {
          this.processing.set(false);
          this.cargarDatos();
        },
        error: (err) => {
          this.processing.set(false);
          this.actionError.set(extractErrorMessage(err, 'No se pudo iniciar el préstamo.'));
        }
      });
  }

  cancelarReservaActiva(): void {
    const res = this.reservation();
    if (!res || this.processing()) return;

    this.processing.set(true);
    this.actionError.set(null);

    this.reservasService.cancelarReserva(res.raw.idReserva).subscribe({
      next: () => {
        this.processing.set(false);
        this.cargarDatos();
      },
      error: (err) => {
        this.processing.set(false);
        this.actionError.set(extractErrorMessage(err, 'No se pudo cancelar la reserva.'));
      }
    });
  }

  editProfile(): void {
  const u = this.authService.currentUser();
  if (!u) return;

  const ref = this.dialog.open(UserFormDialogComponent, {
    width: '540px',
    data: { modo: 'editarPerfil', usuario: u }
  });

  ref.afterClosed().subscribe((actualizado) => {
    if (actualizado && typeof actualizado === 'object') {
      this.authService.updateStoredUser(actualizado);
    }
  });
}

  viewHistory(): void {
    this.dialog.open(HistoryDialogComponent, {
      width: '850px',
      data: { prestamos: this.prestamosHistorial }
    });
  }

  reportProblem(): void {
    const ref = this.dialog.open(ReportDialogComponent, {
      width: '520px',
      data: { estaciones: this.estacionesLista(), bicicletas: this.bicicletasLista }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarDatos());
  }

  finalizarViaje(): void {
    const actual = this.rental();
    const destino = this.estacionDestinoId() || actual?.raw.estacionOrigen;
    if (!actual || !destino || this.processing()) return;

    this.processing.set(true);
    this.actionError.set(null);

    this.prestamosService.finalizarPrestamo(actual.raw.idPrestamo, { estacionDestino: destino }).subscribe({
      next: (res) => {
        this.processing.set(false);
        this.pagosService
          .registrarPago({ idPrestamo: res.idPrestamo, metodoPago: 'BILLETERA_DIGITAL' })
          .subscribe(() => this.pagosService.procesarPagos().subscribe());
        this.cargarDatos();
      },
      error: (err) => {
        this.processing.set(false);
        this.actionError.set(extractErrorMessage(err, 'No se pudo finalizar el viaje.'));
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}