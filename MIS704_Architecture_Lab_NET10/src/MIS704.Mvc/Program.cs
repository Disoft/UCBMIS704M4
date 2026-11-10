using System;

namespace MvcDemo
{
    // =========================================================================
    // 1. MODELO (Model)
    // =========================================================================
    // Responsabilidad: Encapsula el estado y la lógica de datos del dominio.
    public class ProductoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

    // =========================================================================
    // 2. VISTA (View)
    // =========================================================================
    // Responsabilidad: Se encarga exclusivamente de la representación visual 
    // y de mostrar la información al usuario.
    public class ProductoView
    {
        public void MostrarDetalles(int id, string nombre, decimal precio)
        {
            Console.WriteLine("=== VISTA DE PRODUCTO ===");
            Console.WriteLine($"ID: {id}");
            Console.WriteLine($"Nombre: {nombre}");
            Console.WriteLine($"Precio: ${precio:F2}");
            Console.WriteLine("=========================\n");
        }

        public void MostrarMensajeError(string mensaje)
        {
            Console.WriteLine($"[Error]: {mensaje}");
        }
    }

    // =========================================================================
    // 3. CONTROLADOR (Controller)
    // =========================================================================
    // Responsabilidad: Recibe la interacción, procesa la entrada, modifica el 
    // Modelo y coordina la actualización de la Vista.
    public class ProductoController
    {
        private readonly ProductoModel _modelo;
        private readonly ProductoView _vista;

        public ProductoController(ProductoModel modelo, ProductoView vista)
        {
            _modelo = modelo;
            _vista = vista;
        }

        public void ActualizarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                _vista.MostrarMensajeError("El nombre no puede estar vacío.");
                return;
            }

            _modelo.Nombre = nuevoNombre;
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio < 0)
            {
                _vista.MostrarMensajeError("El precio no puede ser un valor negativo.");
                return;
            }

            _modelo.Precio = nuevoPrecio;
        }

        public void ActualizarVista()
        {
            _vista.MostrarDetalles(_modelo.Id, _modelo.Nombre, _modelo.Precio);
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Punto de Entrada / Simulación)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Instanciamos los componentes del patrón MVC
            ProductoModel modelo = new ProductoModel
            {
                Id = 1,
                Nombre = "Teclado Mecánico",
                Precio = 85.00m
            };

            ProductoView vista = new ProductoView();
            ProductoController controlador = new ProductoController(modelo, vista);

            // 2. Mostramos el estado inicial a través de la vista coordinada por el controlador
            Console.WriteLine("--- Estado Inicial ---");
            controlador.ActualizarVista();

            // 3. Simulamos la interacción del usuario modificando los datos
            Console.WriteLine("--- Realizando Cambios ---");
            controlador.ActualizarNombre("Teclado Mecánico RGB Pro");
            controlador.ActualizarPrecio(99.99m);

            // 4. Mostramos el resultado actualizado
            controlador.ActualizarVista();

            // 5. Probamos una validación fallida gestionada por el controlador
            Console.WriteLine("--- Intentando ingresar un precio inválido ---");
            controlador.ActualizarPrecio(-15.00m);
        }
    }
}