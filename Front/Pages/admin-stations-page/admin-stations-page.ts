import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RedistributeDialogComponent } from '../../components/redistribute-dialog/redistribute-dialog';
import { GetEstacionResponse } from '../../interfaces';
import { BicicletasService } from '../../services/bike';
import { EstacionesService } from '../../services/station';
import { extractErrorMessage } from '../../utils/api';

export interface StationTableRow {
  name: string;
  capacity: number;
  occupied: number;
  available: number;
  status: string;
  statusLabel: string;
  raw: GetEstacionResponse;
}

@Component({
  selector: 'app-admin-stations-page',
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
  templateUrl: './admin-stations-page.html',
  styleUrl: './admin-stations-page.scss'
})
export class AdminStationsPage implements OnInit, AfterViewInit {
  private readonly estacionesService = inject(EstacionesService);
  private readonly bicicletasService = inject(BicicletasService);
  private readonly dialog = inject(MatDialog);

  displayedColumns = ['name', 'capacity', 'available', 'status', 'actions'];
  dataSource = new MatTableDataSource<StationTableRow>([]);
  estaciones: GetEstacionResponse[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  private readonly labelsMap: Record<string, string> = {
    OPERATIVA: 'Operativa',
    SIN_STOCK: 'Sin stock',
    SATURADA: 'Saturada',
    INACTIVA: 'Inactiva'
  };

  ngOnInit(): void {
    this.cargarEstaciones();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  cargarEstaciones(): void {
    this.estacionesService.getEstaciones().subscribe((res) => {
      this.estaciones = res;
      this.dataSource.data = res.map((e) => ({
        name: e.nombre,
        capacity: e.capacidad,
        occupied: e.bicicletasActuales,
        available: e.bicicletasDisponibles,
        status: e.estado,
        statusLabel: this.labelsMap[e.estado] ?? e.estado,
        raw: e
      }));
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openRedistribute(): void {
    this.bicicletasService.getBicicletas().subscribe((bicicletas) => {
      const ref = this.dialog.open(RedistributeDialogComponent, {
        width: '600px',
        data: { estaciones: this.estaciones, bicicletas }
      });
      ref.afterClosed().subscribe((ok) => ok && this.cargarEstaciones());
    });
  }

  openCreateStation(): void {
    const nombre = prompt('Nombre de la nueva estación:', 'Estación Envigado');
    if (!nombre?.trim()) return;

    const ubicacion = prompt('Dirección o ubicación:', 'Parque Principal de Envigado');
    if (!ubicacion?.trim()) return;

    const capInput = prompt('Capacidad máxima de bicicletas:', '10');
    if (capInput === null) return;
    const capacidad = Number(capInput.trim());
    if (!Number.isInteger(capacidad) || capacidad <= 0) {
      alert('La capacidad debe ser un número entero mayor a 0.');
      return;
    }

    this.estacionesService
      .crearEstacion({ nombre: nombre.trim(), ubicacion: ubicacion.trim(), capacidad })
      .subscribe({
        next: () => this.cargarEstaciones(),
        error: (err) => alert(extractErrorMessage(err, 'No se pudo crear la estación.'))
      });
  }

  openExpandCapacity(): void {
    if (this.estaciones.length === 0) return;

    const listado = this.estaciones
      .map((e) => `${e.idEstacion}: ${e.nombre} (Capacidad: ${e.capacidad}, Ocupadas: ${e.bicicletasActuales})`)
      .join('\n');

    const idInput = prompt(`Escribe el ID de la estación a modificar:\n\n${listado}`);
    if (idInput === null) return;

    const estacion = this.estaciones.find((e) => e.idEstacion === Number(idInput.trim()));
    if (!estacion) {
      alert('ID de estación no válido.');
      return;
    }

    const capInput = prompt(
      `Nueva capacidad para ${estacion.nombre} (actual: ${estacion.capacidad}, mín: ${estacion.bicicletasActuales}):`,
      String(estacion.capacidad + 2)
    );
    if (capInput === null) return;

    const nuevaCapacidad = Number(capInput.trim());
    if (!Number.isInteger(nuevaCapacidad) || nuevaCapacidad < estacion.bicicletasActuales) {
      alert(`La capacidad debe ser mayor o igual a las bicicletas actuales (${estacion.bicicletasActuales}).`);
      return;
    }

    this.estacionesService
      .actualizarEstacion(estacion.idEstacion, {
        nombre: estacion.nombre,
        ubicacion: estacion.ubicacion,
        capacidad: nuevaCapacidad,
        estado: estacion.estado as any
      })
      .subscribe({
        next: () => this.cargarEstaciones(),
        error: (err) => alert(extractErrorMessage(err, 'No se pudo actualizar la capacidad.'))
      });
  }

  toggleStationActive(row: StationTableRow): void {
    const nuevoEstado = row.status === 'INACTIVA' ? 'OPERATIVA' : 'INACTIVA';
    this.estacionesService
      .actualizarEstacion(row.raw.idEstacion, {
        nombre: row.raw.nombre,
        ubicacion: row.raw.ubicacion,
        capacidad: row.raw.capacidad,
        estado: nuevoEstado as any
      })
      .subscribe({
        next: () => this.cargarEstaciones(),
        error: (err) => alert(extractErrorMessage(err, 'No se pudo cambiar el estado de la estación.'))
      });
  }
}