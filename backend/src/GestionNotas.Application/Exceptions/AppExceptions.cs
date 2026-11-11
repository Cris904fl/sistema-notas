namespace GestionNotas.Application.Exceptions;

/// <summary>El recurso solicitado no existe (HTTP 404).</summary>
public class NoEncontradoException(string mensaje) : Exception(mensaje);

/// <summary>La operación viola una regla de negocio, como eliminar un estudiante con notas (HTTP 409).</summary>
public class ConflictoException(string mensaje) : Exception(mensaje);

/// <summary>Los datos enviados no son válidos (HTTP 400).</summary>
public class ValidacionException(string mensaje) : Exception(mensaje);
