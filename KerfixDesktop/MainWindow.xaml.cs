
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KerfixDesktop
{
    public partial class MainWindow : Window
    {
        private readonly Brush colorSeleccionado =
            new SolidColorBrush(Color.FromRgb(40, 103, 189));

        public MainWindow()
        {
            InitializeComponent();

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

            PanelNuevoRegistro.Guardado += (s, datos) =>
            {
                MessageBox.Show(
                    "Formulario validado correctamente.\n" +
                    "El guardado en la base de datos está pendiente.",
                    "KerFix Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
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

        private void Clientes_Click(
            object sender, RoutedEventArgs e)
        {
            OcultarPaneles();
            PanelClientes.Visibility = Visibility.Visible;
            SeleccionarMenu("Clientes");
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
