
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KerfixDesktop
{
    public partial class DespachoView : UserControl
    {
        private readonly ObservableCollection<DespachoDemo> despachos = new();
        private readonly ObservableCollection<DetalleDespachoDemo> detallesEdicion = new();

        private readonly List<CatalogoDespachoDemo> servicios = new();
        private readonly List<CatalogoDespachoDemo> componentes = new();

        private ICollectionView? vistaDespachos;
        private DespachoDemo? despachoActual;

        // FILTRO ACTUAL
        private string filtroActual = "Todos";

        private static readonly CultureInfo CulturaChile =
            CultureInfo.GetCultureInfo("es-CL");

        public DespachoView()
        {
            InitializeComponent();

            CargarDatosPrueba();

            vistaDespachos = CollectionViewSource
                .GetDefaultView(despachos);

            vistaDespachos.Filter = FiltrarDespacho;

            TablaDespachos.ItemsSource = vistaDespachos;
            TablaDetalles.ItemsSource = detallesEdicion;

            CmbServicios.ItemsSource = servicios;
            CmbComponentes.ItemsSource = componentes;

            ActualizarEstadisticas();

            TablaDespachos.SelectedItem = despachos.FirstOrDefault();
        }

        // ========================================
        // DATOS DE PRUEBA
        // ========================================

        private void CargarDatosPrueba()
        {
            despachos.Add(new DespachoDemo
            {
                RegistroId = 12,
                Cliente = "Juan Pérez",
                Equipo = "Notebook Dell",
                FechaIngreso = new DateTime(2026, 11, 10)
            });

            despachos.Add(new DespachoDemo
            {
                RegistroId = 11,
                Cliente = "María López",
                Equipo = "PC Gamer",
                FechaIngreso = new DateTime(2026, 11, 9),
                Preparado = true,
                FechaTermino = new DateTime(2026, 11, 12),
                Observaciones = "Limpieza general y optimización.",
                Detalles = new()
                {
                    new DetalleDespachoDemo
                    {
                        Tipo = "Servicio",
                        CatalogoId = 3,
                        Descripcion = "Optimización",
                        Cantidad = 1,
                        ValorUnitario = 10000
                    },
                    new DetalleDespachoDemo
                    {
                        Tipo = "Componente",
                        CatalogoId = 2,
                        Descripcion = "SSD SATA 480 GB",
                        Cantidad = 1,
                        ValorUnitario = 25000
                    }
                }
            });

            despachos.Add(new DespachoDemo
            {
                RegistroId = 10,
                Cliente = "Carlos Muñoz",
                Equipo = "All-in-One HP",
                FechaIngreso = new DateTime(2026, 11, 8),
                Preparado = true,
                FechaTermino = new DateTime(2026, 11, 11),
                Observaciones = "Se realizó mantención general.",
                Detalles = new()
                {
                    new DetalleDespachoDemo
                    {
                        Tipo = "Servicio",
                        CatalogoId = 4,
                        Descripcion = "Mantención",
                        Cantidad = 1,
                        ValorUnitario = 15000
                    }
                }
            });

            despachos.Add(new DespachoDemo
            {
                RegistroId = 9,
                Cliente = "Ana Torres",
                Equipo = "Notebook Lenovo",
                FechaIngreso = new DateTime(2026, 11, 7)
            });

            despachos.Add(new DespachoDemo
            {
                RegistroId = 8,
                Cliente = "Felipe Rojas",
                Equipo = "PC Escritorio",
                FechaIngreso = new DateTime(2026, 11, 6),
                Preparado = true,
                FechaTermino = new DateTime(2026, 11, 10),
                Observaciones = "Formateo realizado correctamente.",
                Detalles = new()
                {
                    new DetalleDespachoDemo
                    {
                        Tipo = "Servicio",
                        CatalogoId = 1,
                        Descripcion = "Formateo",
                        Cantidad = 1,
                        ValorUnitario = 15000
                    }
                }
            });

            // SERVICIOS FICTICIOS
            servicios.Add(new CatalogoDespachoDemo
            {
                Id = 1,
                Nombre = "Formateo",
                Valor = 15000
            });

            servicios.Add(new CatalogoDespachoDemo
            {
                Id = 2,
                Nombre = "Formateo + Office",
                Valor = 20000
            });

            servicios.Add(new CatalogoDespachoDemo
            {
                Id = 3,
                Nombre = "Optimización",
                Valor = 10000
            });

            servicios.Add(new CatalogoDespachoDemo
            {
                Id = 4,
                Nombre = "Mantención",
                Valor = 15000
            });

            servicios.Add(new CatalogoDespachoDemo
            {
                Id = 5,
                Nombre = "Diagnóstico",
                Valor = 5000
            });

            // COMPONENTES FICTICIOS
            componentes.Add(new CatalogoDespachoDemo
            {
                Id = 1,
                Nombre = "RAM DDR4 8 GB",
                Valor = 18000
            });

            componentes.Add(new CatalogoDespachoDemo
            {
                Id = 2,
                Nombre = "SSD SATA 480 GB",
                Valor = 25000
            });

            componentes.Add(new CatalogoDespachoDemo
            {
                Id = 3,
                Nombre = "HDD 1 TB",
                Valor = 28000
            });

            componentes.Add(new CatalogoDespachoDemo
            {
                Id = 4,
                Nombre = "Cable HDMI",
                Valor = 4000
            });
        }

        // ========================================
        // ESTADÍSTICAS
        // ========================================

        private void ActualizarEstadisticas()
        {
            TxtPorPreparar.Text =
                despachos.Count(d => !d.Preparado).ToString();

            TxtPreparados.Text =
                despachos.Count(d => d.Preparado).ToString();

            ActualizarCantidadMostrada();
        }

        private void ActualizarCantidadMostrada()
        {
            if (vistaDespachos == null)
                return;

            int cantidad = vistaDespachos
                .Cast<DespachoDemo>()
                .Count();

            TxtCantidadMostrada.Text =
                $"Mostrando: {cantidad} registros completados";
        }

        // ========================================
        // BUSCADOR Y FILTRO
        // ========================================

        private bool FiltrarDespacho(object objeto)
        {
            if (objeto is not DespachoDemo despacho)
                return false;

            string texto = TxtBuscarDespacho.Text.Trim();

            bool coincideBusqueda =
                string.IsNullOrWhiteSpace(texto) ||
                despacho.RegistroTexto.Contains(
                    texto, StringComparison.OrdinalIgnoreCase) ||
                despacho.Cliente.Contains(
                    texto, StringComparison.OrdinalIgnoreCase) ||
                despacho.Equipo.Contains(
                    texto, StringComparison.OrdinalIgnoreCase);

            bool coincideFiltro = filtroActual switch
            {
                "Por preparar" => !despacho.Preparado,
                "Preparados" => despacho.Preparado,
                _ => true
            };

            return coincideBusqueda && coincideFiltro;
        }

        // ABRIR MENÚ DE FILTROS
        private void BtnFiltrarDespacho_Click(
            object sender, RoutedEventArgs e)
        {
            if (BtnFiltrarDespacho.ContextMenu != null)
            {
                BtnFiltrarDespacho.ContextMenu.PlacementTarget =
                    BtnFiltrarDespacho;

                BtnFiltrarDespacho.ContextMenu.IsOpen = true;
            }
        }

        // SELECCIONAR FILTRO
        private void FiltroDespacho_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem opcion)
            {
                filtroActual = opcion.Tag?.ToString() ?? "Todos";

                ActualizarVista();
            }
        }

        // BUSCADOR
        private void Buscar_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            if (vistaDespachos == null)
                return;

            ActualizarVista();
        }

        // ACTUALIZAR LISTADO
        private void ActualizarVista()
        {
            if (vistaDespachos == null)
                return;

            vistaDespachos.Refresh();
            ActualizarCantidadMostrada();

            if (TablaDespachos.SelectedItem == null)
            {
                TablaDespachos.SelectedItem = vistaDespachos
                    .Cast<DespachoDemo>()
                    .FirstOrDefault();
            }

            MostrarSeleccionado();
        }

        // ========================================
        // SELECCIÓN DEL DESPACHO
        // ========================================

        private void TablaDespachos_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (PanelDetalle == null)
                return;

            despachoActual =
                TablaDespachos.SelectedItem as DespachoDemo;

            MostrarSeleccionado();
        }

        private void MostrarSeleccionado()
        {
            despachoActual =
                TablaDespachos.SelectedItem as DespachoDemo;

            PanelFormulario.Visibility = Visibility.Collapsed;
            PanelDetalle.Visibility = Visibility.Visible;

            if (despachoActual == null)
            {
                TxtTituloRegistro.Text = "Selecciona un registro";
                TxtCliente.Text = "";
                TxtEquipo.Text = "";
                TxtFecha.Text = "";
                TxtSituacion.Text = "";

                TxtObservacionesDetalle.Text = "";
                TxtTotalDetalle.Text = "Total: $0";

                ListaDetalleLectura.ItemsSource = null;

                BtnPrepararEditar.IsEnabled = false;
                BtnGenerarPdf.IsEnabled = false;

                return;
            }

            TxtTituloRegistro.Text =
                "Registro " + despachoActual.RegistroTexto;

            TxtCliente.Text =
                "Cliente: " + despachoActual.Cliente;

            TxtEquipo.Text =
                "Equipo: " + despachoActual.Equipo;

            TxtFecha.Text =
                "Ingreso: " +
                despachoActual.FechaIngreso.ToString("dd/MM/yyyy");

            TxtSituacion.Text =
                "Situación: " + despachoActual.Situacion;

            TxtObservacionesDetalle.Text =
                despachoActual.Preparado
                    ? despachoActual.Observaciones
                    : "Todavía no se ha preparado el despacho.";

            ListaDetalleLectura.ItemsSource =
                despachoActual.Preparado
                    ? despachoActual.Detalles
                    : null;

            TxtTotalDetalle.Text =
                "Total: " + Precio(despachoActual.Total);

            BtnPrepararEditar.Content =
                despachoActual.Preparado
                    ? "Editar despacho"
                    : "Preparar despacho";

            BtnPrepararEditar.IsEnabled = true;

            // PDF SOLO PARA DESPACHOS PREPARADOS
            BtnGenerarPdf.IsEnabled = despachoActual.Preparado;
            BtnGenerarPdf.Opacity =
                despachoActual.Preparado ? 1.0 : 0.5;
        }

        // ========================================
        // PREPARAR O EDITAR DESPACHO
        // ========================================

        private void PrepararEditar_Click(
            object sender, RoutedEventArgs e)
        {
            if (despachoActual == null)
                return;

            TxtTituloFormulario.Text =
                despachoActual.Preparado
                    ? "Editar despacho"
                    : "Preparar despacho";

            TxtObservaciones.Text =
                despachoActual.Observaciones;

            detallesEdicion.Clear();

            foreach (var detalle in despachoActual.Detalles)
            {
                detallesEdicion.Add(new DetalleDespachoDemo
                {
                    Tipo = detalle.Tipo,
                    CatalogoId = detalle.CatalogoId,
                    Descripcion = detalle.Descripcion,
                    Cantidad = detalle.Cantidad,
                    ValorUnitario = detalle.ValorUnitario
                });
            }

            CmbServicios.SelectedIndex = -1;
            CmbComponentes.SelectedIndex = -1;

            TxtCantidadServicio.Text = "1";
            TxtCantidadComponente.Text = "1";

            ActualizarTotal();

            PanelDetalle.Visibility = Visibility.Collapsed;
            PanelFormulario.Visibility = Visibility.Visible;
        }

        // ========================================
        // AGREGAR SERVICIO
        // ========================================

        private void AgregarServicio_Click(
            object sender, RoutedEventArgs e)
        {
            AgregarDetalle(
                CmbServicios.SelectedItem as CatalogoDespachoDemo,
                TxtCantidadServicio.Text,
                "Servicio");
        }

        // ========================================
        // AGREGAR COMPONENTE
        // ========================================

        private void AgregarComponente_Click(
            object sender, RoutedEventArgs e)
        {
            AgregarDetalle(
                CmbComponentes.SelectedItem as CatalogoDespachoDemo,
                TxtCantidadComponente.Text,
                "Componente");
        }

        private void AgregarDetalle(
            CatalogoDespachoDemo? item,
            string textoCantidad,
            string tipo)
        {
            if (item == null)
            {
                Aviso(
                    tipo == "Servicio"
                        ? "Selecciona un servicio."
                        : "Selecciona un componente.");

                return;
            }

            if (!int.TryParse(textoCantidad, out int cantidad) ||
                cantidad <= 0)
            {
                Aviso("Ingresa una cantidad mayor a cero.");
                return;
            }

            var existente = detallesEdicion.FirstOrDefault(d =>
                d.Tipo == tipo &&
                d.CatalogoId == item.Id);

            if (existente != null)
            {
                if ((long)existente.Cantidad + cantidad > int.MaxValue)
                {
                    Aviso("La cantidad supera el límite permitido.");
                    return;
                }

                existente.Cantidad += cantidad;
                TablaDetalles.Items.Refresh();
            }
            else
            {
                detallesEdicion.Add(new DetalleDespachoDemo
                {
                    Tipo = tipo,
                    CatalogoId = item.Id,
                    Descripcion = item.Nombre,
                    Cantidad = cantidad,
                    ValorUnitario = item.Valor
                });
            }

            ActualizarTotal();
        }

        // ========================================
        // QUITAR SERVICIO O COMPONENTE
        // ========================================

        private void QuitarDetalle_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.Tag is DetalleDespachoDemo detalle)
            {
                detallesEdicion.Remove(detalle);
                ActualizarTotal();
            }
        }

        // ========================================
        // CALCULAR TOTAL
        // ========================================

        private void ActualizarTotal()
        {
            decimal total = detallesEdicion.Sum(d => d.Subtotal);

            TxtTotalFormulario.Text = Precio(total);
        }

        private static string Precio(decimal monto)
        {
            return "$" + monto.ToString("N0", CulturaChile);
        }

        // ========================================
        // GUARDAR DESPACHO DE PRUEBA
        // ========================================

        private void GuardarDespacho_Click(
            object sender, RoutedEventArgs e)
        {
            if (despachoActual == null)
                return;

            string observaciones = TxtObservaciones.Text.Trim();

            if (string.IsNullOrWhiteSpace(observaciones))
            {
                Aviso(
                    "Ingresa el trabajo realizado, " +
                    "observaciones o recomendaciones.");

                return;
            }

            if (detallesEdicion.Count == 0)
            {
                Aviso("Agrega al menos un servicio o componente.");
                return;
            }

            despachoActual.Observaciones = observaciones;

            despachoActual.Detalles = detallesEdicion
                .Select(d => new DetalleDespachoDemo
                {
                    Tipo = d.Tipo,
                    CatalogoId = d.CatalogoId,
                    Descripcion = d.Descripcion,
                    Cantidad = d.Cantidad,
                    ValorUnitario = d.ValorUnitario
                })
                .ToList();

            despachoActual.Preparado = true;
            despachoActual.FechaTermino ??= DateTime.Now;

            vistaDespachos?.Refresh();

            ActualizarEstadisticas();

            // Si el registro ya no coincide con el filtro,
            // seleccionar otro automáticamente.
            if (!FiltrarDespacho(despachoActual))
            {
                TablaDespachos.SelectedItem = vistaDespachos?
                    .Cast<DespachoDemo>()
                    .FirstOrDefault();
            }

            MostrarSeleccionado();

            MessageBox.Show(
                "Despacho actualizado en los datos de prueba.\n" +
                "El guardado real en la API está pendiente.",
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // ========================================
        // CANCELAR SIN GUARDAR
        // ========================================

        private void Cancelar_Click(
            object sender, RoutedEventArgs e)
        {
            detallesEdicion.Clear();
            MostrarSeleccionado();
        }

        // ========================================
        // GENERAR PDF (PENDIENTE PARA TU AMIGO)
        // ========================================

        private void GenerarPdf_Click(
            object sender, RoutedEventArgs e)
        {
            if (despachoActual == null)
                return;

            if (!despachoActual.Preparado)
            {
                Aviso("Primero debes preparar el despacho.");
                return;
            }

            MessageBox.Show(
                "La generación del PDF estará disponible próximamente.",
                "KerFix Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // ========================================
        // AVISOS
        // ========================================

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
    // MODELOS TEMPORALES
    // ========================================

    public class DespachoDemo
    {
        public int RegistroId { get; set; }
        public string Cliente { get; set; } = "";
        public string Equipo { get; set; } = "";

        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaTermino { get; set; }

        public bool Preparado { get; set; }
        public string Observaciones { get; set; } = "";

        public List<DetalleDespachoDemo> Detalles { get; set; } = new();

        public string RegistroTexto => $"R-{RegistroId:0000}";

        public string Situacion =>
            Preparado ? "Preparado" : "Por preparar";

        public decimal Total => Detalles.Sum(d => d.Subtotal);

        public string TotalTexto =>
            Preparado
                ? "$" + Total.ToString("N0", CulturaPrecio)
                : "—";

        private static readonly CultureInfo CulturaPrecio =
            CultureInfo.GetCultureInfo("es-CL");
    }

    public class DetalleDespachoDemo
    {
        public string Tipo { get; set; } = "";
        public int CatalogoId { get; set; }
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }

        public decimal Subtotal => Cantidad * ValorUnitario;

        public string SubtotalTexto =>
            "$" + Subtotal.ToString(
                "N0", CultureInfo.GetCultureInfo("es-CL"));

        public string ResumenPrecio =>
            $"{Tipo} · {Cantidad} × " +
            "$" + ValorUnitario.ToString(
                "N0", CultureInfo.GetCultureInfo("es-CL")) +
            " = " + SubtotalTexto;
    }

    public class CatalogoDespachoDemo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Valor { get; set; }
    }
}
