
using System;
using System.Collections.Generic;

/// <summary>
/// Contenedor de dependencias
/// </summary>
public static class AppContainer
{
    // Diccionario con los servicios registrados de la aplicación
    private static Dictionary<Type, Func<object>> _servicesRegistered = new Dictionary<Type, Func<object>>();

    // Diccionario con los servicios instanciados (singleton) de la aplicación
    private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

    /// <summary>
    /// Registra un servicio
    /// </summary>
    /// <typeparam name="TInterface">Tipo de objeto a registrar</typeparam>
    /// <param name="function">Función para instanciar el objeto cuando se solicite por primera vez</param>
    public static void Register<T>(Func<object> function)
    {
        _servicesRegistered.Add(typeof(T), function);
    }

    /// <summary>
    /// Devuelve un servicio existente o lo instancia si no lo tiene
    /// </summary>
    /// <typeparam name="T">Tipo de objeto a devolver</typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T Get<T>()
    {
        // Obtenemos el Type a devolver
        Type type = typeof(T);

        // Buscamos en el listado de objetos instanciados
        if (_services.TryGetValue(type, out var service))
            return (T)service;

        // Si tenemos registrado el Type la instanciamos y la registramos
        if (_servicesRegistered.TryGetValue(type, out var serviceRegistered))
        {
            var newService = serviceRegistered();
            _services.Add(type, newService);
            return (T)newService;
        }

        return default(T);
    }   
}
