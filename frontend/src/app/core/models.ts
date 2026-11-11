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
}

export interface PersonaGuardar { nombre: string; }

export interface NotaGuardar {
  nombre: string;
  idEstudiante: number;
  idProfesor: number;
  valor: number;
}
