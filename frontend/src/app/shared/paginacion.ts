import { Component, computed, input, output } from '@angular/core';
import { Icono } from './icono';

@Component({
  selector: 'app-paginacion',
  imports: [Icono],
  template: `
    <div class="paginacion">
      <span class="texto-tenue">{{ desde() }}–{{ hasta() }} de {{ totalElementos() }}</span>
      <div class="paginacion-controles">
        <button type="button" class="boton-icono" aria-label="Página anterior"
                [disabled]="pagina() <= 1" (click)="cambiar.emit(pagina() - 1)">
          <app-icono nombre="anterior" />
        </button>
        @for (p of paginas(); track p) {
          <button type="button" class="pagina-numero" [class.actual]="p === pagina()"
                  [attr.aria-current]="p === pagina() ? 'page' : null" (click)="cambiar.emit(p)">{{ p }}</button>
        }
        <button type="button" class="boton-icono" aria-label="Página siguiente"
                [disabled]="pagina() >= totalPaginas()" (click)="cambiar.emit(pagina() + 1)">
          <app-icono nombre="siguiente" />
        </button>
      </div>
    </div>
  `
})
export class Paginacion {
  readonly pagina = input.required<number>();
  readonly tamano = input.required<number>();
  readonly totalPaginas = input.required<number>();
  readonly totalElementos = input.required<number>();
  readonly cambiar = output<number>();

  readonly desde = computed(() => this.totalElementos() === 0 ? 0 : (this.pagina() - 1) * this.tamano() + 1);
  readonly hasta = computed(() => Math.min(this.pagina() * this.tamano(), this.totalElementos()));

  readonly paginas = computed(() => {
    const total = this.totalPaginas();
    const inicio = Math.max(1, Math.min(this.pagina() - 2, total - 4));
    const fin = Math.min(total, inicio + 4);
    return Array.from({ length: Math.max(0, fin - inicio + 1) }, (_, i) => inicio + i);
  });
}
