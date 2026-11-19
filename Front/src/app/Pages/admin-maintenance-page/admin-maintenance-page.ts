import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MaintenanceFormDialogComponent } from '../../components/maintenance-form-dialog/maintenance-form-dialog';
import { GetMantenimientoResponse } from '../../interfaces';
import { BicicletasService } from '../../services/bike';
import { MantenimientoService } from '../../services/mantenimiento';

export interface MaintenanceOrderView {
  id: number;
  code: string;
  priority: 'Alta' | 'Media' | 'Baja';
  description: string;
  model: string;
  service: string;
  kind: string;
  reason: string;
  status: 'Pendiente' | 'En reparación' | 'Completado';
  progress: number;
  date: string;
  raw: GetMantenimientoResponse;
}

@Component({
  selector: 'app-admin-maintenance-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-maintenance-page.html',
  styleUrl: './admin-maintenance-page.scss'
})
export class AdminMaintenancePage implements OnInit {
  private readonly mantenimientoService = inject(MantenimientoService);
  private readonly bicicletasService = inject(BicicletasService);
  private readonly dialog = inject(MatDialog);

  ordenes = signal<MaintenanceOrderView[]>([]);

  pendientes = computed(() => this.ordenes().filter((o) => o.status === 'Pendiente'));
  enReparacion = computed(() => this.ordenes().filter((o) => o.status === 'En reparación'));
  completadas = computed(() => this.ordenes().filter((o) => o.status === 'Completado'));

  highCount = computed(() => this.ordenes().filter((o) => o.priority === 'Alta' && o.status !== 'Completado').length);

  readonly columns = [
    {
      status: 'Pendiente',
      tone: 'warn',
      icon: 'fa-clock',
      title: 'Pendientes',
      items: this.pendientes,
      empty: 'No hay mantenimientos programados'
    },
    {
      status: 'En reparación',
      tone: 'info',
      icon: 'fa-wrench',
      title: 'En reparación',
      items: this.enReparacion,
      empty: 'Ninguna bicicleta en taller'
    },
    {
      status: 'Completado',
      tone: 'ok',
      icon: 'fa-circle-check',
      title: 'Completados',
      items: this.completadas,
      empty: 'Aún no hay mantenimientos finalizados'
    }
  ];

  ngOnInit(): void {
    this.cargarMantenimientos();
  }

  cargarMantenimientos(): void {
    this.mantenimientoService.getMantenimientos().subscribe((list) => {
      this.ordenes.set(
        list.map((m) => {
          const status =
            m.estado === 'FINALIZADO'
              ? 'Completado'
              : m.estado === 'EN_EJECUCION'
                ? 'En reparación'
                : 'Pendiente';

          const priority = m.tipo.toLowerCase().includes('correctivo') ? 'Alta' : 'Media';

          return {
            id: m.idMantenimiento,
            code: m.serial ?? `BICI-${m.idBicicleta}`,
            priority,
            description: m.descripcion ?? 'Sin descripción',
            model: m.modelo ?? `Bicicleta #${m.idBicicleta}`,
            service: m.tipo,
            kind: m.tipo,
            reason: m.descripcion ?? 'Mantenimiento programado',
            status,
            progress: status === 'Completado' ? 100 : status === 'En reparación' ? 50 : 0,
            date: m.fechaProgramada,
            raw: m
          };
        })
      );
    });
  }

  openCreateModal(): void {
    this.bicicletasService.getBicicletas().subscribe((bicicletas) => {
      const ref = this.dialog.open(MaintenanceFormDialogComponent, {
        width: '520px',
        data: { bicicletas }
      });
      ref.afterClosed().subscribe((ok) => ok && this.cargarMantenimientos());
    });
  }

  start(order: MaintenanceOrderView): void {
    this.mantenimientoService.iniciarMantenimiento(order.id).subscribe(() => this.cargarMantenimientos());
  }

  finish(order: MaintenanceOrderView): void {
    this.mantenimientoService.finalizarMantenimiento(order.id).subscribe(() => this.cargarMantenimientos());
  }

  fmt(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('es-CO', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}