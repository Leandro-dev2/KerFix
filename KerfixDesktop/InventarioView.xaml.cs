
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KerfixDesktop
{
    public partial class InventarioView : UserControl
    {
        private ObservableCollection<Repuesto> repuestos = new();
        private ICollectionView vistaRepuestos;
        private string categoriaActual = "Todo";

        // Eventos para abrir los formularios
        public event EventHandler? SolicitarNuevoRepuesto;
        public event Action<Repuesto>? SolicitarEditarRepuesto;

        public InventarioView()
        {
            InitializeComponent();

            // DATOS TEMPORALES
            repuestos.Add(new Repuesto
            {
                Id = 1,
                TipoRepuesto = "Memoria RAM",
                Especificacion = "DDR4",
                Capacidad = "8 GB",
                Categoria = "RAM",
                Cantidad = 5,
                Precio = 18000
            });

            repuestos.Add(new Repuesto
            {
                Id = 2,
                TipoRepuesto = "Disco sólido",
                Especificacion = "SSD SATA",
                Capacidad = "480 GB",
                Categoria = "Almacenamiento",
                Cantidad = 3,
                Precio = 32000
            });

            repuestos.Add(new Repuesto
            {
                Id = 3,
                TipoRepuesto = "Disco duro",
                Especificacion = "HDD",
                Capacidad = "1 TB",
                Categoria = "Almacenamiento",
                Cantidad = 2,
                Precio = 28000
            });

            repuestos.Add(new Repuesto
            {
                Id = 4,
                TipoRepuesto = "Placa Wi-Fi",
                Especificacion = "PCIe",
                Capacidad = "-",
                Categoria = "Placa",
                Cantidad = 4,
                Precio = 15000
            });

            repuestos.Add(new Repuesto
            {
                Id = 5,
                TipoRepuesto = "Cable HDMI",
                Especificacion = "v2.0",
                Capacidad = "1.8 m",
                Categoria = "Vario",
                Cantidad = 10,
                Precio = 4000
            });

            vistaRepuestos = CollectionViewSource
                .GetDefaultView(repuestos);

            vistaRepuestos.Filter = FiltrarRepuesto;
            TablaInventario.ItemsSource = vistaRepuestos;

            ActualizarTotal();
        }

        // CONTADOR
        private void ActualizarTotal()
        {
            TxtTotalRepuestos.Text = repuestos.Count.ToString();
        }

        // COMPATIBILIDAD CON LOS FILTROS ANTERIORES
        private string NormalizarTipo(string tipo)
        {
            return tipo.Trim().ToLowerInvariant() switch
            {
                "ram" => "RAM",
                "placas" => "Placa",
                "placa" => "Placa",
                "diverso" => "Vario",
                "vario" => "Vario",
                "almacenamiento" => "Almacenamiento",
                "todo" => "Todo",
                _ => tipo
            };
        }

        // FILTROS
        private bool FiltrarRepuesto(object objeto)
        {
            if (objeto is not Repuesto repuesto)
                return false;

            string busqueda = TxtBuscar.Text.Trim();

            bool coincideCategoria =
                categoriaActual == "Todo" ||
                NormalizarTipo(repuesto.Categoria) ==
                NormalizarTipo(categoriaActual);

            bool coincideBusqueda =
                string.IsNullOrWhiteSpace(busqueda) ||
                repuesto.TipoRepuesto.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase) ||
                repuesto.Especificacion.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase) ||
                repuesto.Capacidad.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase);

            return coincideCategoria && coincideBusqueda;
        }

        // BUSCADOR
        private void TxtBuscar_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            vistaRepuestos?.Refresh();
        }

        // ABRIR FILTROS
        private void BtnFiltrar_Click(
            object sender, RoutedEventArgs e)
        {
            if (BtnFiltrar.ContextMenu != null)
            {
                BtnFiltrar.ContextMenu.PlacementTarget =
                    BtnFiltrar;

                BtnFiltrar.ContextMenu.IsOpen = true;
            }
        }

        // SELECCIONAR FILTRO
        private void Filtro_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem opcion)
            {
                categoriaActual = NormalizarTipo(
                    opcion.Tag?.ToString() ?? "Todo");

                TxtFiltroActual.Text =
                    "Mostrando: " + categoriaActual;

                vistaRepuestos.Refresh();
            }
        }

        // PESTAÑA REPUESTOS
        private void BtnRepuestos_Click(
            object sender, RoutedEventArgs e)
        {
            // Ya estamos en Repuestos.
        }

        // PESTAÑA VENTAS
        private void BtnVentas_Click(
            object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "El módulo de ventas se implementará más adelante.",
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // NUEVO REPUESTO
        private void NuevoRepuesto_Click(
            object sender, RoutedEventArgs e)
        {
            SolicitarNuevoRepuesto?.Invoke(this, EventArgs.Empty);
        }

        // EDITAR REPUESTO
        private void EditarRepuesto_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is Repuesto repuesto)
            {
                SolicitarEditarRepuesto?.Invoke(repuesto);
            }
        }

        // ACTUALIZACIÓN LOCAL DE PRUEBA
        public void ActualizarRepuestoLocal(
            DatosRepuestoEventArgs datos)
        {
            if (!datos.EsEdicion || !datos.Id.HasValue)
                return;

            Repuesto? repuesto = repuestos
                .FirstOrDefault(r => r.Id == datos.Id.Value);

            if (repuesto == null)
                return;

            // Se modifica el registro existente
            repuesto.TipoRepuesto = datos.TipoRepuesto;
            repuesto.Especificacion = datos.Especificacion;
            repuesto.Capacidad = datos.Capacidad;
            repuesto.Categoria = datos.TipoRepuesto;
            repuesto.Cantidad = datos.Cantidad;
            repuesto.Precio = datos.Precio;

            // Actualizar visualmente la tabla
            vistaRepuestos.Refresh();
        }

        // ELIMINAR REPUESTO
        private void EliminarRepuesto_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is Repuesto repuesto)
            {
                var respuesta = MessageBox.Show(
                    "¿Eliminar " + repuesto.TipoRepuesto + "?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (respuesta == MessageBoxResult.Yes)
                {
                    repuestos.Remove(repuesto);
                    ActualizarTotal();
                }
            }
        }
    }

    // MODELO TEMPORAL
    public class Repuesto
    {
        public int Id { get; set; }
        public string TipoRepuesto { get; set; } = "";
        public string Especificacion { get; set; } = "";
        public string Capacidad { get; set; } = "";
        public string Categoria { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
