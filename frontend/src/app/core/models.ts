export interface Paginado<T> {
  elementos: T[];
  pagina: number;
  tamano: number;
  totalElementos: number;
  totalPaginas: number;
}

export interface ConsultaPaginada {
  pagina: number;
  tamano: number;
  buscar?: string;
  ordenarPor?: string;
  descendente?: boolean;
}

/** Estudiante o profesor: comparten estructura (id y nombre). */
export interface Persona {
  id: number;
  nombre: string;
  cantidadNotas: number;
  promedio?: number | null;
}

export interface Nota {
  id: number;
  nombre: string;
  valor: number;
  idEstudiante: number;
  estudiante: string;
  idProfesor: number;
  profesor: string;
  /** Versión de la fila (Base64), para el control de concurrencia. */
  version: string;
}

export interface PersonaGuardar { nombre: string; }

export interface NotaGuardar {
  nombre: string;
  idEstudiante: number;
  idProfesor: number;
  valor: number;
  version?: string;
}

export interface Indicadores {
  totalEstudiantes: number;
  totalProfesores: number;
  totalNotas: number;
  promedioGeneral: number | null;
  porcentajeAprobacion: number | null;
  notaMaxima: number | null;
  notaMinima: number | null;
}

export interface Promedio {
  id: number;
  nombre: string;
  cantidadNotas: number;
  promedio: number | null;
}

export interface Resumen {
  indicadores: Indicadores;
  mejoresEstudiantes: Promedio[];
  promedioPorProfesor: Promedio[];
}
