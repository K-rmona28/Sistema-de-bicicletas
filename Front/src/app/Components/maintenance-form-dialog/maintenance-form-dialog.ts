import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { GetBicicletaResponse } from '../../interfaces';
import { MantenimientoService } from '../../services/mantenimiento';

@Component({
  selector: 'app-maintenance-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './maintenance-form-dialog.html',
  styleUrl: './maintenance-form-dialog.scss'
})
export class MaintenanceFormDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly mantenimientoService = inject(MantenimientoService);
  readonly dialogRef = inject(MatDialogRef<MaintenanceFormDialogComponent>);
  readonly data = inject<{ bicicletas: GetBicicletaResponse[] }>(MAT_DIALOG_DATA);

  readonly today = new Date().toISOString().split('T')[0];
  readonly services = ['Ajuste de frenos', 'Cambio de llanta', 'Lubricación y cadena', 'Revisión eléctrica', 'Revisión general'];
  readonly kinds = ['Preventivo', 'Correctivo'];
  readonly priorities = ['Baja', 'Media', 'Alta'];

  get bikes() {
    return (this.data?.bicicletas ?? []).map((b) => ({
      idBicicleta: b.idBicicleta,
      code: b.serial,
      model: `${b.marca} ${b.modelo}`
    }));
  }

  private codeExistsValidator = (control: AbstractControl) => {
    const val = String(control.value ?? '').trim().toUpperCase();
    if (!val) return null;
    const found = this.bikes.some((b) => b.code.toUpperCase() === val);
    return found ? null : { notFound: true };
  };

  form = this.fb.nonNullable.group({
    code: ['', [Validators.required, this.codeExistsValidator]],
    date: [this.today, Validators.required],
    service: ['', Validators.required],
    kind: ['Preventivo', Validators.required],
    priority: ['Media', Validators.required],
    reason: ['', Validators.required],
    description: ['', [Validators.required, Validators.minLength(10)]]
  });

  get match() {
    const code = this.form.controls.code.value.trim().toUpperCase();
    return this.bikes.find((b) => b.code.toUpperCase() === code) ?? null;
  }

  invalid(controlName: keyof typeof this.form.controls): boolean {
    const ctrl = this.form.controls[controlName];
    return ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  close(): void {
    this.dialogRef.close();
  }

  submit(): void {
    if (this.form.invalid || !this.match) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.mantenimientoService
      .registrarMantenimiento({
        idBicicleta: this.match.idBicicleta,
        tipo: raw.kind,
        fechaProgramada: raw.date,
        fechaHoraInicio: null,
        descripcion: `[${raw.priority}] ${raw.service} - ${raw.reason}: ${raw.description}`
      })
      .subscribe({
        next: () => this.dialogRef.close(true)
      });
  }
}