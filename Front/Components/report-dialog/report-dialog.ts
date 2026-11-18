import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { GetBicicletaResponse, GetEstacionResponse } from '../../interfaces';
import { ReportesService } from '../../services/reportes';

@Component({
  selector: 'app-report-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './report-dialog.html',
  styleUrl: './report-dialog.scss'
})
export class ReportDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly reportesService = inject(ReportesService);
  readonly dialogRef = inject(MatDialogRef<ReportDialogComponent>);
  readonly data = inject<{ estaciones?: GetEstacionResponse[]; bicicletas?: GetBicicletaResponse[] }>(MAT_DIALOG_DATA);

  files = signal<File[]>([]);

  readonly reasons = [
    { value: 'Falla mecánica', label: 'Falla mecánica (frenos, cadena, llantas)', icon: 'fa-wrench' },
    { value: 'Daño estructural', label: 'Daño en marco, sillín o manubrio', icon: 'fa-bicycle' },
    { value: 'Robo o extravío', label: 'Robo o extravío de la bicicleta', icon: 'fa-shield-halved' },
    { value: 'Problema en estación', label: 'Anclaje o estación con fallas', icon: 'fa-location-dot' }
  ];

  form = this.fb.nonNullable.group({
    reason: ['', Validators.required],
    bike: ['', Validators.required],
    description: ['', [Validators.required, Validators.minLength(10)]]
  });

  get bikes(): string[] {
    return (this.data?.bicicletas ?? []).map((b) => `${b.serial} · ${b.marca} ${b.modelo}`);
  }

  invalid(controlName: keyof typeof this.form.controls): boolean {
    const ctrl = this.form.controls[controlName];
    return ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  onFiles(event: Event): void {
    const selected = Array.from((event.target as HTMLInputElement).files ?? []);
    if (selected.length) {
      this.files.update((prev) => [...prev, ...selected]);
    }
  }

  removeFile(index: number): void {
    this.files.update((prev) => prev.filter((_, i) => i !== index));
  }

  close(): void {
    this.dialogRef.close();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const serial = raw.bike.split(' · ')[0];
    const bici = (this.data?.bicicletas ?? []).find((b) => b.serial === serial);

    this.reportesService
      .registrarReporte({
        tipo: raw.reason,
        descripcion: raw.description,
        idBicicleta: bici?.idBicicleta ?? null,
        idEstacion: bici?.idEstacion ?? this.data?.estaciones?.[0]?.idEstacion ?? null
      })
      .subscribe({
        next: () => this.dialogRef.close(true)
      });
  }
}