import { Injectable, signal } from '@angular/core';

interface SolicitudConfirmacion { titulo: string; mensaje: string; resolver: (ok: boolean) => void; }

/** Diálogo de confirmación propio (en lugar de window.confirm). */
@Injectable({ providedIn: 'root' })
export class ConfirmacionService {
  readonly solicitud = signal<SolicitudConfirmacion | null>(null);

  confirmar(titulo: string, mensaje: string): Promise<boolean> {
    return new Promise(resolver => this.solicitud.set({ titulo, mensaje, resolver }));
  }

  responder(ok: boolean) {
    this.solicitud()?.resolver(ok);
    this.solicitud.set(null);
  }
}
