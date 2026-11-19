import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { GetBicicletaResponse } from '../../interfaces';
import { buildImageUrl } from '../../utils/api';

@Component({
  selector: 'app-bike-detail-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './bike-detail-dialog.html',
  styleUrl: './bike-detail-dialog.scss'
})
export class BikeDetailDialogComponent {
  readonly dialogRef = inject(MatDialogRef<BikeDetailDialogComponent>);
  readonly data = inject<{ bicicleta: GetBicicletaResponse }>(MAT_DIALOG_DATA);

  get bike() {
    const b = this.data.bicicleta;
    return {
      image: b.imagenUrl ? buildImageUrl(b.imagenUrl) : '',
      name: `${b.marca} ${b.modelo}`,
      icon: 'fa-bicycle',
      model: `${b.modelo} (${b.tamano})`,
      color: b.especificacion || 'Estándar'
    };
  }

  get categoryName(): string {
    return this.data.bicicleta.tipo;
  }
}