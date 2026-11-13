
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KerfixDesktop
{
    public partial class RegistroView : UserControl
    {
        private ObservableCollection<RegistroDemo> registros = new();
        private ICollectionView vistaRegistros;
        private string filtroActual = "Todos";

        public RegistroView()
        {
            InitializeComponent();

            // REGISTROS DE PRUEBA
            registros.Add(new RegistroDemo
            {
                Id = 1,
                Cliente = "Minera del Norte Ltda.",
                Telefono = "+56 9 7123 9876",
                FechaRecepcion = new DateTime(2026, 11, 12),
                Tipo = "Notebook",
                Marca = "Dell",
                Modelo = "Latitude 5420",
                Serie = "ABC123",
                DiagnosticoCliente = "El equipo no enciende",
                Estado = "En revisión",
                Actualizaciones = new()
                {
                    new ActualizacionRegistroDemo
                    {
                        Fecha = new DateTime(2026, 11, 12, 16, 24, 0),
                        Descripcion = "Registro creado."
                    },
                    new ActualizacionRegistroDemo
                    {
                        Fecha = new DateTime(2026, 11, 12, 16, 45, 0),
                        Descripcion = "Diagnóstico inicial realizado."
                    },
                    new ActualizacionRegistroDemo
                    {
                        Fecha = new DateTime(2026, 11, 12, 17, 10, 0),
                        Descripcion = "Estado actualizado a En revisión."
                    }
                }
            });

            registros.Add(new RegistroDemo
            {
                Id = 2,
                Cliente = "Constructora Andes SpA",
                Telefono = "+56 9 8765 4321",
                FechaRecepcion = new DateTime(2026, 11, 11),
                Tipo = "PC Escritorio",
                Marca = "HP",
                Modelo = "ProDesk 400 G9",
                Serie = "HP40022",
                DiagnosticoCliente = "Lento / bajo rendimiento",
                Estado = "Pendiente"
            });

            registros.Add(new RegistroDemo
            {
                Id = 3,
                Cliente = "Servicios Industriales S.A.",
                Telefono = "+56 9 9988 7766",
                FechaRecepcion = new DateTime(2026, 11, 10),
                Tipo = "All-in-One",
                Marca = "Lenovo",
                Modelo = "ThinkCentre M90a",
                Serie = "LEN9041",
                DiagnosticoCliente = "Se reinicia solo",
                Estado = "En revisión"
            });

            registros.Add(new RegistroDemo
            {
                Id = 4,
                Cliente = "AgroEquipos Chile",
                Telefono = "+56 9 6677 5544",
                FechaRecepcion = new DateTime(2026, 11, 9),
                Tipo = "Impresora",
                Marca = "Epson",
                Modelo = "L3250",
                Serie = "EPS32501",
                DiagnosticoCliente = "Error de impresión",
                Estado = "En revisión"
            });

            registros.Add(new RegistroDemo
            {
                Id = 5,
                Cliente = "Transporte Rivera",
                Telefono = "+56 9 4433 2211",
                FechaRecepcion = new DateTime(2026, 11, 8),
                Tipo = "Notebook",
                Marca = "Acer",
                Modelo = "Aspire 5",
                Serie = "ACR5009",
                DiagnosticoCliente = "Pantalla con líneas",
                Estado = "Pendiente"
            });

            registros.Add(new RegistroDemo
            {
                Id = 6,
                Cliente = "Minera del Norte Ltda.",
                Telefono = "+56 9 7123 9876",
                FechaRecepcion = new DateTime(2026, 11, 7),
                Tipo = "PC Escritorio",
                Marca = "HP",
                Modelo = "EliteDesk 800 G6",
                Serie = "HPEL800",
                DiagnosticoCliente = "No reconoce red",
                Estado = "Completado"
            });

            registros.Add(new RegistroDemo
            {
                Id = 7,
                Cliente = "Constructora Andes SpA",
                Telefono = "+56 9 8765 4321",
                FechaRecepcion = new DateTime(2026, 11, 6),
                Tipo = "Monitor",
                Marca = "LG",
                Modelo = "24MK600M",
                Serie = "LG24600",
                DiagnosticoCliente = "Sin señal",
                Estado = "Completado"
            });

            registros.Add(new RegistroDemo
            {
                Id = 8,
                Cliente = "Servicios Industriales S.A.",
                Telefono = "+56 9 9988 7766",
                FechaRecepcion = new DateTime(2026, 11, 5),
                Tipo = "Notebook",
                Marca = "HP",
                Modelo = "ProBook 440 G9",
                Serie = "HP440G9",
                DiagnosticoCliente = "Batería no carga",
                Estado = "En revisión"
            });

            // CONECTAR DATOS A LA TABLA
            vistaRegistros = CollectionViewSource
                .GetDefaultView(registros);

            vistaRegistros.Filter = FiltrarRegistro;

            TablaRegistros.ItemsSource = vistaRegistros;

            ActualizarEstadisticas();

            // Seleccionar el primer registro
            TablaRegistros.SelectedItem = registros[0];
        }

        // ========================================
        // ESTADÍSTICAS
        // ========================================


        private void ActualizarEstadisticas()
        {
            TxtPendientes.Text = registros.Count(r =>
                r.Estado == "Pendiente").ToString();

            TxtEnReparacion.Text = registros.Count(r =>
                r.Estado == "En revisión").ToString();

            ActualizarCantidadMostrada();
        }


        private void ActualizarCantidadMostrada()
        {
            int cantidad = vistaRegistros
                .Cast<object>()
                .Count();

            TxtMostrandoRegistros.Text =
                $"Mostrando: {cantidad} registros";
        }

        // ========================================
        // BUSCADOR Y FILTRO
        // ========================================

        private bool FiltrarRegistro(object objeto)
        {
            if (objeto is not RegistroDemo registro)
                return false;

            string busqueda = TxtBuscarRegistro.Text.Trim();

            bool coincideBusqueda =
                string.IsNullOrWhiteSpace(busqueda) ||
                registro.Id.ToString().Contains(busqueda) ||
                registro.Cliente.Contains(
                    busqueda, StringComparison.OrdinalIgnoreCase) ||
                registro.Tipo.Contains(
                    busqueda, StringComparison.OrdinalIgnoreCase) ||
                registro.Marca.Contains(
                    busqueda, StringComparison.OrdinalIgnoreCase) ||
                registro.Modelo.Contains(
                    busqueda, StringComparison.OrdinalIgnoreCase) ||
                registro.DiagnosticoCliente.Contains(
                    busqueda, StringComparison.OrdinalIgnoreCase);

            bool coincideEstado =
                filtroActual == "Todos" ||
                registro.Estado == filtroActual;

            return coincideBusqueda && coincideEstado;
        }

        private void TxtBuscarRegistro_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            if (vistaRegistros == null)
                return;

            ActualizarVista();
        }

        private void BtnFiltrarRegistro_Click(
            object sender, RoutedEventArgs e)
        {
            if (BtnFiltrarRegistro.ContextMenu != null)
            {
                BtnFiltrarRegistro.ContextMenu.PlacementTarget =
                    BtnFiltrarRegistro;

                BtnFiltrarRegistro.ContextMenu.IsOpen = true;
            }
        }

        private void FiltroRegistro_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem opcion)
            {
                filtroActual = opcion.Tag?.ToString() ?? "Todos";
                ActualizarVista();
            }
        }

        private void ActualizarVista()
        {
            vistaRegistros.Refresh();
            ActualizarCantidadMostrada();

            if (TablaRegistros.SelectedItem == null)
            {
                TablaRegistros.SelectedItem = vistaRegistros
                    .Cast<RegistroDemo>()
                    .FirstOrDefault();
            }

            ActualizarDetalle(
                TablaRegistros.SelectedItem as RegistroDemo);
        }

        // ========================================
        // SELECCIÓN Y DETALLES
        // ========================================

        private void TablaRegistros_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (TxtDetalleCliente == null)
                return;

            ActualizarDetalle(
                TablaRegistros.SelectedItem as RegistroDemo);
        }

        private void ActualizarDetalle(RegistroDemo? registro)
        {
            if (registro == null)
            {
                TxtIdDetalle.Text = "-";
                TxtDetalleCliente.Text = "Selecciona un registro";
                TxtDetalleTelefono.Text = "-";
                TxtDetalleFecha.Text = "-";
                TxtDetalleTipo.Text = "-";
                TxtDetalleMarca.Text = "-";
                TxtDetalleModelo.Text = "-";
                TxtDetalleSerie.Text = "-";
                TxtDetalleDiagnostico.Text = "-";
                TxtDetalleEstado.Text = "-";

                ListaActualizaciones.ItemsSource = null;
                TxtSinActualizaciones.Visibility = Visibility.Visible;
                return;
            }

            TxtIdDetalle.Text = $"R-{registro.Id:0000}";
            TxtDetalleCliente.Text = registro.Cliente;
            TxtDetalleTelefono.Text = registro.Telefono;
            TxtDetalleFecha.Text =
                registro.FechaRecepcion.ToString("dd/MM/yyyy");

            TxtDetalleTipo.Text = registro.Tipo;
            TxtDetalleMarca.Text = registro.Marca;
            TxtDetalleModelo.Text = registro.Modelo;
            TxtDetalleSerie.Text = registro.Serie;
            TxtDetalleDiagnostico.Text = registro.DiagnosticoCliente;
            TxtDetalleEstado.Text = registro.Estado;

            var recientes = registro.Actualizaciones
                .OrderByDescending(a => a.Fecha)
                .Take(4)
                .ToList();

            ListaActualizaciones.ItemsSource = recientes;

            TxtSinActualizaciones.Visibility =
                recientes.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        // ========================================
        // NUEVO REGISTRO
        // ========================================


        public event EventHandler? SolicitarNuevoRegistro;

        private void NuevoRegistro_Click(
            object sender, RoutedEventArgs e)
        {
            SolicitarNuevoRegistro?.Invoke(this, EventArgs.Empty);
        }

        // ========================================
        // EDITAR REGISTRO
        // ========================================

        private void EditarRegistro_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is RegistroDemo registro)
            {
                MessageBox.Show(
                    "Editar registro R-" + registro.Id.ToString("0000"),
                    "KerFix Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        // ========================================
        // ELIMINAR REGISTRO
        // ========================================

        private void EliminarRegistro_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is RegistroDemo registro)
            {
                var respuesta = MessageBox.Show(
                    $"¿Eliminar el registro R-{registro.Id:0000}?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (respuesta == MessageBoxResult.Yes)
                {
                    // Solo elimina el registro de prueba
                    registros.Remove(registro);

                    ActualizarEstadisticas();
                    ActualizarVista();
                }
            }
        }
    }

    // ========================================
    // MODELOS TEMPORALES
    // ========================================

    public class RegistroDemo
    {
        public int Id { get; set; }

        public string Cliente { get; set; } = "";
        public string Telefono { get; set; } = "";

        public DateTime FechaRecepcion { get; set; }

        public string Tipo { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
        public string Serie { get; set; } = "";

        public string DiagnosticoCliente { get; set; } = "";
        public string Estado { get; set; } = "";

        public List<ActualizacionRegistroDemo> Actualizaciones
        { get; set; } = new();
    }

    public class ActualizacionRegistroDemo
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = "";
    }
}
