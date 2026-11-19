import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { forkJoin, switchMap } from 'rxjs';
import { ESTADOS_BICICLETA } from '../../constants/api';
import { GetBicicletaResponse, GetEstacionResponse } from '../../interfaces';
import { BicicletasService } from '../../services/bike';

@Component({
  selector: 'app-bike-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './bike-form-dialog.html',
  styleUrl: './bike-form-dialog.scss'
})
export class BikeFormDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly bicicletasService = inject(BicicletasService);
  readonly dialogRef = inject(MatDialogRef<BikeFormDialogComponent>);
  readonly data = inject<{ bicicleta?: GetBicicletaResponse; estaciones: GetEstacionResponse[] }>(MAT_DIALOG_DATA);

  image = signal<File | null>(null);
  codeHint = 'Ej: BT-001';
  readonly statuses = ESTADOS_BICICLETA;

  readonly categories = [
    { code: 'Urbana', name: 'Urbana' },
    { code: 'Montaña', name: 'Montaña' },
    { code: 'Eléctrica', name: 'Eléctrica' },
    { code: 'Ruta', name: 'Ruta' }
  ];

  form = this.fb.nonNullable.group({
    model: ['', Validators.required],
    code: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9-]+$/)]],
    brand: ['', Validators.required],
    color: ['', Validators.required],
    category: ['', Validators.required],
    station: ['', Validators.required],
    status: ['DISPONIBLE', Validators.required],
    quantity: [1, [Validators.required, Validators.min(1), Validators.max(50)]]
  });

  get isEdit(): boolean {
    return !!this.data?.bicicleta;
  }

  get bike() {
    return this.data?.bicicleta ? { id: this.data.bicicleta.idBicicleta } : null;
  }

  get stations(): string[] {
    return (this.data?.estaciones ?? []).map((e) => e.nombre);
  }

  ngOnInit(): void {
    if (this.data?.bicicleta) {
      const b = this.data.bicicleta;
      this.form.controls.station.clearValidators();
      this.form.controls.station.updateValueAndValidity();

      this.form.patchValue({
        model: b.modelo,
        code: b.serial,
        brand: b.marca,
        color: b.especificacion ?? '',
        category: b.tipo,
        station: b.nombreEstacion ?? '',
        status: b.estado
      });
      this.form.controls.code.disable();
    }
  }

  invalid(controlName: keyof typeof this.form.controls): boolean {
    const ctrl = this.form.controls[controlName];
    return ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  changeQty(delta: number): void {
    const next = Math.min(50, Math.max(1, (this.form.controls.quantity.value || 1) + delta));
    this.form.controls.quantity.setValue(next);
  }

  onImage(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.image.set(file);
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
    const estacionObj = (this.data?.estaciones ?? []).find((e) => e.nombre === raw.station);
    const idEstacion = estacionObj ? estacionObj.idEstacion : null;

    if (this.isEdit && this.data.bicicleta) {
      const id = this.data.bicicleta.idBicicleta;
      forkJoin([
        this.bicicletasService.actualizarBicicleta(id, {
          tipo: raw.category,
          tamano: this.data.bicicleta.tamano || 'M',
          especificacion: raw.color || null,
          marca: raw.brand,
          modelo: raw.model,
          valor: this.data.bicicleta.valor || 15000,
          imagenUrl: this.data.bicicleta.imagenUrl ?? null
        }),
        this.bicicletasService.cambiarEstadoBicicleta(id, {
          nuevoEstado: raw.status,
          idEstacion
        })
      ]).subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          if (err?.status === 409) this.form.controls.code.setErrors({ taken: true });
        }
      });
      return;
    }

    const crearLote = (imagenUrl: string | null) => {
      const qty = Math.max(1, Number(raw.quantity) || 1);
      const peticiones = Array.from({ length: qty }, (_, idx) => {
        const serial = qty === 1 ? raw.code.toUpperCase() : `${raw.code.toUpperCase()}-${idx + 1}`;
        return this.bicicletasService.crearBicicleta({
          idEstacion,
          tipo: raw.category,
          tamano: 'M',
          especificacion: raw.color || null,
          marca: raw.brand,
          modelo: raw.model,
          serial,
          valor: 15000,
          imagenUrl
        });
      });
      return forkJoin(peticiones);
    };

    const file = this.image();
    const flujo$ = file
      ? this.bicicletasService.subirImagen(file).pipe(switchMap((up) => crearLote(up.imagenUrl)))
      : crearLote(null);

    flujo$.subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        if (err?.status === 409) this.form.controls.code.setErrors({ taken: true });
      }
    });
  }
}