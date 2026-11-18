import { CommonModule } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

export interface CycleCardItem {
  id: number;
  name: string;
  image?: string;
  icon?: string;
  badge?: string;
  type?: string;
  tipo?: string;
  pricePerHour?: number;
  valor?: number;
  [key: string]: any;
}

@Component({
  selector: 'app-cycle-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './cycle-card.html',
  styleUrl: './cycle-card.scss'
})
export class CycleCardComponent {
  bike = input.required<CycleCardItem, any>({
    transform: (b: any) => ({
      ...b,
      name: b.name ?? `${b.marca ?? ''} ${b.modelo ?? ''}`.trim(),
      type: b.type ?? b.tipo ?? 'Urbana',
      pricePerHour: b.pricePerHour ?? b.valor ?? 0,
      icon: b.icon ?? 'fa-bicycle'
    })
  });

  select = output<any>();
  rent = output<any>();
}