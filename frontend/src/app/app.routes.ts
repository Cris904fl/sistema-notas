import { Routes } from '@angular/router';
import { Personas } from './features/personas/personas';
import { Notas } from './features/notas/notas';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'estudiantes' },
  {
    path: 'estudiantes', component: Personas, title: 'Estudiantes · Gestión de Notas',
    data: { recurso: 'estudiantes', titulo: 'Estudiantes', singular: 'Estudiante', conPromedio: true }
  },
  {
    path: 'profesores', component: Personas, title: 'Profesores · Gestión de Notas',
    data: { recurso: 'profesores', titulo: 'Profesores', singular: 'Profesor', conPromedio: false }
  },
  { path: 'notas', component: Notas, title: 'Notas · Gestión de Notas' },
  { path: '**', redirectTo: 'estudiantes' }
];
