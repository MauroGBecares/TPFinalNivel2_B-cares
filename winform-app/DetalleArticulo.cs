using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;

namespace winform_app
{
    public partial class DetalleArticulo : Form
    {
        private Articulos articulo;

        public DetalleArticulo(Articulos articulos)
        {
            InitializeComponent();
            this.articulo = articulos;
        }

        private void DetalleArticulo_Load(object sender, EventArgs e)
        {         
            txtDetalle.Lines = new string[]
            {
                "Codigo: " + articulo.Codigo.ToString(),
                "Nombre: " + articulo.Nombre.ToString(),
                "Descripcion: " + articulo.Descripcion.ToString(),
                "Marca: " + articulo.Marca.ToString(),
                "Categoria: " + articulo.Categoria.ToString(),
                "Precio: " + articulo.Precio.ToString("0.00")
            };
        }
    }
}
