using System;
using System.Collections.Generic;

namespace ArquitecturaPipelineDemo
{
    // =========================================================================
    // 1. CONTRATO DE LOS FILTROS (Etapas Independientes)
    // =========================================================================
    public interface IPipelineFilter<T>
    {
        T Execute(T input);
    }

    // =========================================================================
    // 2. IMPLEMENTACIÓN DE FILTROS ESPECÍFICOS
    // =========================================================================

    // Filtro 1: Extracción y Limpieza de datos crudos
    public class LimpiezaFiltro : IPipelineFilter<string>
    {
        public string Execute(string input)
        {
            Console.WriteLine(" -> [Filtro 1: Limpieza] Eliminando espacios y convirtiendo a minúsculas...");
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return input.Trim().ToLower();
        }
    }

    // Filtro 2: Transformación y Enriquecimiento
    public class TransformacionFiltro : IPipelineFilter<string>
    {
        public string Execute(string input)
        {
            Console.WriteLine(" -> [Filtro 2: Transformación] Aplicando reglas de negocio (reemplazo de marcas)...");
            // Ejemplo de transformación de palabras clave
            return input.replace_simulado("urgente", "[ALERTA CRÍTICA]");
        }
    }

    // Filtro 3: Carga y Almacenamiento (Data Sink)
    public class CargaFiltro : IPipelineFilter<string>
    {
        public string Execute(string input)
        {
            Console.WriteLine(" -> [Filtro 3: Carga] Preparando datos para el destino final...");
            return $"[DATOS PROCESADOS Y LISTOS]: {input}";
        }
    }

    // Extensión auxiliar para mantener el código limpio
    public static class StringExtensions
    {
        public static string replace_simulado(this string str, string oldValue, string newValue) =>
            str.Replace(oldValue, newValue);
    }

    // =========================================================================
    // 3. LA TUBERÍA (PIPES / CONNECTOR)
    // =========================================================================
    public class Pipeline<T>
    {
        private readonly List<IPipelineFilter<T>> _filters = new();

        // Conecta una nueva etapa al pipeline (Composición)
        public Pipeline<T> AddFilter(IPipelineFilter<T> filter)
        {
            _filters.Add(filter);
            return this; // Retorna la instancia para permitir encadenamiento fluido (Fluent API)
        }

        // Ejecuta el flujo secuencial de datos a través de los filtros
        public T Execute(T input)
        {
            T currentData = input;
            foreach (var filter in _filters)
            {
                currentData = filter.Execute(currentData);
            }
            return currentData;
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Composition Root)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INICIANDO ARQUITECTURA PIPELINE EN C# ===\n");

            // Fuente de datos (Data Source)
            string datoCrudo = "   Mensaje URGENTE recibido desde la API externa   ";
            Console.WriteLine($"[Fuente de Datos] Entrada original: \"{datoCrudo}\"\n");

            // Configuración del Pipeline uniendo las tuberías y filtros
            var pipeline = new Pipeline<string>()
                .AddFilter(new LimpiezaFiltro())
                .AddFilter(new TransformacionFiltro())
                .AddFilter(new CargaFiltro());

            // Ejecución del flujo completo
            string resultadoFinal = pipeline.Execute(datoCrudo);

            Console.WriteLine($"\n[Destino Final] {resultadoFinal}");
        }
    }
}