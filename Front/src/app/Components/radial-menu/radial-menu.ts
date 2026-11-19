import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-radial-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './radial-menu.html',
  styleUrl: './radial-menu.scss'
})
export class RadialMenuComponent {
  open = signal(false);

  readonly links = [
    { label: 'Instagram', icon: 'fa-instagram', url: 'https://instagram.com', angle: 0 },
    { label: 'Facebook', icon: 'fa-facebook-f', url: 'https://facebook.com', angle: -60 },
    { label: 'X', icon: 'fa-x-twitter', url: 'https://x.com', angle: -120 },
    { label: 'WhatsApp', icon: 'fa-whatsapp', url: 'https://whatsapp.com', angle: -180 }
  ];

  toggle(): void {
    this.open.update((v) => !v);
  }
}