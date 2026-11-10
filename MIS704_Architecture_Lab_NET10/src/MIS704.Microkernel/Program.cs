using System;
using System.Collections.Generic;

namespace MicrokernelArchitectureDemo
{
    // =========================================================================
    // 1. CONTRATO / INTERFAZ COMPARTIDA (Parte del Núcleo)
    // Define la especificación estricta que todo plug-in debe cumplir.
    // =========================================================================
    public interface IPlugin
    {
        string Nombre { get; }
        string Version { get; }
        void Ejecutar(Dictionary<string, object> contexto);
    }

    // =========================================================================
    // 2. MICROKERNEL / GESTOR (Core Estable)
    // Administra el registro, ciclo de vida y ejecución de los plug-ins.
    // =========================================================================
    public class MicrokernelCore
    {
        private readonly Dictionary<string, IPlugin> _plugins = new(StringComparer.OrdinalIgnoreCase);

        public void RegistrarPlugin(IPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));

            if (_plugins.ContainsKey(plugin.Nombre))
            {
                Console.WriteLine($"[CORE] El plug-in '{plugin.Nombre}' ya está registrado.");
                return;
            }

            _plugins[plugin.Nombre] = plugin;
            Console.WriteLine($"[CORE] Plug-in registrado con éxito: {plugin.Nombre} (v{plugin.Version})");
        }

        public void EjecutarPlugin(string nombrePlugin, Dictionary<string, object> contexto)
        {
            if (_plugins.TryGetValue(nombrePlugin, out var plugin))
            {
                Console.WriteLine($"[CORE] Ejecutando plug-in: {nombrePlugin}...");
                plugin.Ejecutar(contexto);
            }
            else
            {
                Console.WriteLine($"[CORE] Error: El plug-in '{nombrePlugin}' no está registrado en el microkernel.");
            }
        }

        public IEnumerable<string> ObtenerPluginsRegistrados()
        {
            return _plugins.Keys;
        }
    }

    // =========================================================================
    // 3. PLUG-INS (Módulos Externos que extienden el sistema sin tocar el Core)
    // =========================================================================

    // Plug-in A: Analizador de Texto / Estadísticas
    public class AnalizadorTextoPlugin : IPlugin
    {
        public string Nombre => "AnalizadorTexto";
        public string Version => "1.0.0";

        public void Ejecutar(Dictionary<string, object> contexto)
        {
            if (contexto.TryGetValue("Texto", out var valor) && valor is string texto)
            {
                int palabras = texto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                Console.WriteLine($"  -> [Plug-in AnalizadorTexto] El texto tiene {palabras} palabras y {texto.Length} caracteres.");
            }
            else
            {
                Console.WriteLine("  -> [Plug-in AnalizadorTexto] Error: No se encontró el parámetro 'Texto' en el contexto.");
            }
        }
    }

    // Plug-in B: Generador de Reportes de Log
    public class ReporteLogPlugin : IPlugin
    {
        public string Nombre => "ReporteLog";
        public string Version => "1.2.0";

        public void Ejecutar(Dictionary<string, object> contexto)
        {
            Console.WriteLine($"  -> [Plug-in ReporteLog] Generando reporte de auditoría...");
            Console.WriteLine($"  -> [Plug-in ReporteLog] Timestamp de ejecución: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Composition Root / Arranque)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INICIANDO ARQUITECTURA MICROKERNEL EN C# ===\n");

            // 1. Inicializar el núcleo estable (Microkernel)
            var kernel = new MicrokernelCore();

            // 2. Registrar los plug-ins (en un entorno de producción avanzado, 
            // esto se carga dinámicamente escaneando archivos DLL con Assembly.LoadFrom)
            IPlugin pluginTexto = new AnalizadorTextoPlugin();
            IPlugin pluginReporte = new ReporteLogPlugin();

            kernel.RegistrarPlugin(pluginTexto);
            kernel.RegistrarPlugin(pluginReporte);

            Console.WriteLine("\n--- ESTADO DEL NÚCLEO ---");
            Console.WriteLine($"Plug-ins activos: {string.Join(", ", kernel.ObtenerPluginsRegistrados())}\n");

            // 3. Ejecutar operaciones mediante los plug-ins a través del núcleo
            var contextoCompartido = new Dictionary<string, object>
            {
                ["Texto"] = "Arquitectura Microkernel con soporte de plug-ins modulares en C#"
            };

            kernel.EjecutarPlugin("AnalizadorTexto", contextoCompartido);
            Console.WriteLine();
            kernel.EjecutarPlugin("ReporteLog", contextoCompartido);

            // 4. Intentar ejecutar un plug-in inexistente para validar la gestión de errores
            Console.WriteLine();
            kernel.EjecutarPlugin("PluginInexistente", contextoCompartido);
        }
    }
}