import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ROLES } from '../../constants/api';
import { AuthService } from '../../services/auth';
import { extractErrorMessage } from '../../utils/api';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss'
})
export class LoginPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  rightPanelActive = signal(false);
  loading = signal(false);
  errorMsg = signal<string | null>(null);
  successMsg = signal<string | null>(null);

  // Campos Sign In
  emailText = signal('');
  passwordText = signal('');

  // Campos Sign Up
  regFullName = signal('');
  regEmail = signal('');
  regPassword = signal('');
  regPhone = signal('');
  regBirthDate = signal('2000-01-01');
  regEmergencyName = signal('Contacto Emergencia');
  regEmergencyPhone = signal('3000000000');

  login(): void {
    const email = this.emailText().trim();
    const pass = this.passwordText();
    if (!email || !pass) {
      this.errorMsg.set('Ingresa tu correo y contraseña.');
      return;
    }

    this.loading.set(true);
    this.errorMsg.set(null);

    this.authService.login({ email, pass }).subscribe({
      next: (res) => {
        this.loading.set(false);
        const ruta = res.usuario.rol === ROLES.ADMINISTRADOR ? '/admin' : '/home';
        this.router.navigate([ruta]);
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMsg.set(extractErrorMessage(err, 'Credenciales inválidas.'));
      }
    });
  }

  register(): void {
    const fullName = this.regFullName().trim();
    const email = this.regEmail().trim();
    const pass = this.regPassword();

    if (!fullName || !email || !pass) {
      this.errorMsg.set('Completa todos los campos obligatorios.');
      return;
    }

    const partes = fullName.split(/\s+/);
    const nombre = partes[0];
    const apellido = partes.slice(1).join(' ') || 'Usuario';

    this.loading.set(true);
    this.errorMsg.set(null);

    this.authService
      .registroCliente({
        nombre,
        apellido,
        email,
        pass,
        telefono: this.regPhone().trim() || null,
        fechaDeNacimiento: this.regBirthDate(),
        nombreContactoEmergencia: this.regEmergencyName(),
        telContactoEmergencia: this.regEmergencyPhone()
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.successMsg.set('Cuenta creada exitosamente. Inicia sesión.');
          this.emailText.set(email);
          this.rightPanelActive.set(false);
        },
        error: (err) => {
          this.loading.set(false);
          this.errorMsg.set(extractErrorMessage(err, 'No se pudo crear la cuenta.'));
        }
      });
  }
}