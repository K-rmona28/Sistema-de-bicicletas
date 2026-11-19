import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { GetUsuarioResponse } from '../../interfaces';
import { AuthService } from '../../services/auth';
import { UsuariosService } from '../../services/usuarios';
import { extractErrorMessage } from '../../utils/api';

export type UserDialogMode = 'crearAdmin' | 'editarAdmin' | 'editarPerfil';

@Component({
  selector: 'app-user-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule],
  templateUrl: './user-form-dialog.html',
  styleUrl: './user-form-dialog.scss'
})
export class UserFormDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly usuariosService = inject(UsuariosService);
  readonly dialogRef = inject(MatDialogRef<UserFormDialogComponent>);
  readonly data = inject<{ modo?: UserDialogMode; usuario?: GetUsuarioResponse }>(MAT_DIALOG_DATA, {
    optional: true
  });

  readonly today = new Date().toISOString().split('T')[0];
  readonly roles = ['Cliente', 'Administrador'];
  readonly statuses = ['ACTIVO', 'BLOQUEADO'];

  readonly mode: UserDialogMode =
    this.data?.modo ?? (this.data?.usuario ? 'editarAdmin' : 'crearAdmin');

  showPassword = signal(false);
  birthDateSignal = signal('');
  submitting = signal(false);
  errorMsg = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    document: ['', [Validators.required, Validators.pattern(/^\d{6,10}$/)]],
    phone: ['', [Validators.required, Validators.pattern(/^3\d{9}$/)]],
    email: ['', [Validators.required, Validators.email]],
    birthDate: ['', Validators.required],
    role: ['Administrador', Validators.required],
    status: ['ACTIVO', Validators.required],
    username: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  age = computed<number | null>(() => {
    const val = this.birthDateSignal();
    if (!val) return null;
    const diff = Date.now() - new Date(val).getTime();
    const years = Math.floor(diff / (365.25 * 24 * 60 * 60 * 1000));
    return years >= 0 ? years : null;
  });

  constructor() {
    this.form.controls.birthDate.valueChanges.subscribe((v) => this.birthDateSignal.set(v));

    if (this.isEdit && this.data?.usuario) {
      const u = this.data.usuario;
      const fecha = String(u.fechaDeNacimiento ?? '').split('T')[0];

      // En edición quitamos obligatoriedad de campos que no aplica actualizar aquí
      this.form.controls.document.clearValidators();
      this.form.controls.document.updateValueAndValidity();
      this.form.controls.username.clearValidators();
      this.form.controls.username.updateValueAndValidity();

      // La contraseña pasa a ser opcional (solo valida longitud mínima si se escribe)
      this.form.controls.password.setValidators([Validators.minLength(6)]);
      this.form.controls.password.updateValueAndValidity();

      this.form.patchValue({
        firstName: u.nombre ?? '',
        lastName: u.apellido ?? '',
        phone: u.telefono ?? '',
        email: u.email ?? '',
        birthDate: fecha,
        role: u.rol ?? 'Cliente',
        status: u.estado === 'BLOQUEADO' ? 'BLOQUEADO' : 'ACTIVO'
      });

      this.birthDateSignal.set(fecha);
      this.form.controls.role.disable();

      if (this.isProfileMode) {
        // Restricciones cuando el cliente edita su propio perfil
        this.form.controls.email.disable();
        this.form.controls.status.disable();
      }
    }
  }

  get isEdit(): boolean {
    return this.mode === 'editarAdmin' || this.mode === 'editarPerfil';
  }

  get isProfileMode(): boolean {
    return this.mode === 'editarPerfil';
  }

  get user() {
    return this.data?.usuario ? { id: this.data.usuario.idUsuario } : null;
  }

  msg(controlName: keyof typeof this.form.controls): string | null {
    const ctrl = this.form.controls[controlName];
    if (!ctrl.invalid || (!ctrl.dirty && !ctrl.touched)) return null;
    if (ctrl.hasError('required')) return 'Campo obligatorio';
    if (ctrl.hasError('email')) return 'Correo inválido';
    if (ctrl.hasError('minlength')) return 'Mínimo 6 caracteres';
    if (ctrl.hasError('pattern')) return 'Formato numérico inválido';
    return 'Valor inválido';
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

    const raw = this.form.getRawValue();
    this.submitting.set(true);

    // 1. Cliente editando su propio perfil
    if (this.mode === 'editarPerfil') {
      this.usuariosService
        .actualizarMisDatos({
          nombre: raw.firstName.trim(),
          apellido: raw.lastName.trim(),
          telefono: raw.phone.trim() || null,
          fechaDeNacimiento: raw.birthDate
        })
        .subscribe({
          next: (actualizado) => {
            this.submitting.set(false);
            this.dialogRef.close(actualizado);
          },
          error: (err) => {
            this.submitting.set(false);
            this.errorMsg.set(extractErrorMessage(err, 'No se pudo actualizar tu perfil.'));
          }
        });
      return;
    }

    // 2. Administrador editando un usuario existente
    if (this.mode === 'editarAdmin' && this.data?.usuario) {
      this.usuariosService
        .actualizarUsuario(this.data.usuario.idUsuario, {
          nombre: raw.firstName.trim(),
          apellido: raw.lastName.trim(),
          email: raw.email.trim(),
          telefono: raw.phone.trim() || null,
          fechaDeNacimiento: raw.birthDate,
          estado: raw.status,
          nuevaPass: raw.password.trim() || null
        })
        .subscribe({
          next: (actualizado) => {
            this.submitting.set(false);
            this.dialogRef.close(actualizado);
          },
          error: (err) => {
            this.submitting.set(false);
            this.errorMsg.set(extractErrorMessage(err, 'No se pudieron guardar los cambios.'));
          }
        });
      return;
    }

    // 3. Administrador creando un nuevo usuario
    // 3. Administrador creando un nuevo usuario
    const request$: Observable<unknown> =
      raw.role === 'Administrador'
        ? this.authService.registroAdministrador({
            nombre: raw.firstName.trim(),
            apellido: raw.lastName.trim(),
            email: raw.email.trim(),
            pass: raw.password,
            telefono: raw.phone.trim() || null,
            fechaDeNacimiento: raw.birthDate,
            cargo: 'Administrador Operativo'
          })
        : this.authService.registroCliente({
            nombre: raw.firstName.trim(),
            apellido: raw.lastName.trim(),
            email: raw.email.trim(),
            pass: raw.password,
            telefono: raw.phone.trim() || null,
            fechaDeNacimiento: raw.birthDate,
            nombreContactoEmergencia: 'Contacto Emergencia',
            telContactoEmergencia: raw.phone.trim() || '3000000000'
          });

    request$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err: unknown) => {
        this.submitting.set(false);
        this.errorMsg.set(extractErrorMessage(err, 'No se pudo crear el usuario.'));
      }
    });
  }
}