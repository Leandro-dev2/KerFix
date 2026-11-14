
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Http;
using KerfixDesktop.Models;
using KerfixDesktop.Services;

namespace KerfixDesktop
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly RegistroApiService _registroApiService;
        private readonly ClienteApiService _clienteApiService;
        private readonly Brush colorSeleccionado =
            new SolidColorBrush(Color.FromRgb(40, 103, 189));

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7184/")
            };

            _registroApiService = new RegistroApiService(_httpClient);
            _clienteApiService = new ClienteApiService(_httpClient);
            // ========================================
            // INVENTARIO
            // ========================================

            PanelInventario.SolicitarNuevoRepuesto += (s, e) =>
            {
                AbrirNuevoRepuesto();
            };

            PanelInventario.SolicitarEditarRepuesto += repuesto =>
            {
                AbrirEditarRepuesto(repuesto);
            };

            PanelFormularioRepuesto.Cancelado += (s, e) =>
            {
                MostrarInventario();
            };

            PanelFormularioRepuesto.Guardado += (s, datos) =>
            {
                if (datos.EsEdicion)
                {
                    PanelInventario.ActualizarRepuestoLocal(datos);

                    MessageBox.Show(
                        "Cambios aplicados a los datos de prueba.\n" +
                        "La actualización en la API está pendiente.",
                        "KerFix Manager",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    MostrarInventario();
                }
                else
                {
                    MessageBox.Show(
                        "Formulario validado correctamente.\n" +
                        "El guardado en la base de datos está pendiente.",
                        "KerFix Manager",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            };

            // ========================================
            // REGISTROS
            // ========================================

            PanelRegistros.SolicitarNuevoRegistro += (s, e) =>
            {
                AbrirNuevoRegistro();
            };

            PanelNuevoRegistro.Cancelado += (s, e) =>
            {
                MostrarRegistros();
            };

            PanelNuevoRegistro.Guardado += async (s, datos) =>
            {
                try
                {
                    int idCliente;

                    // 1. Crear cliente o utilizar uno existente
                    if (datos.ClienteNuevo)
                    {
                        var nuevoCliente = new Cliente
                        {
                            Nombre = datos.Nombre,
                            Telefono = datos.Telefono
                        };

                        var clienteCreado =
                            await _clienteApiService.CrearClienteAsync(nuevoCliente);

                        idCliente = clienteCreado.Id;
                    }
                    else
                    {
                        if (datos.IdCliente is not int idExistente)
                        {
                            MessageBox.Show("Selecciona un cliente válido.");
                            return;
                        }

                        idCliente = idExistente;
                    }

                    // 2. Por ahora, registrar solamente equipos nuevos
                    if (!datos.EquipoNuevo)
                    {
                        MessageBox.Show(
                            "Todavía debemos conectar los equipos existentes.",
                            "KerFix Manager",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        return;
                    }

                    // 3. Crear el registro de recepción
                    var registro = new Registro
                    {
                        IdCliente = idCliente,
                        Tipo = datos.Tipo,
                        Marca = datos.Marca,
                        Modelo = datos.Modelo,
                        Serie = datos.Serie,
                        DiagnosticoCliente = datos.Diagnostico
                    };

                    var registroCreado =
                        await _registroApiService.CrearRegistroAsync(registro);

                    MessageBox.Show(
                        $"Registro N° {registroCreado.Id} creado correctamente.",
                        "KerFix Manager",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // 4. Regresar al listado
                    MostrarRegistros();
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show(
                        $"Error al guardar los datos:\n\n{ex.Message}",
                        "KerFix Manager",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ocurrió un error inesperado:\n\n{ex.Message}",
                        "KerFix Manager",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            };
        }

        // ========================================
        // OCULTAR PANELES
        // ========================================

        private void OcultarPaneles()
        {
            PanelInicio.Visibility = Visibility.Collapsed;
            PanelClientes.Visibility = Visibility.Collapsed;
            PanelInventario.Visibility = Visibility.Collapsed;
            PanelFormularioRepuesto.Visibility = Visibility.Collapsed;
            PanelRegistros.Visibility = Visibility.Collapsed;
            PanelNuevoRegistro.Visibility = Visibility.Collapsed;
            PanelServicios.Visibility = Visibility.Collapsed;
            PanelDespachos.Visibility = Visibility.Collapsed;
        }

        // ========================================
        // MENÚ LATERAL
        // ========================================

        private void SeleccionarMenu(string seccion)
        {
            BtnInicio.Background = Brushes.Transparent;
            BtnClientes.Background = Brushes.Transparent;
            BtnRegistros.Background = Brushes.Transparent;
            BtnDespachos.Background = Brushes.Transparent;
            BtnInventario.Background = Brushes.Transparent;
            BtnServicios.Background = Brushes.Transparent;

            switch (seccion)
            {
                case "Inicio":
                    BtnInicio.Background = colorSeleccionado;
                    break;

                case "Clientes":
                    BtnClientes.Background = colorSeleccionado;
                    break;

                case "Registros":
                    BtnRegistros.Background = colorSeleccionado;
                    break;

                case "Despachos":
                    BtnDespachos.Background = colorSeleccionado;
                    break;

                case "Inventario":
                    BtnInventario.Background = colorSeleccionado;
                    break;

                case "Servicios":
                    BtnServicios.Background = colorSeleccionado;
                    break;
            }
        }

        // ========================================
        // INICIO
        // ========================================

        private void Inicio_Click(
            object sender, RoutedEventArgs e)
        {
            OcultarPaneles();
            PanelInicio.Visibility = Visibility.Visible;
            SeleccionarMenu("Inicio");
        }

        // ========================================
        // CLIENTES
        // ========================================

        private async void Clientes_Click(
    object sender, RoutedEventArgs e)
        {
            OcultarPaneles();

            PanelClientes.Visibility = Visibility.Visible;

            SeleccionarMenu("Clientes");

            try
            {
                await PanelClientes.RecargarClientesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudieron actualizar los clientes:\n{ex.Message}",
                    "KerFix Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ========================================
        // REGISTROS
        // ========================================

        private void Registros_Click(
            object sender, RoutedEventArgs e)
        {
            MostrarRegistros();
        }

        private void MostrarRegistros()
        {
            OcultarPaneles();
            PanelRegistros.Visibility = Visibility.Visible;
            SeleccionarMenu("Registros");
        }

        // NUEVO REGISTRO
        private void AbrirNuevoRegistro()
        {
            PanelNuevoRegistro.PrepararNuevo();

            OcultarPaneles();
            PanelNuevoRegistro.Visibility = Visibility.Visible;

            SeleccionarMenu("Registros");
        }

        // NUEVO REGISTRO DESDE INICIO
        private void NuevoRegistroInicio_Click(
            object sender, RoutedEventArgs e)
        {
            AbrirNuevoRegistro();
        }

        // ========================================
        // DESPACHOS
        // ========================================

        private void Despachos_Click(
            object sender, RoutedEventArgs e)
        {
            OcultarPaneles();

            PanelDespachos.Visibility = Visibility.Visible;

            SeleccionarMenu("Despachos");
        }

        // ========================================
        // INVENTARIO
        // ========================================

        private void Inventario_Click(
            object sender, RoutedEventArgs e)
        {
            MostrarInventario();
        }

        private void MostrarInventario()
        {
            OcultarPaneles();
            PanelInventario.Visibility = Visibility.Visible;
            SeleccionarMenu("Inventario");
        }

        // NUEVO REPUESTO
        private void AbrirNuevoRepuesto()
        {
            PanelFormularioRepuesto.PrepararNuevo();
            MostrarFormularioRepuesto();
        }

        // NUEVO REPUESTO DESDE INICIO
        private void IngresarInventario_Click(
            object sender, RoutedEventArgs e)
        {
            AbrirNuevoRepuesto();
        }

        // EDITAR REPUESTO
        private void AbrirEditarRepuesto(Repuesto repuesto)
        {
            PanelFormularioRepuesto.PrepararEdicion(repuesto);
            MostrarFormularioRepuesto();
        }

        private void MostrarFormularioRepuesto()
        {
            OcultarPaneles();

            PanelFormularioRepuesto.Visibility = Visibility.Visible;

            SeleccionarMenu("Inventario");
        }

        // ========================================
        // SERVICIOS
        // ========================================

        private void Servicios_Click(
            object sender, RoutedEventArgs e)
        {
            OcultarPaneles();

            PanelServicios.Visibility = Visibility.Visible;

            SeleccionarMenu("Servicios");
        }

        // ========================================
        // EVENTO ANTERIOR
        // ========================================

        private void Button_Click(
            object sender, RoutedEventArgs e)
        {
            // Conservado por compatibilidad.
        }
    }
}
