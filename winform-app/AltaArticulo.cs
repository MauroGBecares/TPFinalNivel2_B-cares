using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace winform_app
{
    public partial class AltaArticulo : Form
    {
        private Articulos articulo = null;
        private List<Articulos> listaArticulos;
        public AltaArticulo()
        {
            InitializeComponent();
            lblTitulo.Text = "Agregar";
        }
        public AltaArticulo(Articulos articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Articulo";
            lblTitulo.Text = "Modificar";
            txtCodigo.ReadOnly = true;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticulosNegocio articulosNegocio = new ArticulosNegocio();
                      
            if (articulo == null)
                articulo = new Articulos();

            if (validarCarga())
                return;

            articulo.Codigo = txtCodigo.Text;
            articulo.Nombre = txtNombre.Text;
            articulo.Descripcion = txtDescripcion.Text;
            articulo.Marca = (Marcas)cboMarca.SelectedItem;
            articulo.Categoria = (Categorias)cboCategoria.SelectedItem;
            articulo.UrlImagen = txtImagenUrl.Text;
            articulo.Precio = decimal.Parse(txtPrecio.Text);
            if (articulo.Id == 0)
            {
                try
                {
                    if (validarCodigo())
                        return;
                }
                catch (Exception)
                {
                    MessageBox.Show("Ha surgido un error, contactese con el desarrollador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                articulosNegocio.AgregarArticulo(articulo);
                MessageBox.Show("Agregado con éxito");
            }
            else
            {
                articulosNegocio.ModificarArticulo(articulo);
                MessageBox.Show("Modificado con éxito");
            }
            Close();
        }

        private void AltaArticulo_Load(object sender, EventArgs e)
        {
            CategoriasNegocio categoriasNegocio = new CategoriasNegocio();
            MarcasNegocio marcasNegocio = new MarcasNegocio();
            try
            {
                cboCategoria.DataSource = categoriasNegocio.listarCategorias();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";
                cboMarca.DataSource = marcasNegocio.listarMarcas();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";
            
                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    cboCategoria.SelectedValue = articulo.Categoria.Id;
                    txtImagenUrl.Text = articulo.UrlImagen;
                    cargarImagen(articulo.UrlImagen);
                    txtPrecio.Text = articulo.Precio.ToString("0.00");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ha surgido un error, contactese con el desarrollador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtImagenUrl_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagenUrl.Text);
        }
        public void cargarImagen(string imagen)
        {
            try
            {
                pbxAltaImagen.Load(imagen);
                if (pbxAltaImagen.Image.Width < 400)
                    pbxAltaImagen.SizeMode = PictureBoxSizeMode.Normal;
                else
                    pbxAltaImagen.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (Exception)
            {
                pbxAltaImagen.Load("https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM=");
                pbxAltaImagen.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
                e.KeyChar = ',';
            if (!char.IsControl(e.KeyChar) && !char.IsNumber(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }
        private bool validarCarga()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtDescripcion.Text) || string.IsNullOrEmpty(txtPrecio.Text))
            {
                MessageBox.Show("Debe llenar los espacios vacios");
                if (string.IsNullOrEmpty(txtCodigo.Text))
                    txtCodigo.BackColor = Color.LightCoral;
                else
                    txtCodigo.BackColor = System.Drawing.SystemColors.Window;
                if (string.IsNullOrEmpty(txtNombre.Text))
                    txtNombre.BackColor = Color.LightCoral;
                else
                    txtNombre.BackColor = System.Drawing.SystemColors.Window;
                if (string.IsNullOrEmpty(txtDescripcion.Text))
                    txtDescripcion.BackColor = Color.LightCoral;
                else
                    txtDescripcion.BackColor = System.Drawing.SystemColors.Window;
                if (string.IsNullOrEmpty(txtPrecio.Text))
                    txtPrecio.BackColor = Color.LightCoral;
                else
                    txtPrecio.BackColor = System.Drawing.SystemColors.Window;
                return true;
            }
            if (!(letraNumero(txtCodigo.Text)))
            {
                MessageBox.Show("Para agregar un codigo nuevo debe ser primero letra y luego números");
                return true;
            }
            return false;
        }
        private bool letraNumero(string cadena)
        {
            bool esLetra = false;
            if (char.IsLetter(cadena[0]))
                esLetra = true;
            bool esNumero = true;
            for (int i = 1; i < cadena.Length; i++)
            {
                if (!char.IsNumber(cadena[i]))
                {
                    esNumero = false;
                    break;
                }
            }
            if (esLetra && esNumero)
                return true;
            else
                return false;
        }
        private bool validarCodigo()
        {
            ArticulosNegocio negocio = new ArticulosNegocio();
            try
            {
            listaArticulos = negocio.listar();
            foreach (Articulos item in listaArticulos)
            {
                if (item.Codigo == txtCodigo.Text)
                {
                    MessageBox.Show("El codigo ya existe");
                    return true;
                }
            }

            return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
