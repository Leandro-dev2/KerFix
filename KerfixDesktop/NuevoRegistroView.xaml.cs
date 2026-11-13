
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KerfixDesktop
{
    public partial class NuevoRegistroView : UserControl
    {
        public event EventHandler? Cancelado;
        public event EventHandler<NuevoRegistroDatos>? Guardado;

        private readonly List<ClienteRegistroPrueba> clientes = new();

        public NuevoRegistroView()
        {
            InitializeComponent();

            CargarDatosPrueba();
            PrepararNuevo();
        }

        // ========================================
        // CLIENTES Y EQUIPOS FICTICIOS
        // ========================================

        private void CargarDatosPrueba()
        {
            clientes.Add(new ClienteRegistroPrueba
            {
                Id = 101,
                Nombre = "Constructora Andes SpA",
                Telefono = "+56 9 8765 4321",
                Equipos = new()
                {
                    new EquipoRegistroPrueba
                    {
                        Id = 1,
                        Tipo = "Notebook",
                        Marca = "Dell",
                        Modelo = "Latitude 5420"
                    },
                    new EquipoRegistroPrueba
                    {
                        Id = 2,
                        Tipo = "PC Escritorio",
                        Marca = "HP",
                        Modelo = "ProDesk 400 G9"
                    }
                }
            });

            clientes.Add(new ClienteRegistroPrueba
            {
                Id = 102,
                Nombre = "Minera del Norte Ltda.",
                Telefono = "+56 9 7123 9876",
                Equipos = new()
                {
                    new EquipoRegistroPrueba
                    {
                        Id = 3,
                        Tipo = "Notebook",
                        Marca = "HP",
                        Modelo = "ProBook 440 G9"
                    },
                    new EquipoRegistroPrueba
                    {
                        Id = 4,
                        Tipo = "PC Escritorio",
                        Marca = "HP",
                        Modelo = "EliteDesk 800 G6"
                    }
                }
            });

            clientes.Add(new ClienteRegistroPrueba
            {
                Id = 103,
                Nombre = "Servicios Industriales S.A.",
                Telefono = "+56 9 9988 7766",
                Equipos = new()
                {
                    new EquipoRegistroPrueba
                    {
                        Id = 5,
                        Tipo = "All-in-One",
                        Marca = "Lenovo",
                        Modelo = "ThinkCentre M90a"
                    }
                }
            });

            clientes.Add(new ClienteRegistroPrueba
            {
                Id = 104,
                Nombre = "AgroEquipos Chile",
                Telefono = "+56 9 6677 5544",
                Equipos = new()
                {
                    new EquipoRegistroPrueba
                    {
                        Id = 6,
                        Tipo = "Impresora",
                        Marca = "Epson",
                        Modelo = "L3250"
                    }
                }
            });

            clientes.Add(new ClienteRegistroPrueba
            {
                Id = 105,
                Nombre = "Transporte Rivera",
                Telefono = "+56 9 4433 2211",
                Equipos = new()
                {
                    new EquipoRegistroPrueba
                    {
                        Id = 7,
                        Tipo = "Notebook",
                        Marca = "Acer",
                        Modelo = "Aspire 5"
                    }
                }
            });

            CmbClienteExistente.ItemsSource = clientes;
        }

        // ========================================
        // REINICIAR FORMULARIO
        // ========================================

        public void PrepararNuevo()
        {
            RbNuevoCliente.IsChecked = true;
            RbPendiente.IsChecked = true;

            TxtNombreNuevo.Clear();
            TxtTelefonoNuevo.Clear();

            CmbClienteExistente.SelectedIndex = -1;
            CmbEquipoExistente.ItemsSource = null;

            CmbTipoEquipo.SelectedIndex = -1;
            TxtMarca.Clear();
            TxtModelo.Clear();
            TxtDiagnostico.Clear();

            TxtTelefonoExistente.Text = "";

            ActualizarPaneles();
        }

        // ========================================
        // CAMBIAR NUEVO / EXISTENTE
        // ========================================

        private void ModoCliente_Checked(
            object sender, RoutedEventArgs e)
        {
            // Puede ejecutarse durante InitializeComponent
            if (PanelNuevoCliente == null ||
                PanelClienteExistente == null ||
                PanelEquipoNuevo == null)
                return;

            ActualizarPaneles();
        }

        private void ActualizarPaneles()
        {
            bool nuevoCliente =
                RbNuevoCliente.IsChecked == true;

            PanelNuevoCliente.Visibility =
                nuevoCliente
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            PanelClienteExistente.Visibility =
                nuevoCliente
                    ? Visibility.Collapsed
                    : Visibility.Visible;

            // Si es cliente nuevo, siempre es equipo nuevo.
            // Si es existente, depende del desplegable.
            bool equipoNuevo = nuevoCliente ||
                (CmbEquipoExistente.SelectedItem
                    is EquipoRegistroPrueba equipo &&
                 equipo.EsNuevo);

            PanelEquipoNuevo.Visibility =
                equipoNuevo
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        // ========================================
        // CLIENTE EXISTENTE SELECCIONADO
        // ========================================

        private void ClienteExistente_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (CmbEquipoExistente == null ||
                TxtTelefonoExistente == null)
                return;

            CmbEquipoExistente.ItemsSource = null;
            TxtTelefonoExistente.Text = "";

            if (CmbClienteExistente.SelectedItem
                is ClienteRegistroPrueba cliente)
            {
                TxtTelefonoExistente.Text =
                    "Teléfono: " + cliente.Telefono;

                var equipos = new List<EquipoRegistroPrueba>(
                    cliente.Equipos);

                // Siempre al final del listado
                equipos.Add(new EquipoRegistroPrueba
                {
                    EsNuevo = true,
                    Descripcion = "＋ Equipo nuevo"
                });

                CmbEquipoExistente.ItemsSource = equipos;
            }

            ActualizarPaneles();
        }

        // ========================================
        // EQUIPO SELECCIONADO
        // ========================================

        private void EquipoExistente_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (PanelEquipoNuevo == null)
                return;

            ActualizarPaneles();
        }

        // ========================================
        // CANCELAR
        // ========================================

        private void Cancelar_Click(
            object sender, RoutedEventArgs e)
        {
            Cancelado?.Invoke(this, EventArgs.Empty);
        }

        // ========================================
        // GUARDAR
        // ========================================

        private void Guardar_Click(
            object sender, RoutedEventArgs e)
        {
            bool nuevoCliente =
                RbNuevoCliente.IsChecked == true;

            var datos = new NuevoRegistroDatos
            {
                ClienteNuevo = nuevoCliente,
                Estado = RbRevisar.IsChecked == true
                    ? "En revisión"
                    : "Pendiente"
            };

            if (nuevoCliente)
            {
                datos.Nombre =
                    TxtNombreNuevo.Text.Trim();

                datos.Telefono =
                    TxtTelefonoNuevo.Text.Trim();

                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    Aviso("Ingresa el nombre del cliente.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(datos.Telefono))
                {
                    Aviso("Ingresa el teléfono del cliente.");
                    return;
                }

                datos.EquipoNuevo = true;
            }
            else
            {
                if (CmbClienteExistente.SelectedItem
                    is not ClienteRegistroPrueba cliente)
                {
                    Aviso("Selecciona un cliente.");
                    return;
                }

                if (CmbEquipoExistente.SelectedItem
                    is not EquipoRegistroPrueba equipo)
                {
                    Aviso("Selecciona un equipo.");
                    return;
                }

                datos.IdCliente = cliente.Id;
                datos.Nombre = cliente.Nombre;
                datos.Telefono = cliente.Telefono;

                datos.EquipoNuevo = equipo.EsNuevo;

                if (!equipo.EsNuevo)
                {
                    datos.IdEquipo = equipo.Id;
                    datos.Tipo = equipo.Tipo;
                    datos.Marca = equipo.Marca;
                    datos.Modelo = equipo.Modelo;
                }
            }

            // Datos de un equipo nuevo
            if (datos.EquipoNuevo)
            {
                if (CmbTipoEquipo.SelectedItem
                    is not ComboBoxItem tipoSeleccionado)
                {
                    Aviso("Selecciona el tipo de equipo.");
                    return;
                }

                datos.Tipo =
                    tipoSeleccionado.Content?.ToString() ?? "";

                datos.Marca = TxtMarca.Text.Trim();
                datos.Modelo = TxtModelo.Text.Trim();

                if (string.IsNullOrWhiteSpace(datos.Marca))
                {
                    Aviso("Ingresa la marca del equipo.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(datos.Modelo))
                {
                    Aviso("Ingresa el modelo del equipo.");
                    return;
                }
            }

            datos.Diagnostico =
                TxtDiagnostico.Text.Trim();

            if (string.IsNullOrWhiteSpace(datos.Diagnostico))
            {
                Aviso("Ingresa el diagnóstico del cliente.");
                return;
            }

            // Entrega los datos para la futura API
            Guardado?.Invoke(this, datos);
        }

        private void Aviso(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    // ========================================
    // DATOS PARA LA FUTURA API
    // ========================================

    public class NuevoRegistroDatos : EventArgs
    {
        public bool ClienteNuevo { get; set; }
        public int? IdCliente { get; set; }

        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";

        public bool EquipoNuevo { get; set; }
        public int? IdEquipo { get; set; }

        public string Tipo { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";

        public string Diagnostico { get; set; } = "";
        public string Estado { get; set; } = "Pendiente";
    }

    // ========================================
    // MODELOS PARA DATOS DE PRUEBA
    // ========================================

    public class ClienteRegistroPrueba
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";

        public List<EquipoRegistroPrueba> Equipos { get; set; }
            = new();
    }

    public class EquipoRegistroPrueba
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";

        public bool EsNuevo { get; set; }

        public string Descripcion
        {
            get
            {
                if (EsNuevo)
                    return "＋ Equipo nuevo";

                return $"{Tipo} - {Marca} {Modelo}";
            }
            set { }
        }
    }
}
