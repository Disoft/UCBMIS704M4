using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MvvmDemo
{
    // =========================================================================
    // 1. MODELO (Model)
    // =========================================================================
    // Representa los datos puros y la lógica de negocio del dominio.
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

    // =========================================================================
    // CLASE BASE PARA NOTIFICAR CAMBIOS (INotifyPropertyChanged)
    // =========================================================================
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref int storage, T value, [CallerMemberName] string propertyName = null)
        {
            // Simplificación para el ejemplo
            return true;
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    // =========================================================================
    // IMPLEMENTACIÓN BÁSICA DE ICommand
    // =========================================================================
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // =========================================================================
    // 2. VIEWMODEL (ViewModel)
    // =========================================================================
    // Expone el estado de presentación, propiedades enlazables y comandos.
    // No tiene referencias directas a elementos visuales de la Vista.
    public class ProductoViewModel : ObservableObject
    {
        private readonly Producto _modelo;

        private string _nombreEditable;
        private decimal _precioEditable;
        private string _mensajeEstado;

        public ProductoViewModel()
        {
            // Simulamos la carga de un modelo de datos
            _modelo = new Producto { Id = 1, Nombre = "Monitor Ultrawide", Precio = 350.00m };

            // Inicializamos las propiedades de UI con los valores del modelo
            _nombreEditable = _modelo.Nombre;
            _precioEditable = _modelo.Precio;

            // Inicializamos los comandos expuestos a la Vista
            GuardarCommand = new RelayCommand(GuardarCambios, ValidarPermisoGuardar);
        }

        // Propiedades enlazadas (Data Binding)
        public string NombreEditable
        {
            get => _nombreEditable;
            set
            {
                if (SetProperty(ref _nombreEditable, value))
                {
                    (GuardarCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal PrecioEditable
        {
            get => _precioEditable;
            set => SetProperty(ref _precioEditable, value);
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        // Comandos expuestos
        public ICommand GuardarCommand { get; }

        private void GuardarCambios()
        {
            // Actualizamos el modelo con los datos validados del ViewModel
            _modelo.Nombre = NombreEditable;
            _modelo.Precio = PrecioEditable;

            MensajeEstado = $"[Éxito] Producto '{_modelo.Nombre}' actualizado correctamente a las {DateTime.Now:T}.";
        }

        private bool ValidarPermisoGuardar()
        {
            // Regla de habilitación para el comando: el nombre no puede estar vacío
            return !string.IsNullOrWhiteSpace(NombreEditable);
        }
    }

    // =========================================================================
    // 3. VISTA (View - Simulación en Consola del Data Binding)
    // =========================================================================
    // En frameworks como WPF, MAUI o WinUI, esta clase se conecta al ViewModel
    // declarativamente mediante XAML o enlaces de datos automáticos.
    public class ProductoViewSimulada
    {
        private readonly ProductoViewModel _viewModel;

        public ProductoViewSimulada(ProductoViewModel viewModel)
        {
            _viewModel = viewModel;

            // Simulamos el enlace de datos (Binding): Nos suscribimos a los cambios de propiedad
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ProductoViewModel.MensajeEstado))
                {
                    Console.WriteLine($"[Vista - Notificación de UI]: {_viewModel.MensajeEstado}");
                }
            };
        }

        public void MostrarEstadoActual()
        {
            Console.WriteLine("=== PANTALLA DE EDICIÓN DE PRODUCTO (MVVM) ===");
            Console.WriteLine($"-> Campo Nombre (Binding): {_viewModel.NombreEditable}");
            Console.WriteLine($"-> Campo Precio (Binding): ${_viewModel.PrecioEditable:F2}");
            Console.WriteLine($"-> ¿Comando Guardar habilitado?: {_viewModel.GuardarCommand.CanExecute(null)}");
            Console.WriteLine("----------------------------------------------\n");
        }

        public void SimularAccionUsuarioEscribir(string nuevoNombre, decimal nuevoPrecio)
        {
            Console.WriteLine($"(Usuario escribe en la UI) -> Nuevo Nombre: '{nuevoNombre}', Nuevo Precio: ${nuevoPrecio}");

            // El usuario interactúa modificando las propiedades del ViewModel (vía Binding bidireccional)
            _viewModel.NombreEditable = nuevoNombre;
            _viewModel.PrecioEditable = nuevoPrecio;
        }

        public void SimularClickEnBotonGuardar()
        {
            Console.WriteLine("(Usuario hace clic en el botón 'Guardar')");
            if (_viewModel.GuardarCommand.CanExecute(null))
            {
                _viewModel.GuardarCommand.Execute(null);
            }
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Punto de Arranque)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Instanciamos el ViewModel (el cual encapsula al Modelo)
            ProductoViewModel viewModel = new ProductoViewModel();

            // 2. Instanciamos la Vista y la enlazamos al ViewModel
            ProductoViewSimulada vista = new ProductoViewSimulada(viewModel);

            // 3. Mostramos la vista con su estado inicial sincronizado
            vista.MostrarEstadoActual();

            // 4. Simulamos interacción de usuario en la interfaz gráfica
            vista.SimularAccionUsuarioEscribir("Monitor Curvado 4K", 499.99m);
            Console.WriteLine();

            // 5. Verificamos cambios reflejados y ejecutamos el comando
            vista.MostrarEstadoActual();
            vista.SimularClickEnBotonGuardar();
        }
    }
}