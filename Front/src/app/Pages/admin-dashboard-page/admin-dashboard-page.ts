import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { catchError, forkJoin, of } from 'rxjs';
import { AdministracionService } from '../../services/administracion';
import { AuthService } from '../../services/auth';
import { BicicletasService } from '../../services/bike';
import { MantenimientoService } from '../../services/mantenimiento';
import { PrestamosService } from '../../services/prestamos';
import { ReportesService } from '../../services/reportes';
import { EstacionesService } from '../../services/station';
import { UsuariosService } from '../../services/usuarios';
import { formatCop } from '../../utils/api';

interface KpiItem {
  label: string;
  icon: string;
  trend: number;
  value: string;
  area: string;
  line: string;
}

interface ChartData {
  total: number;
  grid: { v: number; y: number }[];
  area: string;
  line: string;
  points: { x: number; y: number; peak: boolean; value: number; label: string }[];
}

@Component({
  selector: 'app-admin-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard-page.html',
  styleUrl: './admin-dashboard-page.scss'
})
export class AdminDashboardPage implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly adminService = inject(AdministracionService);
  private readonly bicicletasService = inject(BicicletasService);
  private readonly estacionesService = inject(EstacionesService);
  private readonly prestamosService = inject(PrestamosService);
  private readonly mantenimientoService = inject(MantenimientoService);
  private readonly reportesService = inject(ReportesService);
  private readonly usuariosService = inject(UsuariosService);

  readonly adminName = computed(() => {
    const u = this.authService.currentUser();
    return u ? `${u.nombre} ${u.apellido}`.trim() : 'Administrador';
  });

  readonly currentPeriod = new Date().toLocaleDateString('es-CO', {
    month: 'long',
    year: 'numeric'
  });

  kpis = signal<KpiItem[]>([
    { label: 'Alquileres totales', icon: 'fa-bicycle', trend: 12, value: '0', area: 'M0,40 L120,40', line: 'M0,30 L120,10' },
    { label: 'Ingresos', icon: 'fa-dollar-sign', trend: 8, value: '$0', area: 'M0,40 L120,40', line: 'M0,32 L120,14' },
    { label: 'Flota disponible', icon: 'fa-check-circle', trend: 4, value: '0/0', area: 'M0,40 L120,40', line: 'M0,25 L120,12' },
    { label: 'Reportes abiertos', icon: 'fa-triangle-exclamation', trend: -2, value: '0', area: 'M0,40 L120,40', line: 'M0,15 L120,28' }
  ]);

  chart = signal<ChartData>({
    total: 0,
    grid: [
      { v: 10, y: 30 },
      { v: 5, y: 115 },
      { v: 0, y: 200 }
    ],
    area: 'M48,200 L552,200',
    line: 'M48,200 L552,200',
    points: []
  });

  fleetTotal = signal(0);
  fleet = signal<{ label: string; color: string; dash: string; offset: number; value: number; pct: number }[]>([]);
  categories = signal<{ name: string; value: number; h: number }[]>([]);
  sedes = signal<{ name: string; pct: number }[]>([]);
  maintenance = signal<{ id: number; bike: string; issue: string; level: string; levelLabel: string; when: string }[]>([]);
  recent = signal<{ id: number; user: string; bike: string; sede: string; time: string; status: string; statusLabel: string; amount: string }[]>([]);
  reports = signal<{ id: number; title: string; who: string; when: string; icon: string }[]>([]);

  ngOnInit(): void {
    const now = new Date();
    const desde = new Date(now.getFullYear(), 0, 1).toISOString();
    const hasta = now.toISOString();

    forkJoin({
      general: this.adminService.generarReporteGeneral({ desde, hasta }).pipe(catchError(() => of(null))),
      bicis: this.bicicletasService.getBicicletas().pipe(catchError(() => of([]))),
      estaciones: this.estacionesService.getEstaciones().pipe(catchError(() => of([]))),
      prestamos: this.prestamosService.getPrestamos().pipe(catchError(() => of([]))),
      mantenimientos: this.mantenimientoService.getMantenimientos().pipe(catchError(() => of([]))),
      reportes: this.reportesService.getReportes().pipe(catchError(() => of([]))),
      usuarios: this.usuariosService.filtrar({ page: 1, pageSize: 200 }).pipe(catchError(() => of(null)))
    }).subscribe(({ general, bicis, estaciones, prestamos, mantenimientos, reportes, usuarios }) => {
      // 1. KPIs
      const disponibles = bicis.filter((b) => b.estado === 'DISPONIBLE').length;
      const enUso = bicis.filter((b) => b.estado === 'EN_PRESTAMO').length;
      const enTaller = bicis.filter((b) => b.estado === 'EN_MANTENIMIENTO').length;

      const totalPrestamos = general?.totalPrestamos ?? prestamos.length;
      const ingresosTotales =
        general?.ingresosTotales ?? prestamos.reduce((acc, p) => acc + (Number(p.costoTotal) || 0), 0);
      const reportesAbiertos =
        general?.reportesAbiertos ??
        reportes.filter((r) => r.estado !== 'RESUELTO' && r.estado !== 'CERRADO').length;

      this.kpis.update((prev) => [
        { ...prev[0], value: String(totalPrestamos) },
        { ...prev[1], value: formatCop(ingresosTotales) },
        { ...prev[2], value: `${disponibles}/${bicis.length}` },
        { ...prev[3], value: String(reportesAbiertos) }
      ]);

      // 2. Gráfica de alquileres últimos 7 días
      this.buildWeeklyChart(prestamos);

      // 3. Donut de flota
      const totalBicis = bicis.length;
      this.fleetTotal.set(totalBicis);
      const circ = 2 * Math.PI * 52;
      const segmentos = [
        { label: 'Disponibles', color: '#0fd1e6', value: disponibles },
        { label: 'En uso', color: '#3b82f6', value: enUso },
        { label: 'Mantenimiento', color: '#f59e0b', value: enTaller }
      ];
      let acumulado = 0;
      this.fleet.set(
        segmentos.map((s) => {
          const pct = totalBicis > 0 ? Math.round((s.value / totalBicis) * 100) : 0;
          const len = totalBicis > 0 ? (s.value / totalBicis) * circ : 0;
          const item = {
            ...s,
            pct,
            dash: `${len} ${circ - len}`,
            offset: -acumulado
          };
          acumulado += len;
          return item;
        })
      );

      // 4. Alquileres del mes por categoría
      const biciTipoMap = new Map(bicis.map((b) => [b.idBicicleta, b.tipo]));
      const prestamosMes = prestamos.filter((p) => {
        const d = new Date(p.fechaHoraInicio);
        return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
      });
      const tipos = ['Urbana', 'Montaña', 'Eléctrica', 'Ruta'];
      const conteosCat = tipos.map((name) => {
        const value = prestamosMes.filter(
          (p) => (biciTipoMap.get(p.idBicicleta) ?? '').toLowerCase() === name.toLowerCase()
        ).length;
        return { name, value };
      });
      const maxCat = Math.max(1, ...conteosCat.map((c) => c.value));
      this.categories.set(
        conteosCat.map((c) => ({
          name: c.name,
          value: c.value,
          h: c.value > 0 ? Math.max(12, Math.round((c.value / maxCat) * 100)) : 4
        }))
      );

      // 5. Ocupación por sede
      this.sedes.set(
        estaciones.map((e) => ({
          name: e.nombre,
          pct: e.capacidad > 0 ? Math.min(100, Math.round((e.bicicletasActuales / e.capacidad) * 100)) : 0
        }))
      );

      // 6. Mantenimientos pendientes
      this.maintenance.set(
        mantenimientos
          .filter((m) => m.estado !== 'FINALIZADO')
          .slice(0, 4)
          .map((m) => ({
            id: m.idMantenimiento,
            bike: m.serial ?? `Bici #${m.idBicicleta}`,
            issue: m.descripcion ?? m.tipo,
            level: m.estado === 'EN_EJECUCION' || m.estado === 'EN_CURSO' ? 'high' : 'med',
            levelLabel: m.estado === 'EN_EJECUCION' || m.estado === 'EN_CURSO' ? 'En curso' : 'Programado',
            when: new Date(m.fechaProgramada).toLocaleDateString('es-CO')
          }))
      );

      // 7. Últimos alquileres (con nombre real del usuario)
      const userMap = new Map(
        (usuarios?.items ?? []).map((u) => [u.idUsuario, `${u.nombre} ${u.apellido}`.trim()])
      );
      this.recent.set(
        prestamos.slice(0, 5).map((p) => ({
          id: p.idPrestamo,
          user: userMap.get(p.idUsuario) || `Usuario #${p.idUsuario}`,
          bike: p.serial,
          sede: p.nombreEstacionOrigen,
          time: p.tiempoUso ? `${p.tiempoUso} min` : 'En curso',
          status: p.estado === 'FINALIZADO' ? 'ok' : 'run',
          statusLabel: p.estado,
          amount: formatCop(p.costoTotal ?? 0)
        }))
      );

      // 8. Reportes recientes
      this.reports.set(
        reportes.slice(0, 4).map((r) => ({
          id: r.idReporte,
          title: r.tipo,
          who: r.nombreUsuario ?? userMap.get(r.idUsuario) ?? `Usuario #${r.idUsuario}`,
          when: new Date(r.fechaHora).toLocaleDateString('es-CO'),
          icon: 'fa-circle-exclamation'
        }))
      );
    });
  }

  private buildWeeklyChart(prestamos: { fechaHoraInicio: string }[]): void {
    const diasNombres = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];
    const days: { date: Date; label: string; value: number }[] = [];

    for (let i = 6; i >= 0; i--) {
      const d = new Date();
      d.setDate(d.getDate() - i);
      const count = prestamos.filter((p) => {
        const pd = new Date(p.fechaHoraInicio);
        return (
          pd.getFullYear() === d.getFullYear() &&
          pd.getMonth() === d.getMonth() &&
          pd.getDate() === d.getDate()
        );
      }).length;

      days.push({
        date: d,
        label: `${diasNombres[d.getDay()]} ${d.getDate()}`,
        value: count
      });
    }

    const totalSemana = days.reduce((acc, d) => acc + d.value, 0);
    const maxRaw = Math.max(...days.map((d) => d.value), 2);
    const maxVal = maxRaw % 2 === 0 ? maxRaw : maxRaw + 1;

    const xStart = 48;
    const xEnd = 552;
    const stepX = (xEnd - xStart) / 6;

    const points = days.map((d, idx) => {
      const x = Math.round(xStart + idx * stepX);
      const y = Math.round(200 - (d.value / maxVal) * 170);
      return {
        x,
        y,
        value: d.value,
        label: d.label,
        peak: d.value === maxRaw && d.value > 0
      };
    });

    const line = 'M' + points.map((p) => `${p.x},${p.y}`).join(' L');
    const area = `${line} L${points[points.length - 1].x},200 L${points[0].x},200 Z`;

    this.chart.set({
      total: totalSemana,
      grid: [
        { v: maxVal, y: 30 },
        { v: Math.round(maxVal / 2), y: 115 },
        { v: 0, y: 200 }
      ],
      line,
      area,
      points
    });
  }
}