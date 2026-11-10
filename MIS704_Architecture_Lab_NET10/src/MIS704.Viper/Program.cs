using System;
using System.Collections.Generic;

namespace ViperArchitectureDemo
{
    // =========================================================================
    // 1. ENTITY (Modelos de datos puros del dominio)
    // =========================================================================
    public class ProductoEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

    // =========================================================================
    // 2. CONTRATOS (Interfaces que comunican los componentes del módulo VIPER)
    // =========================================================================
    public interface IView
    {
        void MostrarDetalleProducto(string detalle);
        void MostrarError(string mensaje);
    }

    public interface IPresenter
    {
        void SolicitarCargarProducto(int id);
        void MostrarProductoCargado(ProductoEntity producto);
        void MostrarErrorCarga(string mensaje);
        void NavegarADetalleAvanzado(int id);
    }

    public interface IInteractor
    {
        void ObtenerProductoPorId(int id);
    }

    public interface IRouter
    {
        void NavegarAPantallaAvanzada(int idProducto);
    }

    // =========================================================================
    // 3. INTERACTOR (Lógica de negocio y casos de uso)
    // =========================================================================
    public class ProductoInteractor : IInteractor
    {
        private readonly IPresenter _presenter;

        public ProductoInteractor(IPresenter presenter)
        {
            _presenter = presenter;
        }

        public void ObtenerProductoPorId(int id)
        {
            // Simulación de búsqueda en una base de datos o API (Caso de uso)
            if (id <= 0)
            {
                _presenter.MostrarErrorCarga("El identificador del producto no es válido.");
                return;
            }

            // Simulando un producto encontrado
            var producto = new ProductoEntity
            {
                Id = id,
                Nombre = id == 1 ? "Laptop Pro 16\"" : "Teclado Mecánico RGB",
                Precio = id == 1 ? 1200.00m : 85.50m
            };

            // Devuelve los datos puros al Presenter
            _presenter.MostrarProductoCargado(producto);
        }
    }

    // =========================================================================
    // 4. PRESENTER (Lógica de presentación, coordinación y preparación de datos)
    // =========================================================================
    public class ProductoPresenter : IPresenter
    {
        public IView View { get; set; } // Se asigna tras la creación por el Router/Ensamblador
        public IInteractor Interactor { get; set; }
        public IRouter Router { get; set; }

        public void SolicitarCargarProducto(int id)
        {
            // El Presenter le pide al Interactor que ejecute el caso de uso
            Interactor.ObtenerProductoPorId(id);
        }

        public void MostrarProductoCargado(ProductoEntity producto)
        {
            // Prepara los datos crudos de la entidad para que la Vista los consuma directamente
            string detalleFormateado = $"[VIPER] Producto: {producto.Nombre} | Precio: ${producto.Precio:F2}";
            View.MostrarDetalleProducto(detalleFormateado);

            // Simula una transición disparada por lógica de negocio hacia otra pantalla
            Router.NavegarAPantallaAvanzada(producto.Id);
        }

        public void MostrarErrorCarga(string mensaje)
        {
            View.MostrarError(mensaje);
        }

        public void NavegarADetalleAvanzado(int id)
        {
            Router.NavegarAPantallaAvanzada(id);
        }
    }

    // =========================================================================
    // 5. ROUTER / WIREFRAME (Gestión de navegación y flujo de pantallas)
    // =========================================================================
    public class ProductoRouter : IRouter
    {
        public void NavegarAPantallaAvanzada(int idProducto)
        {
            Console.WriteLine($"[ROUTER] Transicionando a la vista de detalles avanzados para el producto ID: {idProducto}...\n");
        }

        // Método estático para ensamblar todo el módulo VIPER inicial
        public static (IView view, IPresenter presenter) CrearModulo()
        {
            var view = new ProductoView();
            var presenter = new ProductoPresenter();
            var interactor = new ProductoInteractor(presenter);
            var router = new ProductoRouter();

            // Conexión bidireccional y de dependencias del módulo
            presenter.View = view;
            presenter.Interactor = interactor;
            presenter.Router = router;
            view.Presenter = presenter;

            return (view, presenter);
        }
    }

    // =========================================================================
    // 6. VIEW (Interfaz de usuario y captura de entradas)
    // =========================================================================
    public class ProductoView : IView
    {
        public IPresenter Presenter { get; set; }

        public void CargarPantalla(int idProductoBuscado)
        {
            Console.WriteLine("--- [VIEW] Renderizando Pantalla de Producto ---");
            // La vista notifica la interacción al Presenter
            Presenter.SolicitarCargarProducto(idProductoBuscado);
        }

        public void MostrarDetalleProducto(string detalle)
        {
            Console.WriteLine($"[VIEW - UI Render]: {detalle}");
        }

        public void MostrarError(string mensaje)
        {
            Console.WriteLine($"[VIEW - Alerta UI]: Error -> {mensaje}");
        }
    }

    // =========================================================================
    // 7. PROGRAMA PRINCIPAL (Arranque del módulo)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Ensamblamos el módulo VIPER a través del Router
            var (viewGenerica, presenter) = ProductoRouter.CrearModulo();
            ProductoView vistaProducto = (ProductoView)viewGenerica;

            // Simulamos el uso de la aplicación con un ID válido
            vistaProducto.CargarPantalla(1);

            // Simulamos el uso con un ID inválido para probar la validación del Interactor
            Console.WriteLine("---------------------------------------------");
            vistaProducto.CargarPantalla(0);
        }
    }
}