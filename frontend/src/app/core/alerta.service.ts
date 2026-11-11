import { Injectable, signal } from '@angular/core';

export type TipoAlerta = 'exito' | 'error' | 'info';
export interface Alerta { id: number; tipo: TipoAlerta; mensaje: string; }

/** Alertas tipo "toast" que desaparecen solas. */
@Injectable({ providedIn: 'root' })
export class AlertaService {
  private contador = 0;
  readonly alertas = signal<Alerta[]>([]);

  exito(mensaje: string) { this.mostrar('exito', mensaje); }
  error(mensaje: string) { this.mostrar('error', mensaje, 6000); }

  cerrar(id: number) {
    this.alertas.update(lista => lista.filter(a => a.id !== id));
  }

  private mostrar(tipo: TipoAlerta, mensaje: string, duracion = 3500) {
    const alerta: Alerta = { id: ++this.contador, tipo, mensaje };
    this.alertas.update(lista => [...lista, alerta]);
    setTimeout(() => this.cerrar(alerta.id), duracion);
  }
}
