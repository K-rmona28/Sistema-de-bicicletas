import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { GetPrestamoResponse, GetSancionResponse, GetUsuarioResponse } from '../../interfaces';
import { formatCop } from '../../utils/api';

@Component({
  selector: 'app-history-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './history-dialog.html',
  styleUrl: './history-dialog.scss'
})
export class HistoryDialogComponent {
  readonly dialogRef = inject(MatDialogRef<HistoryDialogComponent>);
  readonly data = inject<{
    prestamos: GetPrestamoResponse[];
    sanciones?: GetSancionResponse[];
    usuario?: GetUsuarioResponse;
  }>(MAT_DIALOG_DATA);

  get stats() {
    const list = this.data?.prestamos ?? [];
    const totalMin = list.reduce((acc, p) => acc + (p.tiempoUso ?? 0), 0);
    const totalPagado = list.reduce((acc, p) => acc + (p.costoTotal ?? 0), 0);

    return [
      { label: 'Viajes', value: list.length, icon: 'fa-bicycle' },
      { label: 'Minutos', value: `${totalMin} min`, icon: 'fa-clock' },
      { label: 'Invertido', value: formatCop(totalPagado), icon: 'fa-wallet' }
    ];
  }

  get sanctions() {
    return (this.data?.sanciones ?? []).map((s) => ({
      reason: s.motivo,
      date: new Date(s.fechaHora).toLocaleDateString('es-CO'),
      status: s.estado
    }));
  }

  get loans() {
    return (this.data?.prestamos ?? []).map((p, idx) => ({
      bike: p.serial,
      sede: `${p.nombreEstacionOrigen}${p.nombreEstacionDestino ? ' → ' + p.nombreEstacionDestino : ''}`,
      date: `${new Date(p.fechaHoraInicio).toLocaleString('es-CO')} #${idx + 1}`,
      duration: p.tiempoUso ? `${p.tiempoUso} min` : p.estado
    }));
  }

  close(): void {
    this.dialogRef.close();
  }
}