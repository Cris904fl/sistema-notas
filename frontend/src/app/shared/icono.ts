import { Component, input } from '@angular/core';

/** Íconos SVG en línea (trazo), para no depender de librerías externas. */
@Component({
  selector: 'app-icono',
  template: `
    <svg [attr.width]="tamano()" [attr.height]="tamano()" viewBox="0 0 24 24" fill="none" stroke="currentColor"
         stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
      @switch (nombre()) {
        @case ('mas') { <path d="M12 5v14M5 12h14"/> }
        @case ('buscar') { <circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/> }
        @case ('editar') { <path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z"/> }
        @case ('eliminar') { <path d="M3 6h18"/><path d="M8 6V4h8v2"/><path d="M19 6l-1 14H6L5 6"/> }
        @case ('cerrar') { <path d="M18 6 6 18M6 6l12 12"/> }
        @case ('anterior') { <path d="m15 18-6-6 6-6"/> }
        @case ('siguiente') { <path d="m9 18 6-6-6-6"/> }
        @case ('ok') { <path d="M20 6 9 17l-5-5"/> }
        @case ('sol') { <circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/> }
        @case ('luna') { <path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8Z"/> }
        @case ('alerta') { <circle cx="12" cy="12" r="9"/><path d="M12 8v4M12 16h.01"/> }
      }
    </svg>
  `,
  styles: [':host { display: inline-flex; }']
})
export class Icono {
  readonly nombre = input.required<string>();
  readonly tamano = input(16);
}
