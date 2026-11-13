
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KerfixDesktop
{
    public partial class ClientesView : UserControl
    {
        private ObservableCollection<ClienteDemo> clientes = new();
        private ICollectionView vistaClientes;
        private string filtroActual = "Todos";

        public ClientesView()
        {
            InitializeComponent();

            // CLIENTES DE PRUEBA
            clientes.Add(new ClienteDemo
            {
                Id = 101,
                Nombre = "Constructora Andes SpA",
                Telefono = "+56 9 8765 4321",
                Atenciones = 7,
                Registros = new()
                {
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 11, 10),
                        Equipo = "Notebook Dell",
                        Estado = "Entregado"
                    },
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 10, 20),
                        Equipo = "PC Oficina",
                        Estado = "Entregado"
                    }
                }
            });

            clientes.Add(new ClienteDemo
            {
                Id = 102,
                Nombre = "Minera del Norte Ltda.",
                Telefono = "+56 9 7123 9876",
                Atenciones = 12,
                Registros = new()
                {
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 11, 12),
                        Equipo = "Notebook HP",
                        Estado = "Entregado"
                    },
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 11, 3),
                        Equipo = "PC Gamer",
                        Estado = "En reparación"
                    },
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 10, 28),
                        Equipo = "All-in-One Lenovo",
                        Estado = "Entregado"
                    },
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 10, 15),
                        Equipo = "Impresora Epson",
                        Estado = "Entregado"
                    }
                }
            });

            clientes.Add(new ClienteDemo
            {
                Id = 103,
                Nombre = "Servicios Industriales S.A.",
                Telefono = "+56 9 9988 7766",
                Atenciones = 5,
                Registros = new()
                {
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 11, 1),
                        Equipo = "Notebook Acer",
                        Estado = "Pendiente"
                    }
                }
            });

            clientes.Add(new ClienteDemo
            {
                Id = 104,
                Nombre = "AgroEquipos Chile",
                Telefono = "+56 9 6677 5544",
                Atenciones = 9,
                Registros = new()
                {
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 11, 8),
                        Equipo = "PC Escritorio",
                        Estado = "Entregado"
                    },
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 10, 26),
                        Equipo = "Notebook ASUS",
                        Estado = "Entregado"
                    }
                }
            });

            clientes.Add(new ClienteDemo
            {
                Id = 105,
                Nombre = "Transporte Rivera",
                Telefono = "+56 9 4433 2211",
                Atenciones = 4,
                Registros = new()
                {
                    new RegistroClienteDemo
                    {
                        Fecha = new DateTime(2026, 11, 5),
                        Equipo = "Notebook Lenovo",
                        Estado = "En reparación"
                    }
                }
            });

            vistaClientes = CollectionViewSource
                .GetDefaultView(clientes);

            vistaClientes.Filter = FiltrarCliente;
            TablaClientes.ItemsSource = vistaClientes;

            ActualizarEstadisticas();

            TablaClientes.SelectedItem = clientes[1];
        }

        // ESTADÍSTICAS
        private void ActualizarEstadisticas()
        {
            TxtTotalClientes.Text = clientes.Count.ToString();

            TxtTotalAtenciones.Text = clientes
                .Sum(c => c.Atenciones)
                .ToString();

            ActualizarCantidadMostrada();
        }

        private void ActualizarCantidadMostrada()
        {
            int cantidad = vistaClientes
                .Cast<object>()
                .Count();

            TxtMostrandoClientes.Text =
                $"Mostrando: {cantidad} clientes";
        }

        // BUSCADOR Y FILTRO
        private bool FiltrarCliente(object objeto)
        {
            if (objeto is not ClienteDemo cliente)
                return false;

            string busqueda = TxtBuscarCliente.Text.Trim();

            bool coincideBusqueda =
                string.IsNullOrWhiteSpace(busqueda) ||
                cliente.Nombre.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase) ||
                cliente.Telefono.Contains(
                    busqueda,
                    StringComparison.OrdinalIgnoreCase);

            bool coincideFiltro = filtroActual switch
            {
                "Con" => cliente.Atenciones > 0,
                "Sin" => cliente.Atenciones == 0,
                _ => true
            };

            return coincideBusqueda && coincideFiltro;
        }

        private void TxtBuscarCliente_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            if (vistaClientes == null)
                return;

            ActualizarVista();
        }

        private void BtnFiltroClientes_Click(
            object sender, RoutedEventArgs e)
        {
            if (BtnFiltroClientes.ContextMenu != null)
            {
                BtnFiltroClientes.ContextMenu.PlacementTarget =
                    BtnFiltroClientes;

                BtnFiltroClientes.ContextMenu.IsOpen = true;
            }
        }

        private void FiltroClientes_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem opcion)
            {
                filtroActual =
                    opcion.Tag?.ToString() ?? "Todos";

                ActualizarVista();
            }
        }

        private void ActualizarVista()
        {
            vistaClientes.Refresh();
            ActualizarCantidadMostrada();

            if (TablaClientes.SelectedItem == null)
            {
                ClienteDemo? primero = vistaClientes
                    .Cast<ClienteDemo>()
                    .FirstOrDefault();

                TablaClientes.SelectedItem = primero;
            }

            ActualizarDetalle(
                TablaClientes.SelectedItem as ClienteDemo);
        }

        // SELECCIÓN DE CLIENTE
        private void TablaClientes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (TxtDetalleNombre == null)
                return;

            ActualizarDetalle(
                TablaClientes.SelectedItem as ClienteDemo);
        }

        private void ActualizarDetalle(ClienteDemo? cliente)
        {
            if (cliente == null)
            {
                TxtDetalleNombre.Text = "Selecciona un cliente";
                TxtDetalleTelefono.Text = "-";
                TxtDetalleAtenciones.Text = "-";

                ListaRegistros.ItemsSource = null;
                TxtSinRegistros.Visibility = Visibility.Visible;
                return;
            }

            TxtDetalleNombre.Text = cliente.Nombre;
            TxtDetalleTelefono.Text = cliente.Telefono;
            TxtDetalleAtenciones.Text =
                cliente.Atenciones.ToString();

            // Solo los últimos 4 registros
            var recientes = cliente.Registros
                .OrderByDescending(r => r.Fecha)
                .Take(4)
                .ToList();

            ListaRegistros.ItemsSource = recientes;

            TxtSinRegistros.Visibility =
                recientes.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        // EDITAR CLIENTE (PENDIENTE)

        private void EditarCliente_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is ClienteDemo cliente)
            {
                // Abrir ventana con los datos del cliente
                var ventana = new EditarClienteWindow(cliente);

                ventana.Owner = Window.GetWindow(this);

                if (ventana.ShowDialog() == true)
                {
                    // Modificar el cliente existente
                    cliente.Nombre = ventana.NombreEditado;
                    cliente.Telefono = ventana.TelefonoEditado;

                    // Actualizar tabla y buscador
                    vistaClientes.Refresh();

                    // Actualizar panel derecho
                    ActualizarDetalle(cliente);

                    MessageBox.Show(
                        "Cambios aplicados a los datos de prueba.\n" +
                        "La actualización en la API está pendiente.",
                        "KerFix Manager",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }


        // ELIMINAR CLIENTE (PRUEBA LOCAL)
        private void EliminarCliente_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is ClienteDemo cliente)
            {
                var respuesta = MessageBox.Show(
                    "¿Eliminar a " + cliente.Nombre + "?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (respuesta == MessageBoxResult.Yes)
                {
                    clientes.Remove(cliente);
                    ActualizarEstadisticas();
                    ActualizarVista();
                }
            }
        }
    }

    // MODELOS TEMPORALES PARA LA INTERFAZ
    public class ClienteDemo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";
        public int Atenciones { get; set; }

        public List<RegistroClienteDemo> Registros { get; set; }
            = new();
    }

    public class RegistroClienteDemo
    {
        public DateTime Fecha { get; set; }
        public string Equipo { get; set; } = "";
        public string Estado { get; set; } = "";
    }
}
