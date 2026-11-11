import { Injectable, effect, signal } from '@angular/core';

export type Tema = 'claro' | 'oscuro';
const CLAVE = 'tema';

/** Maneja el tema claro/oscuro: respeta la preferencia del sistema y recuerda la elección del usuario. */
@Injectable({ providedIn: 'root' })
export class TemaService {
  readonly tema = signal<Tema>(this.temaInicial());

  constructor() {
    // Cada vez que cambia el tema, se aplica al documento y se guarda.
    effect(() => {
      const tema = this.tema();
      document.documentElement.setAttribute('data-tema', tema);
      try { localStorage.setItem(CLAVE, tema); } catch { /* almacenamiento no disponible */ }
    });
  }

  alternar() {
    this.tema.update(t => (t === 'claro' ? 'oscuro' : 'claro'));
  }

  private temaInicial(): Tema {
    try {
      const guardado = localStorage.getItem(CLAVE);
      if (guardado === 'claro' || guardado === 'oscuro') return guardado;
    } catch { /* sin acceso a localStorage */ }
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'oscuro' : 'claro';
  }
}
