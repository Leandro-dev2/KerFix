
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KerfixDesktop
{
    public partial class FormularioRepuestoView : UserControl
    {
        public event EventHandler? Cancelado;
        public event EventHandler<DatosRepuestoEventArgs>? Guardado;

        private bool modoEdicion = false;
        private int? idRepuesto = null;

        public FormularioRepuestoView()
        {
            InitializeComponent();
        }

        // ========================================
        // MODO NUEVO REPUESTO
        // ========================================

        public void PrepararNuevo()
        {
            modoEdicion = false;
            idRepuesto = null;

            TxtTitulo.Text = "Nuevo repuesto";
            BtnGuardar.Content = "Guardar repuesto";

            CmbTipoRepuesto.SelectedIndex = -1;
            TxtEspecificacion.Clear();
            TxtCapacidad.Clear();
            TxtCantidad.Clear();
            TxtPrecio.Clear();
        }

        // ========================================
        // MODO EDITAR REPUESTO
        // ========================================

        public void PrepararEdicion(Repuesto repuesto)
        {
            modoEdicion = true;
            idRepuesto = repuesto.Id;

            TxtTitulo.Text = "Editar repuesto";
            BtnGuardar.Content = "Guardar cambios";

            // Seleccionar tipo desde la lista
            string tipo = repuesto.Categoria;

            if (string.IsNullOrWhiteSpace(tipo))
                tipo = repuesto.TipoRepuesto;

            tipo = tipo.Trim().ToLowerInvariant() switch
            {
                "ram" => "RAM",
                "memoria ram" => "RAM",
                "placas" => "Placa",
                "placa" => "Placa",
                "diverso" => "Vario",
                "vario" => "Vario",
                "almacenamiento" => "Almacenamiento",
                "disco sólido" => "Almacenamiento",
                "disco duro" => "Almacenamiento",
                _ => tipo
            };

            CmbTipoRepuesto.SelectedIndex = -1;

            foreach (ComboBoxItem item in CmbTipoRepuesto.Items)
            {
                if (string.Equals(
                    item.Content?.ToString(),
                    tipo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    CmbTipoRepuesto.SelectedItem = item;
                    break;
                }
            }

            // Cargar los datos existentes
            TxtEspecificacion.Text = repuesto.Especificacion;
            TxtCapacidad.Text = repuesto.Capacidad;
            TxtCantidad.Text = repuesto.Cantidad.ToString();
            TxtPrecio.Text = repuesto.Precio.ToString("0.##");
        }

        // ========================================
        // SOLO NÚMEROS
        // ========================================

        private void SoloNumeros(
            object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
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
        // GUARDAR / EDITAR
        // ========================================

        private void Guardar_Click(
            object sender, RoutedEventArgs e)
        {
            if (CmbTipoRepuesto.SelectedItem
                is not ComboBoxItem seleccionado)
            {
                MostrarAviso("Selecciona un tipo de repuesto.");
                return;
            }

            string tipo =
                seleccionado.Content?.ToString() ?? "";

            string especificacion =
                TxtEspecificacion.Text.Trim();

            string capacidad =
                TxtCapacidad.Text.Trim();

            if (string.IsNullOrWhiteSpace(especificacion))
            {
                MostrarAviso("Ingresa la especificación.");
                return;
            }

            if (string.IsNullOrWhiteSpace(capacidad))
            {
                MostrarAviso("Ingresa la capacidad.");
                return;
            }

            if (!int.TryParse(
                TxtCantidad.Text, out int cantidad)
                || cantidad <= 0)
            {
                MostrarAviso(
                    "Ingresa una cantidad mayor a cero.");
                return;
            }

            if (!decimal.TryParse(
                TxtPrecio.Text, out decimal precio)
                || precio < 0)
            {
                MostrarAviso("Ingresa un precio válido.");
                return;
            }

            // Datos para la interfaz y futura API
            var datos = new DatosRepuestoEventArgs
            {
                Id = idRepuesto,
                EsEdicion = modoEdicion,
                TipoRepuesto = tipo,
                Especificacion = especificacion,
                Capacidad = capacidad,
                Cantidad = cantidad,
                Precio = precio
            };

            Guardado?.Invoke(this, datos);
        }

        private void MostrarAviso(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    // INFORMACIÓN ENTREGADA AL GUARDAR
    public class DatosRepuestoEventArgs : EventArgs
    {
        public int? Id { get; set; }
        public bool EsEdicion { get; set; }

        public string TipoRepuesto { get; set; } = "";
        public string Especificacion { get; set; } = "";
        public string Capacidad { get; set; } = "";

        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
