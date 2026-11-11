import { Component, inject } from '@angular/core';
import { AlertaService } from '../core/alerta.service';
import { Icono } from './icono';

@Component({
  selector: 'app-alertas',
  imports: [Icono],
  template: `
    <div class="avisos" aria-live="polite">
      @for (a of servicio.alertas(); track a.id) {
        <div class="aviso" [class.aviso-error]="a.tipo === 'error'" role="status">
          <span class="aviso-icono"><app-icono [nombre]="a.tipo === 'error' ? 'alerta' : 'ok'" /></span>
          <span class="aviso-texto">{{ a.mensaje }}</span>
          <button type="button" class="boton-icono boton-icono-claro" aria-label="Cerrar" (click)="servicio.cerrar(a.id)">
            <app-icono nombre="cerrar" [tamano]="14" />
          </button>
        </div>
      }
    </div>
  `
})
export class Alertas {
  readonly servicio = inject(AlertaService);
}
