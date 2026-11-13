
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KerfixDesktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // ABRIR INVENTARIO
        private void Inventario_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar inicio y mostrar inventario
            PanelInicio.Visibility = Visibility.Collapsed;
            PanelInventario.Visibility = Visibility.Visible;

            // Actualizar botones del menú
            BtnInicio.Background = Brushes.Transparent;
            BtnInventario.Background = new SolidColorBrush(
                Color.FromRgb(40, 103, 189));
        }

        // VOLVER AL INICIO
        private void Inicio_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar inventario y mostrar inicio
            PanelInventario.Visibility = Visibility.Collapsed;
            PanelInicio.Visibility = Visibility.Visible;

            // Actualizar botones del menú
            BtnInventario.Background = Brushes.Transparent;
            BtnInicio.Background = new SolidColorBrush(
                Color.FromRgb(40, 103, 189));
        }

        // EVENTO ANTERIOR
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Lo conservamos por compatibilidad.
        }
    }
}
