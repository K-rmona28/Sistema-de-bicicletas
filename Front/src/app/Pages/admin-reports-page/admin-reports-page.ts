import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild, computed, effect, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ReportManageDialogComponent } from '../../components/report-manage-dialog/report-manage-dialog';
import { EstadoReporteEnum, GetReporteResponse } from '../../interfaces';
import { ReportesService } from '../../services/reportes';

export interface ReportTableRow {
  id: number;
  type: string;
  bikeCode: string;
  bikeModel: string;
  user: string;
  status: 'Abierto' | 'En curso' | 'Cerrado';
  fechaHora: string;
  raw: GetReporteResponse;
}

@Component({
  selector: 'app-admin-reports-page',
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
  templateUrl: './admin-reports-page.html',
  styleUrl: './admin-reports-page.scss'
})
export class AdminReportsPage implements OnInit, AfterViewInit {
  private readonly reportesService = inject(ReportesService);
  private readonly dialog = inject(MatDialog);

  displayedColumns = ['id', 'type', 'bike', 'user', 'age', 'status', 'actions'];
  dataSource = new MatTableDataSource<ReportTableRow>([]);
  allRows = signal<ReportTableRow[]>([]);
  tab = signal<string>('Todos');

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  stats = computed(() => {
    const rows = this.allRows();
    return [
      { label: 'Total reportes', value: rows.length, tone: 'info', icon: 'fa-clipboard-list' },
      { label: 'Abiertos', value: rows.filter((r) => r.status === 'Abierto').length, tone: 'warn', icon: 'fa-circle-exclamation' },
      { label: 'En curso', value: rows.filter((r) => r.status === 'En curso').length, tone: 'accent', icon: 'fa-spinner' },
      { label: 'Cerrados', value: rows.filter((r) => r.status === 'Cerrado').length, tone: 'ok', icon: 'fa-circle-check' }
    ];
  });

  tabs = computed(() => {
    const rows = this.allRows();
    return [
      { label: 'Todos', count: rows.length },
      { label: 'Abierto', count: rows.filter((r) => r.status === 'Abierto').length },
      { label: 'En curso', count: rows.filter((r) => r.status === 'En curso').length },
      { label: 'Cerrado', count: rows.filter((r) => r.status === 'Cerrado').length }
    ];
  });

  constructor() {
    effect(() => {
      const selectedTab = this.tab();
      const rows = this.allRows();
      this.dataSource.data = selectedTab === 'Todos' ? rows : rows.filter((r) => r.status === selectedTab);
    });
  }

  ngOnInit(): void {
    this.cargarReportes();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  cargarReportes(): void {
    this.reportesService.getReportes().subscribe((list) => {
      this.allRows.set(
        list.map((r) => ({
          id: r.idReporte,
          type: r.tipo,
          bikeCode: r.serialBicicleta ?? (r.idEstacion ? `Estación #${r.idEstacion}` : 'General'),
          bikeModel: r.modeloBicicleta ?? '',
          user: r.nombreUsuario ?? `Usuario #${r.idUsuario}`,
          status: r.estado === 'CERRADO' ? 'Cerrado' : r.estado === 'EN_CURSO' ? 'En curso' : 'Abierto',
          fechaHora: r.fechaHora,
          raw: r
        }))
      );
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  urgent(row: ReportTableRow): boolean {
    return row.type.toLowerCase().includes('robo') || row.type.toLowerCase().includes('accidente');
  }

  overdue(row: ReportTableRow): boolean {
    const hours = (Date.now() - new Date(row.fechaHora).getTime()) / 3600000;
    return row.status !== 'Cerrado' && hours > 24;
  }

  ageLabel(row: ReportTableRow): string {
    const hours = Math.max(1, Math.floor((Date.now() - new Date(row.fechaHora).getTime()) / 3600000));
    return hours < 24 ? `${hours}h` : `${Math.floor(hours / 24)}d`;
  }

  take(row: ReportTableRow): void {
    this.reportesService.actualizarEstado(row.id, { estado: EstadoReporteEnum.EN_CURSO }).subscribe(() => {
      this.cargarReportes();
    });
  }

  openManageModal(row: ReportTableRow): void {
    const ref = this.dialog.open(ReportManageDialogComponent, {
      width: '540px',
      data: { reporte: row.raw }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarReportes());
  }
}