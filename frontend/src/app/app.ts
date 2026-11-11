import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Alertas } from './shared/alertas';
import { Confirmacion } from './shared/confirmacion';
import { Icono } from './shared/icono';
import { TemaService } from './core/tema.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, Alertas, Confirmacion, Icono],
  template: `
    <header class="encabezado">
      <div class="encabezado-contenido">
        <a class="marca" routerLink="/">
          <span class="marca-logo">SN</span>
          <span>Sistema de Notas</span>
        </a>
        <nav class="pestanas" aria-label="Menú principal">
          <a routerLink="/estudiantes" routerLinkActive="activa">Estudiantes</a>
          <a routerLink="/profesores" routerLinkActive="activa">Profesores</a>
          <a routerLink="/notas" routerLinkActive="activa">Notas</a>
        </nav>
        <button type="button" class="boton-icono boton-tema"
                [attr.aria-label]="temaServicio.tema() === 'oscuro' ? 'Cambiar a tema claro' : 'Cambiar a tema oscuro'"
                [title]="temaServicio.tema() === 'oscuro' ? 'Tema claro' : 'Tema oscuro'"
                (click)="temaServicio.alternar()">
          <app-icono [nombre]="temaServicio.tema() === 'oscuro' ? 'sol' : 'luna'" [tamano]="18" />
        </button>
      </div>
    </header>

    <main class="contenedor">
      <router-outlet />
    </main>

    <app-alertas />
    <app-confirmacion />
  `
})
export class App {
  readonly temaServicio = inject(TemaService);
}
