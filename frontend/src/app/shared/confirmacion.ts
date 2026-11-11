import { Component, inject } from '@angular/core';
import { ConfirmacionService } from '../core/confirmacion.service';

@Component({
  selector: 'app-confirmacion',
  template: `
    @if (servicio.solicitud(); as s) {
      <div class="capa" (click)="servicio.responder(false)">
        <div class="dialogo dialogo-pequeno" role="alertdialog" aria-modal="true" (click)="$event.stopPropagation()">
          <div class="dialogo-cuerpo">
            <h2 class="dialogo-titulo">{{ s.titulo }}</h2>
            <p class="texto-tenue">{{ s.mensaje }}</p>
          </div>
          <div class="dialogo-pie">
            <button type="button" class="boton" (click)="servicio.responder(false)">Cancelar</button>
            <button type="button" class="boton boton-peligro" (click)="servicio.responder(true)">Eliminar</button>
          </div>
        </div>
      </div>
    }
  `
})
export class Confirmacion {
  readonly servicio = inject(ConfirmacionService);
}
