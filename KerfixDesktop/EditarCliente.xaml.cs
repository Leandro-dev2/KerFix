
using System.Windows;

namespace KerfixDesktop
{
    public partial class EditarClienteWindow : Window
    {
        // Datos modificados
        public string NombreEditado { get; private set; }
        public string TelefonoEditado { get; private set; }

        public EditarClienteWindow(ClienteDemo cliente)
        {
            InitializeComponent();

            // Cargar información existente
            TxtNombre.Text = cliente.Nombre;
            TxtTelefono.Text = cliente.Telefono;

            NombreEditado = cliente.Nombre;
            TelefonoEditado = cliente.Telefono;

            TxtNombre.Focus();
        }

        // CANCELAR
        private void Cancelar_Click(
            object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // GUARDAR CAMBIOS
        private void Guardar_Click(
            object sender, RoutedEventArgs e)
        {
            string nombre = TxtNombre.Text.Trim();
            string telefono = TxtTelefono.Text.Trim();

            // Validar nombre
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    "Ingresa el nombre del cliente.",
                    "KerFix Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Validar teléfono
            if (string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show(
                    "Ingresa el teléfono del cliente.",
                    "KerFix Manager",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Guardar valores temporalmente
            NombreEditado = nombre;
            TelefonoEditado = telefono;

            // Cerrar y confirmar
            DialogResult = true;
        }
    }
}
