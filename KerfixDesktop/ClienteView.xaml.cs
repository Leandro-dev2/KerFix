
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net.Http;
using KerfixDesktop.Services;
using KerfixDesktop.Models;

namespace KerfixDesktop
{
    public partial class ClientesView : UserControl
    {
        private readonly HttpClient _httpClient;

        

        private readonly ClienteApiService _clienteApiService;
        private ObservableCollection<ClienteDemo> clientes = new();
        private ICollectionView vistaClientes;
        private string filtroActual = "Todos";

        public ClientesView()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7184/")
            };

            _clienteApiService = new ClienteApiService(_httpClient);

            InitializeComponent();
            Loaded += ClientesView_Loaded;

            

            vistaClientes = CollectionViewSource
                .GetDefaultView(clientes);

            vistaClientes.Filter = FiltrarCliente;
            TablaClientes.ItemsSource = vistaClientes;

            ActualizarEstadisticas();

          
        }


        private async void ClientesView_Loaded(
    object sender,
    RoutedEventArgs e)
        {
            try
            {
                await RecargarClientesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar clientes: {ex.Message}");
            }
        }


        public async Task RecargarClientesAsync()
        {
            var clientesApi = await _clienteApiService.ObtenerClientesAsync();

            clientes.Clear();

            foreach (var cliente in clientesApi)
            {
                clientes.Add(new ClienteDemo
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    Telefono = cliente.Telefono,
                    Atenciones = 0,
                    Registros = new()
                });
            }

            vistaClientes.Refresh();
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

        private async void EditarCliente_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is ClienteDemo cliente)
            {
                var ventana = new EditarClienteWindow(cliente);

                ventana.Owner = Window.GetWindow(this);

                if (ventana.ShowDialog() == true)
                {
                    try
                    {
                        var clienteActualizado = new Cliente
                        {
                            Id = cliente.Id,
                            Nombre = ventana.NombreEditado,
                            Telefono = ventana.TelefonoEditado
                        };

                        await _clienteApiService.ActualizarClienteAsync(
                            cliente.Id,
                            clienteActualizado
                            );

                        cliente.Nombre = clienteActualizado.Nombre;
                        cliente.Telefono = clienteActualizado.Telefono;

                        vistaClientes.Refresh();
                        ActualizarVista();

                        MessageBox.Show(
                            "Cliente actualizado correctamente.",
                            "KerFix Manager",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    catch (HttpRequestException ex)
                    {
                        MessageBox.Show(
                            $"Error al actualizar el cliente: {ex.Message}",
                            "KerFix Manager",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                    
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
