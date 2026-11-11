import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService, mensajeDeError } from '../../core/api.service';
import { AlertaService } from '../../core/alerta.service';
import { Resumen } from '../../core/models';

@Component({
  selector: 'app-inicio',
  imports: [DecimalPipe, RouterLink],
  templateUrl: './inicio.html'
})
export class Inicio implements OnInit {
  private readonly api = inject(ApiService);
  private readonly alertas = inject(AlertaService);

  readonly resumen = signal<Resumen | null>(null);
  readonly cargando = signal(true);

  ngOnInit() {
    this.api.resumen().subscribe({
      next: r => { this.resumen.set(r); this.cargando.set(false); },
      error: (e: HttpErrorResponse) => { this.cargando.set(false); this.alertas.error(mensajeDeError(e)); }
    });
  }

  /** Ancho de la barra en porcentaje, sobre la escala de 0 a 5. */
  ancho(valor: number | null) {
    return valor == null ? 0 : Math.max(2, (valor / 5) * 100);
  }
}
