
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace KerfixDesktop
{
    public partial class ServicioView : UserControl
    {
        private readonly ObservableCollection<ServicioDemo> servicios = new();
        private ICollectionView? vistaServicios;

        private ServicioDemo? servicioEditando;
        private bool modoEdicion = false;

        public ServicioView()
        {
            InitializeComponent();

            // SERVICIOS DE PRUEBA
            servicios.Add(new ServicioDemo
            {
                Id = 1,
                Tipo = "Formateo",
                Descripcion = "Instalación limpia de Windows.",
                Valor = 15000
            });

            servicios.Add(new ServicioDemo
            {
                Id = 2,
                Tipo = "Formateo + Office",
                Descripcion = "Instalación de Windows y Office.",
                Valor = 20000
            });

            servicios.Add(new ServicioDemo
            {
                Id = 3,
                Tipo = "Optimización",
                Descripcion = "Limpieza y optimización de software.",
                Valor = 10000
            });

            servicios.Add(new ServicioDemo
            {
                Id = 4,
                Tipo = "Mantención",
                Descripcion = "Limpieza interna y cambio de pasta térmica.",
                Valor = 15000
            });

            servicios.Add(new ServicioDemo
            {
                Id = 5,
                Tipo = "Diagnóstico",
                Descripcion = "Revisión general del equipo.",
                Valor = 5000
            });

            // CONECTAR TABLA
            vistaServicios = CollectionViewSource
                .GetDefaultView(servicios);

            vistaServicios.Filter = FiltrarServicio;
            TablaServicios.ItemsSource = vistaServicios;

            ActualizarContadores();

            // Mostrar detalle del primer servicio
            TablaServicios.SelectedItem = servicios[0];
            MostrarDetalle();
        }

        // ========================================
        // CONTADORES
        // ========================================

        private void ActualizarContadores()
        {
            TxtTotalServicios.Text = servicios.Count.ToString();
            ActualizarCantidadMostrada();
        }

        private void ActualizarCantidadMostrada()
        {
            if (vistaServicios == null)
                return;

            int cantidad = vistaServicios
                .Cast<object>()
                .Count();

            TxtMostrandoServicios.Text =
                $"Mostrando: {cantidad} servicios";
        }

        // ========================================
        // BUSCADOR
        // ========================================

        private bool FiltrarServicio(object objeto)
        {
            if (objeto is not ServicioDemo servicio)
                return false;

            string busqueda = TxtBuscarServicio.Text.Trim();

            return string.IsNullOrWhiteSpace(busqueda) ||
                servicio.Tipo.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase) ||
                servicio.Descripcion.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase);
        }

        private void TxtBuscarServicio_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            if (vistaServicios == null)
                return;

            ActualizarVista();
        }

        private void ActualizarVista()
        {
            if (vistaServicios == null)
                return;

            vistaServicios.Refresh();
            ActualizarCantidadMostrada();

            if (TablaServicios.SelectedItem == null)
            {
                TablaServicios.SelectedItem = vistaServicios
                    .Cast<ServicioDemo>()
                    .FirstOrDefault();
            }

            ActualizarDetalle();
        }

        // ========================================
        // SELECCIONAR SERVICIO
        // ========================================

        private void TablaServicios_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (TxtDetalleId == null)
                return;

            ActualizarDetalle();

            // Al seleccionar un servicio, mostrar sus datos
            // excepto cuando estamos editando el formulario.
            if (PanelFormulario.Visibility != Visibility.Visible)
            {
                MostrarDetalle();
            }
        }

        private void ActualizarDetalle()
        {
            if (TablaServicios.SelectedItem
                is not ServicioDemo servicio)
            {
                TxtDetalleId.Text = "-";
                TxtDetalleTipo.Text = "Selecciona un servicio";
                TxtDetalleDescripcion.Text = "-";
                TxtDetalleValor.Text = "-";
                return;
            }

            TxtDetalleId.Text = servicio.Id.ToString();
            TxtDetalleTipo.Text = servicio.Tipo;
            TxtDetalleDescripcion.Text = servicio.Descripcion;
            TxtDetalleValor.Text = FormatearPrecio(servicio.Valor);
        }

        private string FormatearPrecio(decimal valor)
        {
            return "$" + valor.ToString(
                "N0",
                CultureInfo.GetCultureInfo("es-CL"));
        }

        // ========================================
        // MOSTRAR DETALLE
        // ========================================

        private void MostrarDetalle()
        {
            PanelFormulario.Visibility = Visibility.Collapsed;
            PanelDetalle.Visibility = Visibility.Visible;

            ActualizarDetalle();
        }

        // ========================================
        // NUEVO SERVICIO
        // ========================================

        private void NuevoServicio_Click(
            object sender, RoutedEventArgs e)
        {
            modoEdicion = false;
            servicioEditando = null;

            TxtTituloFormulario.Text = "Nuevo servicio";
            BtnGuardarServicio.Content = "Guardar servicio";

            TxtNombreServicio.Clear();
            TxtDescripcionServicio.Clear();
            TxtValorServicio.Clear();

            PanelDetalle.Visibility = Visibility.Collapsed;
            PanelFormulario.Visibility = Visibility.Visible;

            TxtNombreServicio.Focus();
        }

        // ========================================
        // EDITAR SERVICIO
        // ========================================

        private void EditarServicio_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is ServicioDemo servicio)
            {
                PrepararEdicion(servicio);
            }
        }

        private void EditarSeleccionado_Click(
            object sender, RoutedEventArgs e)
        {
            if (TablaServicios.SelectedItem
                is ServicioDemo servicio)
            {
                PrepararEdicion(servicio);
            }
        }

        private void PrepararEdicion(ServicioDemo servicio)
        {
            modoEdicion = true;
            servicioEditando = servicio;

            TablaServicios.SelectedItem = servicio;

            TxtTituloFormulario.Text = "Editar servicio";
            BtnGuardarServicio.Content = "Guardar cambios";

            TxtNombreServicio.Text = servicio.Tipo;
            TxtDescripcionServicio.Text = servicio.Descripcion;
            TxtValorServicio.Text = servicio.Valor.ToString("0.##");

            PanelDetalle.Visibility = Visibility.Collapsed;
            PanelFormulario.Visibility = Visibility.Visible;

            TxtNombreServicio.Focus();
        }

        // ========================================
        // VALIDAR NÚMEROS
        // ========================================

        private void SoloNumeros(
            object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        // ========================================
        // GUARDAR SERVICIO
        // ========================================

        private void GuardarServicio_Click(
            object sender, RoutedEventArgs e)
        {
            string nombre = TxtNombreServicio.Text.Trim();
            string descripcion =
                TxtDescripcionServicio.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Aviso("Ingresa el nombre del servicio.");
                return;
            }

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                Aviso("Ingresa la descripción del servicio.");
                return;
            }

            if (!decimal.TryParse(
                    TxtValorServicio.Text,
                    out decimal valor) ||
                valor < 0)
            {
                Aviso("Ingresa un valor válido.");
                return;
            }

            if (modoEdicion && servicioEditando != null)
            {
                // ACTUALIZAR SERVICIO EXISTENTE
                servicioEditando.Tipo = nombre;
                servicioEditando.Descripcion = descripcion;
                servicioEditando.Valor = valor;

                vistaServicios?.Refresh();

                TablaServicios.SelectedItem = servicioEditando;
            }
            else
            {
                // AGREGAR SERVICIO DE PRUEBA
                int siguienteId = servicios.Count == 0
                    ? 1
                    : servicios.Max(s => s.Id) + 1;

                var nuevo = new ServicioDemo
                {
                    Id = siguienteId,
                    Tipo = nombre,
                    Descripcion = descripcion,
                    Valor = valor
                };

                servicios.Add(nuevo);

                ActualizarContadores();

                vistaServicios?.Refresh();
                TablaServicios.SelectedItem = nuevo;
            }

            // Volver al detalle
            MostrarDetalle();
            ActualizarContadores();

            MessageBox.Show(
                "Cambios aplicados a los datos de prueba.\n" +
                "El guardado en la API está pendiente.",
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // ========================================
        // CANCELAR FORMULARIO
        // ========================================

        private void Cancelar_Click(
            object sender, RoutedEventArgs e)
        {
            modoEdicion = false;
            servicioEditando = null;

            MostrarDetalle();
        }

        // ========================================
        // ELIMINAR SERVICIO
        // ========================================

        private void EliminarServicio_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is ServicioDemo servicio)
            {
                EliminarServicio(servicio);
            }
        }

        private void EliminarSeleccionado_Click(
            object sender, RoutedEventArgs e)
        {
            if (TablaServicios.SelectedItem
                is ServicioDemo servicio)
            {
                EliminarServicio(servicio);
            }
        }

        private void EliminarServicio(ServicioDemo servicio)
        {
            var respuesta = MessageBox.Show(
                $"¿Eliminar el servicio \"{servicio.Tipo}\"?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            // ELIMINACIÓN LOCAL DE PRUEBA
            servicios.Remove(servicio);

            ActualizarContadores();
            ActualizarVista();
            MostrarDetalle();
        }

        // ========================================
        // AVISOS
        // ========================================

        private void Aviso(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    // MODELO TEMPORAL DE SERVICIOS
    public class ServicioDemo
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Valor { get; set; }
    }
}
