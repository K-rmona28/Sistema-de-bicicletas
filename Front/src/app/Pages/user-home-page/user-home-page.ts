import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RadialMenuComponent } from '../../components/radial-menu/radial-menu';
import { SedesSectionComponent } from '../../components/sedes-section/sedes-section';
import { GetEstacionResponse } from '../../interfaces';
import { BicicletasService } from '../../services/bike';
import { EstacionesService } from '../../services/station';

@Component({
  selector: 'app-user-home-page',
  standalone: true,
  imports: [CommonModule, RouterLink, SedesSectionComponent, RadialMenuComponent],
  templateUrl: './user-home-page.html',
  styleUrl: './user-home-page.scss'
})
export class UserHomePage implements OnInit {
  private readonly estacionesService = inject(EstacionesService);
  private readonly bicicletasService = inject(BicicletasService);

  sedes: GetEstacionResponse[] = [];
  disponiblesAhora = signal(0);

  readonly dust = Array.from({ length: 18 }, (_, i) => ({
    x: (i * 19 + 7) % 100,
    y: (i * 29 + 13) % 100,
    s: 2 + (i % 4),
    d: 6 + (i % 7),
    delay: -(i % 5)
  }));

  ngOnInit(): void {
    this.estacionesService.getEstaciones().subscribe((res) => {
      this.sedes = res;
      const totalDisp = res.reduce((acc, s) => acc + s.bicicletasDisponibles, 0);
      this.disponiblesAhora.set(totalDisp);
    });

    this.bicicletasService.getBicicletas().subscribe((bicis) => {
      const disp = bicis.filter((b) => b.estado === 'DISPONIBLE').length;
      if (disp > 0) this.disponiblesAhora.set(disp);
    });
  }

  goToSedes(): void {
    document.getElementById('sedes')?.scrollIntoView({ behavior: 'smooth' });
  }
}