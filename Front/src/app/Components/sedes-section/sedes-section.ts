import { CommonModule } from '@angular/common';
import { Component, computed, input, signal } from '@angular/core';

export interface SedeViewModel {
  id: number;
  name: string;
  zone: string;
  address: string;
  schedule: string;
  bikesAvailable: number;
  lat?: number;
  lng?: number;
}

@Component({
  selector: 'app-sedes-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sedes-section.html',
  styleUrl: './sedes-section.scss'
})
export class SedesSectionComponent {
  sedes = input<SedeViewModel[], any[]>([], {
    transform: (list: any[]) =>
      (list ?? []).map((e) => ({
        id: e.id ?? e.idEstacion ?? 0,
        name: e.name ?? e.nombre ?? 'Sede CycleOn',
        zone: e.zone ?? e.estado ?? 'Medellín',
        address: e.address ?? e.ubicacion ?? 'Ubicación disponible en mapa',
        schedule: e.schedule ?? 'Lun - Dom · 6:00 AM - 10:00 PM',
        bikesAvailable: e.bikesAvailable ?? e.bicicletasDisponibles ?? 0,
        lat: e.lat ?? e.latitud,
        lng: e.lng ?? e.longitud
      }))
  });

  private selectedId = signal<number | null>(null);

  active = computed<SedeViewModel>(() => {
    const list = this.sedes();
    if (!list.length) {
      return {
        id: 0,
        name: 'Cargando sedes...',
        zone: 'CycleOn',
        address: 'Medellín, Antioquia',
        schedule: '6:00 AM - 10:00 PM',
        bikesAvailable: 0
      };
    }
    return list.find((s) => s.id === this.selectedId()) ?? list[0];
  });

  select(s: SedeViewModel): void {
    this.selectedId.set(s.id);
  }

  mapsUrl(s: SedeViewModel): string {
    if (s.lat && s.lng) {
      return `https://www.google.com/maps/search/?api=1&query=${s.lat},${s.lng}`;
    }
    return `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(s.address + ', Medellín')}`;
  }
}