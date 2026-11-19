import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { forkJoin } from 'rxjs';
import { UserFormDialogComponent } from '../../components/user-form-dialog/user-form-dialog';
import { GetSancionResponse, GetUsuarioResponse } from '../../interfaces';
import { AdministracionService } from '../../services/administracion';
import { PrestamosService } from '../../services/prestamos';
import { SancionesService } from '../../services/sanciones';
import { UsuariosService } from '../../services/usuarios';

export interface UserTableRow {
  id: number;
  nombre: string;
  apellido: string;
  document: string;
  status: 'Activo' | 'Bloqueado' | 'Sancionado';
  sanctions: number;
  trips: number;
  activeSanction?: GetSancionResponse;
  raw: GetUsuarioResponse;
}

@Component({
  selector: 'app-admin-users-page',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './admin-users-page.html',
  styleUrl: './admin-users-page.scss'
})
export class AdminUsersPage implements OnInit, AfterViewInit {
  private readonly usuariosService = inject(UsuariosService);
  private readonly sancionesService = inject(SancionesService);
  private readonly prestamosService = inject(PrestamosService);
  private readonly adminService = inject(AdministracionService);
  private readonly dialog = inject(MatDialog);

  displayedColumns = ['id', 'name', 'document', 'status', 'sanctions', 'trips', 'actions'];
  dataSource = new MatTableDataSource<UserTableRow>([]);
  allRows = signal<UserTableRow[]>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  stats = computed(() => {
    const rows = this.allRows();
    return [
      { label: 'Total usuarios', value: rows.length, tone: 'info', icon: 'fa-users' },
      { label: 'Activos', value: rows.filter((r) => r.status === 'Activo').length, tone: 'ok', icon: 'fa-user-check' },
      { label: 'Sancionados', value: rows.filter((r) => r.status === 'Sancionado').length, tone: 'warn', icon: 'fa-gavel' },
      { label: 'Bloqueados', value: rows.filter((r) => r.status === 'Bloqueado').length, tone: 'danger', icon: 'fa-ban' }
    ];
  });

  ngOnInit(): void {
    this.cargarUsuarios();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  cargarUsuarios(): void {
    // Dentro de cargarUsuarios():

    forkJoin({
      usuarios: this.usuariosService.filtrar({ page: 1, pageSize: 100 }),
      sanciones: this.sancionesService.getSanciones(),
      prestamos: this.prestamosService.getPrestamos()
    }).subscribe(({ usuarios, sanciones, prestamos }) => {
      const rows: UserTableRow[] = usuarios.items.map((u) => {
        const userSanciones = sanciones.filter((s) => s.idUsuario === u.idUsuario);
        const activa = userSanciones.find(
          (s) =>
            s.estado === 'ACTIVA_TEMPORAL' ||
            s.estado === 'ACTIVA_PERMANENTE' ||
            s.estado === 'PENDIENTE_PAGO'
        );
        const trips = prestamos.filter((p) => p.idUsuario === u.idUsuario).length;

        const status: 'Activo' | 'Bloqueado' | 'Sancionado' =
          u.estado === 'BLOQUEADO' ? 'Bloqueado' : activa ? 'Sancionado' : 'Activo';

        return {
          id: u.idUsuario,
          nombre: u.nombre,
          apellido: u.apellido,
          document: u.email,
          status,
          sanctions: userSanciones.length,
          trips,
          activeSanction: activa,
          raw: u
        };
      });

      this.allRows.set(rows);
      this.dataSource.data = rows;
    });
  }

  fullName(row: UserTableRow): string {
    return `${row.nombre} ${row.apellido}`;
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openCreateModal(): void {
    const ref = this.dialog.open(UserFormDialogComponent, {
      width: '540px',
      data: { modo: 'crearAdmin' }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarUsuarios());
  }

  openEditModal(row: UserTableRow): void {
    const ref = this.dialog.open(UserFormDialogComponent, {
      width: '540px',
      data: { modo: 'editarAdmin', usuario: row.raw }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarUsuarios());
  }

  toggleSanction(row: UserTableRow): void {
    if (row.activeSanction) {
      const motivoRevocacion = prompt(
        `Revocar sanción (${row.activeSanction.tipoDePenalizacion}). Ingresa el motivo:`,
        'Revocada desde panel de administración'
      );
      if (motivoRevocacion === null) return;

      this.sancionesService
        .revocarSancion(row.activeSanction.idSancion, {
          motivo: motivoRevocacion.trim() || 'Revocada desde panel de administración'
        })
        .subscribe(() => this.cargarUsuarios());
      return;
    }

    const diasInput = prompt(
      `¿Cuántos días durará la sanción para ${this.fullName(row)}?\n(Escribe 0 para sanción permanente)`,
      '3'
    );
    if (diasInput === null) return;

    const dias = Number(diasInput.trim());
    if (!Number.isInteger(dias) || dias < 0) {
      alert('Ingresa un número válido de días (0 o mayor).');
      return;
    }

    const motivo = prompt('Motivo de la sanción:', 'Incumplimiento de normas de uso');
    if (motivo === null) return;

    const esPermanente = dias === 0;

    this.sancionesService
      .aplicarPenalizacion({
        idUsuario: row.id,
        motivo: motivo.trim() || 'Incumplimiento de normas de uso',
        tipoDePenalizacion: esPermanente
          ? 'SUSPENSION PERMANENTE'
          : `SUSPENSION (${dias} ${dias === 1 ? 'día' : 'días'})`,
        dias: esPermanente ? null : dias
      })
      .subscribe(() => this.cargarUsuarios());
  }

  toggleBlock(row: UserTableRow): void {
    const nuevoEstado = row.raw.estado === 'BLOQUEADO' ? 'ACTIVO' : 'BLOQUEADO';
    this.usuariosService.cambiarEstado(row.id, { estado: nuevoEstado }).subscribe(() => this.cargarUsuarios());
  }
}