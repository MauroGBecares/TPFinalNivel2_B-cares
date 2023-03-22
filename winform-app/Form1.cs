using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace winform_app
{
    public partial class FormArticulos : Form
    {
        private List<Articulos> listaArticulos;
        public FormArticulos()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboCampo.Items.Add("Codigo de Articulo");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Marca");
            cboCampo.Items.Add("Categoria");
            cboCampo.Items.Add("Precio");
            cargar();
        }
        public void cargar()
        {
            ArticulosNegocio negocio = new ArticulosNegocio();
            try
            {
                listaArticulos = negocio.listar();
                dgvArticulos.DataSource = listaArticulos;
                ocultarColumnas();
            }
            catch (Exception)
            {
                MessageBox.Show("Ha surgido un error, contactese con el desarrollador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ocultarColumnas()
        {
            dgvArticulos.Columns["Id"].Visible = false;
            dgvArticulos.Columns["UrlImagen"].Visible = false;
            dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "0.00";
        }
        public void cargarImagen(string imagen)
        {  
            try
            {
                pbxArticulo.Load(imagen);
                if (pbxArticulo.Image.Width < 200)
                    pbxArticulo.SizeMode = PictureBoxSizeMode.Normal;
                else
                    pbxArticulo.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (Exception)
            {
                pbxArticulo.Load("https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM=");
                pbxArticulo.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulos seleccionado = (Articulos)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }    
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AltaArticulo ventana = new AltaArticulo();
            ventana.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulos seleccionado = (Articulos)dgvArticulos.CurrentRow.DataBoundItem;
            AltaArticulo ventana = new AltaArticulo(seleccionado);
            ventana.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticulosNegocio negocio = new ArticulosNegocio();
            try
            {
                Articulos seleccionado = (Articulos)dgvArticulos.CurrentRow.DataBoundItem;
                DialogResult resultado = MessageBox.Show("Esta seguro que quiere eliminar el producto?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    negocio.eliminarArticulo(seleccionado.Id);
                    cargar();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ha surgido un error, contactese con el desarrollador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            Articulos seleccionado = (Articulos)dgvArticulos.CurrentRow.DataBoundItem;
            DetalleArticulo ventana = new DetalleArticulo(seleccionado);
            ventana.ShowDialog();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            cargar();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Codigo de Articulo")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Igual");
                cboCriterio.Items.Add("Comienza con");
            }
            else if (opcion == "Nombre" || opcion == "Marca" || opcion == "Categoria")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Contiene");
                cboCriterio.Items.Add("Termina con");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Menos a");
                cboCriterio.Items.Add("Igual");
                cboCriterio.Items.Add("Mayor a");
            }
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            ArticulosNegocio negocio = new ArticulosNegocio();
            try
            {
                if (validarFiltro())
                    return;
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltro.Text;
                dgvArticulos.DataSource = negocio.filtro(campo, criterio, filtro);
            }
            catch (Exception)
            {
                MessageBox.Show("Ha surgido un error, contactese con el desarrollador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un campo");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un criterio");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltro.Text))
                {
                    MessageBox.Show("Debe cargar números");
                }
            }
            return false;
        }

        private void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
            }
        }
    }
}
