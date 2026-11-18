import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { EstadoReporteEnum, GetReporteResponse } from '../../interfaces';
import { ReportesService } from '../../services/reportes';

@Component({
  selector: 'app-report-manage-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './report-manage-dialog.html',
  styleUrl: './report-manage-dialog.scss'
})
export class ReportManageDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly reportesService = inject(ReportesService);
  readonly dialogRef = inject(MatDialogRef<ReportManageDialogComponent>);
  readonly data = inject<{ reporte: GetReporteResponse }>(MAT_DIALOG_DATA);

  readonly statuses = ['Registrado', 'Abierto', 'En curso', 'Cerrado', 'Descartado'];

  form = this.fb.nonNullable.group({
    status: [this.mapEstadoLabel(this.data.reporte.estado), Validators.required],
    resolution: ['']
  });

  get report() {
    const r = this.data.reporte;
    return {
      id: r.idReporte,
      type: r.tipo,
      bikeCode: r.serialBicicleta ?? (r.idEstacion ? `Estación #${r.idEstacion}` : 'General'),
      bikeModel: r.modeloBicicleta ?? '',
      user: r.nombreUsuario ?? `Usuario #${r.idUsuario}`,
      createdAt: r.fechaHora,
      status: this.mapEstadoLabel(r.estado),
      description: r.descripcion,
      evidence: [] as string[]
    };
  }

  get mustExplain(): boolean {
    const val = this.form.controls.status.value;
    return val === 'Cerrado' || val === 'Descartado';
  }

  get resolutionInvalid(): boolean {
    const ctrl = this.form.controls.resolution;
    return this.mustExplain && (ctrl.dirty || ctrl.touched) && ctrl.value.trim().length < 10;
  }

  private mapEstadoLabel(estado: string): string {
    switch (estado) {
      case 'REGISTRADO':
        return 'Registrado';
      case 'EN_CURSO':
        return 'En curso';
      case 'CERRADO':
        return 'Cerrado';
      case 'DESCARTADO':
        return 'Descartado';
      default:
        return 'Abierto';
    }
  }

  fmt(dateStr: string): string {
    return new Date(dateStr).toLocaleString('es-CO');
  }

  close(): void {
    this.dialogRef.close();
  }

  submit(): void {
    this.form.controls.resolution.markAsTouched();
    if (this.resolutionInvalid) return;

    const mapToEnum: Record<string, EstadoReporteEnum> = {
      Registrado: EstadoReporteEnum.REGISTRADO,
      Abierto: EstadoReporteEnum.ABIERTO,
      'En curso': EstadoReporteEnum.EN_CURSO,
      Cerrado: EstadoReporteEnum.CERRADO,
      Descartado: EstadoReporteEnum.DESCARTADO
    };

    const estado = mapToEnum[this.form.controls.status.value] ?? EstadoReporteEnum.ABIERTO;

    this.reportesService.actualizarEstado(this.data.reporte.idReporte, { estado }).subscribe({
      next: () => this.dialogRef.close(true)
    });
  }
}