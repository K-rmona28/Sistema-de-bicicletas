import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BikeDetailDialogComponent } from '../../components/bike-detail-dialog/bike-detail-dialog';
import { CycleCardComponent } from '../../components/cycle-card/cycle-card';
import { RentDialogComponent } from '../../components/rent-dialog/rent-dialog';
import { GetBicicletaResponse, GetEstacionResponse, GetReservaResponse } from '../../interfaces';
import { BicicletasService } from '../../services/bike';
import { ReservasService } from '../../services/reservas';
import { EstacionesService } from '../../services/station';
import { buildImageUrl } from '../../utils/api';

export interface BikeCardModel extends GetBicicletaResponse {
  id: number;
  name: string;
  image: string;
}

@Component({
  selector: 'app-alquiler-page',
  standalone: true,
  imports: [CommonModule, MatListModule, MatIconModule, CycleCardComponent],
  templateUrl: './alquiler-page.html',
  styleUrl: './alquiler-page.scss'
})
export class AlquilerPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly dialog = inject(MatDialog);
  private readonly bicicletasService = inject(BicicletasService);
  private readonly estacionesService = inject(EstacionesService);
  private readonly reservasService = inject(ReservasService);

  readonly categories = [
    { code: 'Urbana', name: 'Urbana', icon: 'directions_bike' },
    { code: 'Montaña', name: 'Montaña', icon: 'terrain' },
    { code: 'Eléctrica', name: 'Eléctrica', icon: 'electric_bike' },
    { code: 'Ruta', name: 'Ruta', icon: 'speed' }
  ];

  categorySelected = signal<string>('ALL');
  estacionId = signal<number | null>(null);
  bikes = signal<BikeCardModel[]>([]);
  estaciones = signal<GetEstacionResponse[]>([]);
  reservaActiva = signal<GetReservaResponse | null>(null);

  filteredBikes = computed(() => {
    const cat = this.categorySelected();
    const est = this.estacionId();
    return this.bikes().filter((b) => {
      if (b.estado !== 'DISPONIBLE') return false;
      if (cat !== 'ALL' && b.tipo.toLowerCase() !== cat.toLowerCase()) return false;
      if (est !== null && b.idEstacion !== est) return false;
      return true;
    });
  });

  ngOnInit(): void {
    const estParam = this.route.snapshot.queryParamMap.get('estacion');
    if (estParam) this.estacionId.set(Number(estParam));
    this.cargarDatos();
  }

  cargarDatos(): void {
    forkJoin({
      bicis: this.bicicletasService.getBicicletas(),
      estaciones: this.estacionesService.getEstaciones(),
      reservas: this.reservasService.getReservas()
    }).subscribe(({ bicis, estaciones, reservas }) => {
      this.estaciones.set(estaciones);
      this.reservaActiva.set(reservas.find((r) => r.estado === 'VIGENTE') ?? null);
      this.bikes.set(
        bicis.map((b) => ({
          ...b,
          id: b.idBicicleta,
          name: `${b.marca} ${b.modelo}`,
          image: buildImageUrl(b.imagenUrl)
        }))
      );
    });
  }

  onRent(bike: BikeCardModel): void {
    const ref = this.dialog.open(RentDialogComponent, {
      width: '500px',
      data: {
        bicicleta: bike,
        estaciones: this.estaciones(),
        reservaActiva: this.reservaActiva()
      }
    });
    ref.afterClosed().subscribe((ok) => ok && this.cargarDatos());
  }

  onSelect(bike: BikeCardModel): void {
    this.dialog.open(BikeDetailDialogComponent, {
      width: '540px',
      data: { bicicleta: bike }
    });
  }
}