import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ApiService, Recurso, mensajeDeError } from '../../core/api.service';
import { AlertaService } from '../../core/alerta.service';
import { ConfirmacionService } from '../../core/confirmacion.service';
import { ConsultaPaginada, Paginado, Persona } from '../../core/models';
import { Paginacion } from '../../shared/paginacion';
import { Icono } from '../../shared/icono';

/** Datos que llegan desde la ruta para reutilizar la misma pantalla en Estudiantes y Profesores. */
interface ConfigPersonas { recurso: Recurso; titulo: string; singular: string; conPromedio: boolean; }

@Component({
  selector: 'app-personas',
  imports: [ReactiveFormsModule, DecimalPipe, Paginacion, Icono],
  templateUrl: './personas.html'
})
export class Personas implements OnInit {
  private readonly api = inject(ApiService);
  private readonly alertas = inject(AlertaService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);

  readonly config = inject(ActivatedRoute).snapshot.data as ConfigPersonas;

  readonly datos = signal<Paginado<Persona> | null>(null);
  readonly cargando = signal(false);
  readonly guardando = signal(false);
  readonly consulta = signal<ConsultaPaginada>({ pagina: 1, tamano: 5, ordenarPor: 'nombre' });

  readonly mostrarFormulario = signal(false);
  readonly editando = signal<Persona | null>(null);

  readonly formulario = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]]
  });

  private readonly busqueda$ = new Subject<string>();

  ngOnInit() {
    // Búsqueda con debounce: consulta la API cuando el usuario deja de escribir.
    this.busqueda$
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(texto => {
        this.consulta.update(c => ({ ...c, buscar: texto, pagina: 1 }));
        this.cargar();
      });
    this.cargar();
  }

  buscar(texto: string) { this.busqueda$.next(texto); }

  cargar() {
    this.cargando.set(true);
    this.api.listar<Persona>(this.config.recurso, this.consulta()).subscribe({
      next: datos => { this.datos.set(datos); this.cargando.set(false); },
      error: (e: HttpErrorResponse) => { this.cargando.set(false); this.alertas.error(mensajeDeError(e)); }
    });
  }

  cambiarPagina(pagina: number) {
    this.consulta.update(c => ({ ...c, pagina }));
    this.cargar();
  }

  ordenar(campo: string) {
    this.consulta.update(c => ({
      ...c, ordenarPor: campo, descendente: c.ordenarPor === campo ? !c.descendente : false, pagina: 1
    }));
    this.cargar();
  }

  icono(campo: string) {
    const c = this.consulta();
    return c.ordenarPor !== campo ? '↕' : c.descendente ? '↓' : '↑';
  }

  nuevo() {
    this.editando.set(null);
    this.formulario.reset({ nombre: '' });
    this.mostrarFormulario.set(true);
  }

  editar(persona: Persona) {
    this.editando.set(persona);
    this.formulario.reset({ nombre: persona.nombre });
    this.mostrarFormulario.set(true);
  }

  cerrarFormulario() { this.mostrarFormulario.set(false); }

  guardar() {
    if (this.formulario.invalid) { this.formulario.markAllAsTouched(); return; }
    const cuerpo = { nombre: this.formulario.getRawValue().nombre.trim() };
    const actual = this.editando();
    const peticion = actual
      ? this.api.actualizar<Persona>(this.config.recurso, actual.id, cuerpo)
      : this.api.crear<Persona>(this.config.recurso, cuerpo);

    this.guardando.set(true);
    peticion.subscribe({
      next: () => {
        this.guardando.set(false);
        this.mostrarFormulario.set(false);
        this.alertas.exito(`${this.config.singular} ${actual ? 'actualizado' : 'creado'} correctamente.`);
        this.cargar();
      },
      error: (e: HttpErrorResponse) => { this.guardando.set(false); this.alertas.error(mensajeDeError(e)); }
    });
  }

  async eliminar(persona: Persona) {
    const ok = await this.confirmacion.confirmar(
      `Eliminar ${this.config.singular.toLowerCase()}`,
      `¿Seguro que desea eliminar a "${persona.nombre}"? Esta acción no se puede deshacer.`);
    if (!ok) return;

    this.api.eliminar(this.config.recurso, persona.id).subscribe({
      next: () => {
        this.alertas.exito(`${this.config.singular} eliminado correctamente.`);
        // Si se borró el último registro de la página, se retrocede una página.
        const d = this.datos();
        if (d && d.elementos.length === 1 && d.pagina > 1) this.consulta.update(c => ({ ...c, pagina: c.pagina - 1 }));
        this.cargar();
      },
      error: (e: HttpErrorResponse) => this.alertas.error(mensajeDeError(e))
    });
  }
}
