import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from './config';
import { ConsultaPaginada, Paginado } from './models';

export type Recurso = 'estudiantes' | 'profesores' | 'notas';

/** Servicio genérico de CRUD contra la API REST. */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);

  listar<T>(recurso: Recurso, consulta: ConsultaPaginada, filtros: Record<string, number | null> = {}): Observable<Paginado<T>> {
    let params = new HttpParams()
      .set('pagina', consulta.pagina)
      .set('tamano', consulta.tamano);
    if (consulta.buscar?.trim()) params = params.set('buscar', consulta.buscar.trim());
    if (consulta.ordenarPor) params = params.set('ordenarPor', consulta.ordenarPor).set('descendente', !!consulta.descendente);
    for (const [clave, valor] of Object.entries(filtros)) {
      if (valor) params = params.set(clave, valor);
    }
    return this.http.get<Paginado<T>>(`${API_URL}/${recurso}`, { params });
  }

  crear<T>(recurso: Recurso, cuerpo: unknown): Observable<T> {
    return this.http.post<T>(`${API_URL}/${recurso}`, cuerpo);
  }

  actualizar<T>(recurso: Recurso, id: number, cuerpo: unknown): Observable<T> {
    return this.http.put<T>(`${API_URL}/${recurso}/${id}`, cuerpo);
  }

  eliminar(recurso: Recurso, id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${recurso}/${id}`);
  }
}

/** Convierte la respuesta de error de la API (ProblemDetails) en un mensaje legible. */
export function mensajeDeError(error: HttpErrorResponse): string {
  if (error.status === 0) return 'No fue posible conectar con el servidor. Verifique que la API esté en ejecución.';
  const cuerpo = error.error;
  if (cuerpo?.errors) {
    // Errores de validación de ASP.NET: { errors: { Campo: ["mensaje"] } }
    return Object.values(cuerpo.errors as Record<string, string[]>).flat().join(' ');
  }
  return cuerpo?.detail ?? cuerpo?.title ?? 'Ocurrió un error inesperado.';
}
