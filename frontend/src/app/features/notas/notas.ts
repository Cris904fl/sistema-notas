import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { Subject, debounceTime, distinctUntilChanged, forkJoin } from 'rxjs';
import { ApiService, mensajeDeError } from '../../core/api.service';
import { AlertaService } from '../../core/alerta.service';
import { ConfirmacionService } from '../../core/confirmacion.service';
import { ConsultaPaginada, Nota, NotaGuardar, Paginado, Persona } from '../../core/models';
import { Paginacion } from '../../shared/paginacion';
import { Icono } from '../../shared/icono';

@Component({
  selector: 'app-notas',
  imports: [ReactiveFormsModule, DecimalPipe, Paginacion, Icono],
  templateUrl: './notas.html'
})
export class Notas implements OnInit {
  private readonly api = inject(ApiService);
  private readonly alertas = inject(AlertaService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);

  readonly datos = signal<Paginado<Nota> | null>(null);
  readonly cargando = signal(false);
  readonly guardando = signal(false);
  readonly consulta = signal<ConsultaPaginada>({ pagina: 1, tamano: 5 });
  readonly filtroEstudiante = signal<number | null>(null);
  readonly filtroProfesor = signal<number | null>(null);

  readonly estudiantes = signal<Persona[]>([]);
  readonly profesores = signal<Persona[]>([]);

  readonly mostrarFormulario = signal(false);
  readonly editando = signal<Nota | null>(null);

  readonly formulario = this.fb.group({
    nombre: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]),
    idEstudiante: this.fb.control<number | null>(null, Validators.required),
    idProfesor: this.fb.control<number | null>(null, Validators.required),
    valor: this.fb.control<number | null>(null, [Validators.required, Validators.min(0), Validators.max(5)])
  });

  private readonly busqueda$ = new Subject<string>();

  ngOnInit() {
    this.busqueda$
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(texto => {
        this.consulta.update(c => ({ ...c, buscar: texto, pagina: 1 }));
        this.cargar();
      });
    this.cargarCatalogos();
    this.cargar();
  }

  /** Estudiantes y profesores para los filtros y los selectores del formulario. */
  private cargarCatalogos() {
    const todos: ConsultaPaginada = { pagina: 1, tamano: 50, ordenarPor: 'nombre' };
    forkJoin({
      estudiantes: this.api.listar<Persona>('estudiantes', todos),
      profesores: this.api.listar<Persona>('profesores', todos)
    }).subscribe({
      next: r => { this.estudiantes.set(r.estudiantes.elementos); this.profesores.set(r.profesores.elementos); },
      error: (e: HttpErrorResponse) => this.alertas.error(mensajeDeError(e))
    });
  }

  buscar(texto: string) { this.busqueda$.next(texto); }

  filtrar(tipo: 'estudiante' | 'profesor', valor: string) {
    const id = valor ? Number(valor) : null;
    (tipo === 'estudiante' ? this.filtroEstudiante : this.filtroProfesor).set(id);
    this.consulta.update(c => ({ ...c, pagina: 1 }));
    this.cargar();
  }

  cargar() {
    this.cargando.set(true);
    this.api.listar<Nota>('notas', this.consulta(), {
      idEstudiante: this.filtroEstudiante(),
      idProfesor: this.filtroProfesor()
    }).subscribe({
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

  nueva() {
    this.editando.set(null);
    this.formulario.reset({ nombre: '', idEstudiante: null, idProfesor: null, valor: null });
    this.mostrarFormulario.set(true);
  }

  editar(nota: Nota) {
    this.editando.set(nota);
    this.formulario.reset({
      nombre: nota.nombre, idEstudiante: nota.idEstudiante, idProfesor: nota.idProfesor, valor: nota.valor
    });
    this.mostrarFormulario.set(true);
  }

  cerrarFormulario() { this.mostrarFormulario.set(false); }

  guardar() {
    if (this.formulario.invalid) { this.formulario.markAllAsTouched(); return; }
    const v = this.formulario.getRawValue();
    const cuerpo: NotaGuardar = {
      nombre: v.nombre.trim(),
      idEstudiante: Number(v.idEstudiante),
      idProfesor: Number(v.idProfesor),
      valor: Number(v.valor)
    };
    const actual = this.editando();
    const peticion = actual
      ? this.api.actualizar<Nota>('notas', actual.id, cuerpo)
      : this.api.crear<Nota>('notas', cuerpo);

    this.guardando.set(true);
    peticion.subscribe({
      next: () => {
        this.guardando.set(false);
        this.mostrarFormulario.set(false);
        this.alertas.exito(`Nota ${actual ? 'actualizada' : 'creada'} correctamente.`);
        this.cargar();
      },
      error: (e: HttpErrorResponse) => { this.guardando.set(false); this.alertas.error(mensajeDeError(e)); }
    });
  }

  async eliminar(nota: Nota) {
    const ok = await this.confirmacion.confirmar('Eliminar nota',
      `¿Seguro que desea eliminar la nota "${nota.nombre}" de ${nota.estudiante}? Esta acción no se puede deshacer.`);
    if (!ok) return;

    this.api.eliminar('notas', nota.id).subscribe({
      next: () => {
        this.alertas.exito('Nota eliminada correctamente.');
        const d = this.datos();
        if (d && d.elementos.length === 1 && d.pagina > 1) this.consulta.update(c => ({ ...c, pagina: c.pagina - 1 }));
        this.cargar();
      },
      error: (e: HttpErrorResponse) => this.alertas.error(mensajeDeError(e))
    });
  }

  invalido(campo: 'nombre' | 'idEstudiante' | 'idProfesor' | 'valor') {
    const c = this.formulario.controls[campo];
    return c.touched && c.invalid;
  }
}
