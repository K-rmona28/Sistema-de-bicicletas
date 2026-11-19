import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { GetBicicletaResponse, GetEstacionResponse, GetReservaResponse } from '../../interfaces';
import { AuthService } from '../../services/auth';
import { ReservasService } from '../../services/reservas';

export interface SedeOption {
  id: number;
  name: string;
}

@Component({
  selector: 'app-rent-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './rent-dialog.html',
  styleUrl: './rent-dialog.scss'
})
export class RentDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly reservasService = inject(ReservasService);
  readonly dialogRef = inject(MatDialogRef<RentDialogComponent>);
  readonly data = inject<{
    bicicleta?: GetBicicletaResponse & { id?: number; name?: string };
    estaciones?: GetEstacionResponse[];
    reservaActiva?: GetReservaResponse | null;
  }>(MAT_DIALOG_DATA);

  errorMsg = signal<string | null>(null);
  submitting = signal(false);
  private userStorageKey = 'default';

  readonly today = this.getLocalToday();
  readonly paymentTypes = ['Tarjeta de crédito', 'Tarjeta débito', 'PSE', 'Efectivo en estación'];

  private readonly rawBike: any = this.data?.bicicleta ?? this.data ?? {};

  readonly bike = {
    id: Number(this.rawBike.idBicicleta ?? this.rawBike.id ?? 0),
    name: this.rawBike.name ?? `${this.rawBike.marca ?? ''} ${this.rawBike.modelo ?? 'Bicicleta'}`.trim(),
    pricePerHour: Number(this.rawBike.valor ?? 5000),
    stationId: Number(this.rawBike.idEstacion ?? 0),
    stationName: String(this.rawBike.nombreEstacion ?? '').trim(),
    status: String(this.rawBike.estado ?? 'DISPONIBLE').trim().toUpperCase()
  };

  readonly allSedes: SedeOption[] = (this.data?.estaciones ?? []).map((s) => ({
    id: s.idEstacion,
    name: s.nombre.trim()
  }));

  /** Devuelve únicamente la estación donde está ubicada la bicicleta */
  get sedes(): SedeOption[] {
    if (!this.bike.stationId) return [];

    const filtradas = this.allSedes.filter((s) => s.id === this.bike.stationId);
    if (filtradas.length > 0) return filtradas;

    return this.bike.stationName
      ? [{ id: this.bike.stationId, name: this.bike.stationName }]
      : [];
  }

  private readonly hoursSignal = signal(1);
  readonly total = computed(() => this.bike.pricePerHour * this.hoursSignal());

  form = this.fb.nonNullable.group({
    fullName: ['', Validators.required],
    document: ['', [Validators.required, Validators.pattern(/^\d{6,10}$/)]],
    phone: ['', [Validators.required, Validators.pattern(/^3\d{9}$/)]],
    pickupPoint: ['', Validators.required],
    hours: [1, [Validators.required, Validators.min(1), Validators.max(12)]],
    date: [this.today, Validators.required],
    time: ['', Validators.required],
    paymentType: ['', Validators.required]
  });

  ngOnInit(): void {
    this.form.controls.hours.valueChanges.subscribe((val) => {
      this.hoursSignal.set(Math.max(1, Number(val) || 1));
    });

    this.precargarDatosUsuario();

    if (this.sedes.length > 0 && this.bike.status === 'DISPONIBLE') {
      this.form.patchValue({ pickupPoint: this.sedes[0].name });
    } else {
      this.errorMsg.set('Esta bicicleta no está disponible para reservar en este momento.');
    }
  }

  private getLocalToday(): string {
  const now = new Date();
  const year = now.getFullYear();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}
  private precargarDatosUsuario(): void {
    const auth = this.authService as any;
    let user: any =
      (typeof auth.currentUser === 'function' ? auth.currentUser() : auth.currentUser) ??
      (typeof auth.usuario === 'function' ? auth.usuario() : auth.usuario) ??
      (typeof auth.user === 'function' ? auth.user() : auth.user) ??
      (typeof auth.getUsuario === 'function' ? auth.getUsuario() : null) ??
      (typeof auth.getUser === 'function' ? auth.getUser() : null);

    if (!user) {
      for (const key of Object.keys(localStorage)) {
        const raw = localStorage.getItem(key);
        if (!raw || !raw.startsWith('{') || !raw.includes('"email"')) continue;
        try {
          const parsed = JSON.parse(raw);
          user = parsed.usuario ?? parsed;
          break;
        } catch {}
      }
    }

    let fullName = '';
    let phone = '';
    let document = '';

    if (user) {
      fullName = (user.nombreCompleto ?? `${user.nombre ?? ''} ${user.apellido ?? ''}`).trim();
      phone = String(user.telefono ?? user.phone ?? '').trim();
      document = String(user.documento ?? user.cedula ?? '').trim();
      this.userStorageKey = String(user.idUsuario ?? user.email ?? 'default');
    } else {
      for (const key of Object.keys(localStorage)) {
        const val = localStorage.getItem(key);
        if (val && val.split('.').length === 3) {
          try {
            const payload = JSON.parse(atob(val.split('.')[1]));
            fullName = String(
              payload.name ??
              payload.unique_name ??
              payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
              ''
            ).trim();
            this.userStorageKey = String(payload.sub ?? payload.email ?? 'default');
            break;
          } catch {}
        }
      }
    }

    const savedExtra = localStorage.getItem(`cycleon_rent_profile_${this.userStorageKey}`);
    if (savedExtra) {
      try {
        const extra = JSON.parse(savedExtra);
        if (!document && extra.document) document = extra.document;
        if (!phone && extra.phone) phone = extra.phone;
        if (!fullName && extra.fullName) fullName = extra.fullName;
      } catch {}
    }

    this.form.patchValue({
      ...(fullName ? { fullName } : {}),
      ...(document ? { document } : {}),
      ...(phone ? { phone } : {})
    });
  }

  private guardarDatosContacto(fullName: string, document: string, phone: string): void {
    localStorage.setItem(
      `cycleon_rent_profile_${this.userStorageKey}`,
      JSON.stringify({ fullName, document, phone })
    );
  }

  invalid(controlName: keyof typeof this.form.controls): boolean {
    const ctrl = this.form.controls[controlName];
    return ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }

  changeHours(delta: number): void {
    const next = Math.min(12, Math.max(1, (this.form.controls.hours.value || 1) + delta));
    this.form.controls.hours.setValue(next);
  }

  close(): void {
    this.dialogRef.close();
  }

  submit(): void {
    this.errorMsg.set(null);

    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    const sedeSeleccionada = this.sedes.find((s) => s.name === this.form.controls.pickupPoint.value);
    const idEstacion = sedeSeleccionada?.id ?? this.bike.stationId;

    if (!idEstacion || idEstacion !== this.bike.stationId) {
      this.errorMsg.set(
        `Esta bicicleta solo está disponible para recogida en ${this.bike.stationName || 'su estación asignada'}.`
      );
      return;
    }

    this.submitting.set(true);
    const raw = this.form.getRawValue();

    this.reservasService
      .crearReserva({
        idBicicleta: this.bike.id,
        idEstacion
      })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.guardarDatosContacto(raw.fullName, raw.document, raw.phone);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.submitting.set(false);
          this.errorMsg.set(
            err?.error?.mensaje ?? 'No fue posible confirmar la reserva. Verifica disponibilidad.'
          );
        }
      });
  }
}