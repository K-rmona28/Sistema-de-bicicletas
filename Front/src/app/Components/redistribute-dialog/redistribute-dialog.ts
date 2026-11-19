import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { GetBicicletaResponse, GetEstacionResponse } from '../../interfaces';
import { MovimientosService } from '../../services/movimientos';

@Component({
  selector: 'app-redistribute-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './redistribute-dialog.html',
  styleUrl: './redistribute-dialog.scss'
})
export class RedistributeDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly movimientosService = inject(MovimientosService);
  readonly dialogRef = inject(MatDialogRef<RedistributeDialogComponent>);
  readonly data = inject<{ estaciones: GetEstacionResponse[]; bicicletas: GetBicicletaResponse[] }>(MAT_DIALOG_DATA);

  readonly MIN = 1;

  form = this.fb.nonNullable.group({
    stationId: [0, [Validators.required, Validators.min(1)]],
    model: ['', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]]
  });

  get stations() {
    return (this.data?.estaciones ?? []).map((e) => ({
      id: e.idEstacion,
      name: e.nombre,
      free: Math.max(0, e.capacidad - e.bicicletasActuales)
    }));
  }

  get models(): string[] {
    const set = new Set((this.data?.bicicletas ?? []).map((b) => `${b.marca} ${b.modelo}`));
    return Array.from(set);
  }

  free(): number | null {
    const id = Number(this.form.controls.stationId.value);
    if (!id) return null;
    return this.stations.find((s) => s.id === id)?.free ?? null;
  }

  overflow(): boolean {
    const espacio = this.free();
    if (espacio === null) return false;
    return Number(this.form.controls.quantity.value) > espacio;
  }

  invalid(controlName: keyof typeof this.form.controls): boolean {
    const ctrl = this.form.controls[controlName];
    return ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  changeQty(delta: number): void {
    const next = Math.max(this.MIN, (this.form.controls.quantity.value || 1) + delta);
    this.form.controls.quantity.setValue(next);
  }

  close(): void {
    this.dialogRef.close();
  }

  submit(): void {
    if (this.form.invalid || this.overflow()) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const destinoId = Number(raw.stationId);
    const qty = Number(raw.quantity);

    const candidatas = (this.data?.bicicletas ?? [])
      .filter(
        (b) =>
          b.estado === 'DISPONIBLE' &&
          b.idEstacion !== null &&
          b.idEstacion !== destinoId &&
          `${b.marca} ${b.modelo}` === raw.model
      )
      .slice(0, qty);

    if (candidatas.length === 0) return;

    const porOrigen = new Map<number, number[]>();
    for (const b of candidatas) {
      const origen = b.idEstacion!;
      const arr = porOrigen.get(origen) ?? [];
      arr.push(b.idBicicleta);
      porOrigen.set(origen, arr);
    }

    const peticiones = Array.from(porOrigen.entries()).map(([idEstacionOrigen, idsBicicletas]) =>
      this.movimientosService.redistribuirBicicleta({
        idEstacionOrigen,
        idEstacionDestino: destinoId,
        motivo: `Redistribución de ${raw.model}`,
        idsBicicletas
      })
    );

    forkJoin(peticiones).subscribe({
      next: () => this.dialogRef.close(true)
    });
  }
}