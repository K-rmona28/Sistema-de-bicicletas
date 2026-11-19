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
import { forkJoin } from 'rxjs';
import { BikeFormDialogComponent } from '../../components/bike-form-dialog/bike-form-dialog';
import { GetBicicletaResponse, GetEstacionResponse } from '../../interfaces';
import { BicicletasService } from '../../services/bike';
import { EstacionesService } from '../../services/station';

export interface BikeTableRow {
  id: number;
  code: string;
  model: string;
  status: string;
  station: string;
  raw: GetBicicletaResponse;
}

@Component({
  selector: 'app-admin-bikes-page',
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
  templateUrl: './admin-bikes-page.html',
  styleUrl: './admin-bikes-page.scss'
})
export class AdminBikesPage implements OnInit, AfterViewInit {
  private readonly bicicletasService = inject(BicicletasService);
  private readonly estacionesService = inject(EstacionesService);
  private readonly dialog = inject(MatDialog);

  displayedColumns = ['id', 'code', 'model', 'status', 'station', 'actions'];
  dataSource = new MatTableDataSource<BikeTableRow>([]);
  estaciones: GetEstacionResponse[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit(): void {
    this.cargarDatos();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  cargarDatos(): void {
    forkJoin({
      bicicletas: this.bicicletasService.getBicicletas(),
      estaciones: this.estacionesService.getEstaciones()
    }).subscribe(({ bicicletas, estaciones }) => {
      this.estaciones = estaciones;
      this.dataSource.data = bicicletas.map((b) => ({
        id: b.idBicicleta,
        code: b.serial,
        model: `${b.marca} ${b.modelo}`,
        status: b.estado,
        station: b.nombreEstacion ?? 'Sin estación',
        raw: b
      }));
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openCreateModal(): void {
    const ref = this.dialog.open(BikeFormDialogComponent, {
      width: '560px',
      data: { estaciones: this.estaciones }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarDatos());
  }

  openEditModal(row: BikeTableRow): void {
    const ref = this.dialog.open(BikeFormDialogComponent, {
      width: '560px',
      data: { bicicleta: row.raw, estaciones: this.estaciones }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarDatos());
  }

  deleteBike(row: BikeTableRow): void {
    if (!confirm(`¿Seguro que deseas eliminar la bicicleta ${row.code}?`)) return;

    this.bicicletasService.eliminarBicicleta(row.id).subscribe({
      next: () => this.cargarDatos() // usa el nombre de tu método que recarga la lista
    });
  }
}