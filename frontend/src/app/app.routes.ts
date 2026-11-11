import { Routes } from '@angular/router';
import { Personas } from './features/personas/personas';
import { Notas } from './features/notas/notas';
import { Inicio } from './features/inicio/inicio';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: Inicio, title: 'Resumen · Sistema de Notas' },
  {
    path: 'estudiantes', component: Personas, title: 'Estudiantes · Sistema de Notas',
    data: { recurso: 'estudiantes', titulo: 'Estudiantes', singular: 'Estudiante', conPromedio: true }
  },
  {
    path: 'profesores', component: Personas, title: 'Profesores · Sistema de Notas',
    data: { recurso: 'profesores', titulo: 'Profesores', singular: 'Profesor', conPromedio: false }
  },
  { path: 'notas', component: Notas, title: 'Notas · Sistema de Notas' },
  { path: '**', redirectTo: '' }
];
