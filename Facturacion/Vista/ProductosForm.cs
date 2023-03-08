using Datos;
using Entidades;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Vista
{
    public partial class ProductosForm : Form
    {
        public ProductosForm()
        {
            InitializeComponent();
        }
        string operacion;
        Producto producto = new Producto();
        ProductoDB productoDB = new ProductoDB();
        DataTable dt = new DataTable();

        private void NuevoButton_Click(object sender, System.EventArgs e)
        {
            operacion = "Nuevo";
            HabilitarControles();
        }

        private void HabilitarControles()
        {
            CodigoTextBox.Enabled = true;
            DescripcionTextBox.Enabled = true;
            ExistenciaTextBox.Enabled = true;
            PrecioTextBox.Enabled = true;
            AdjuntarImagenButton.Enabled = true;
            GuardarButton.Enabled = true;
            CancelarButton.Enabled = true;
            NuevoButton.Enabled = false;
        }
        private void DeshabilitarControles()
        {
            CodigoTextBox.Enabled = false;
            DescripcionTextBox.Enabled = false;
            ExistenciaTextBox.Enabled = false;
            PrecioTextBox.Enabled = false;
            AdjuntarImagenButton.Enabled = false;
            GuardarButton.Enabled = false;
            CancelarButton.Enabled = false;
            NuevoButton.Enabled = true;
        }

        private void LimpiarControles()
        {
            CodigoTextBox.Clear();
            DescripcionTextBox.Clear();
            ExistenciaTextBox.Clear();
            PrecioTextBox.Clear();
            AdjuntarImagenButton.Image = null;
            EstaActivoCheckBox.Checked = false;
        }

        private void CancelarButton_Click(object sender, System.EventArgs e)
        {
            DeshabilitarControles();
        }

        private void ModificarButton_Click(object sender, System.EventArgs e)
        {
            operacion = "Modificar";

            if (ProductosDataGridView.SelectedRows.Count > 0)
            {
                CodigoTextBox.ReadOnly = true;
                CodigoTextBox.Text = ProductosDataGridView.CurrentRow.Cells["Codigo"].Value.ToString();
                DescripcionTextBox.Text = ProductosDataGridView.CurrentRow.Cells["Descripcion"].Value.ToString();
                ExistenciaTextBox.Text = ProductosDataGridView.CurrentRow.Cells["Existencia"].Value.ToString();
                PrecioTextBox.Text = ProductosDataGridView.CurrentRow.Cells["Precio"].Value.ToString();
                EstaActivoCheckBox.Checked = Convert.ToBoolean(ProductosDataGridView.CurrentRow.Cells["EstaActivo"].Value);

                byte[] img = productoDB.DevolverImagen(ProductosDataGridView.CurrentRow.Cells["Codigo"].Value.ToString());

                if (img.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(img);
                    ImagenPictureBox.Image = Bitmap.FromStream(ms);
                }
                HabilitarControles();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un registro");
            }
        }

        private void GuardarButton_Click(object sender, System.EventArgs e)
        {
            if (operacion == "Nuevo")
            {
                if (string.IsNullOrEmpty(CodigoTextBox.Text))
                {
                    errorProvider1.SetError(CodigoTextBox, "Ingrese un Código del producto");
                    CodigoTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();

                if (string.IsNullOrEmpty(DescripcionTextBox.Text))
                {
                    errorProvider1.SetError(DescripcionTextBox, "Ingrese una descripción");
                    DescripcionTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();

                //Insertamos en la BD

                producto.Codigo = CodigoTextBox.Text;
                producto.Descripcion = DescripcionTextBox.Text;
                producto.Existencia = Convert.ToInt32(ExistenciaTextBox.Text);
                producto.Precio = Convert.ToDecimal(PrecioTextBox.Text);
                producto.EstaActivo = EstaActivoCheckBox.Checked;

                if (ImagenPictureBox.Image != null)
                {
                    MemoryStream ms = new MemoryStream();
                    ImagenPictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    producto.Foto = ms.GetBuffer();
                }

                bool inserto = productoDB.Insertar(producto);
                if (inserto)
                {
                    DeshabilitarControles();
                    LimpiarControles();
                    TraerProductos();
                    MessageBox.Show("Registro guardado con exito");
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el registro");
                }
            }
            else if (operacion == "Modificar")
            {
                if (string.IsNullOrEmpty(CodigoTextBox.Text))
                {
                    errorProvider1.SetError(CodigoTextBox, "Ingrese un Código del producto");
                    CodigoTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();

                if (string.IsNullOrEmpty(DescripcionTextBox.Text))
                {
                    errorProvider1.SetError(DescripcionTextBox, "Ingrese una descripción");
                    DescripcionTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();
                //Insertamos en la BD

                producto.Codigo = CodigoTextBox.Text;
                producto.Descripcion = DescripcionTextBox.Text;
                producto.Existencia = Convert.ToInt32(ExistenciaTextBox.Text);
                producto.Precio = Convert.ToDecimal(PrecioTextBox.Text);
                producto.EstaActivo = EstaActivoCheckBox.Checked;

                if (ImagenPictureBox.Image != null)
                {
                    MemoryStream ms = new MemoryStream();
                    ImagenPictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    producto.Foto = ms.GetBuffer();
                }

                bool modifico = productoDB.Editar(producto);
                if (modifico)
                {
                    DeshabilitarControles();
                    LimpiarControles();
                    TraerProductos();
                    MessageBox.Show("Registro Actualizado con exito");
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el registro");
                }

            }
        }

        private void ExistenciaTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
                errorProvider1.Clear();
            }
            else
            {
                e.Handled = true;
                errorProvider1.SetError(ExistenciaTextBox, "Ingrese valores numéricos");
            }
        }

        private void PrecioTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
                errorProvider1.SetError(PrecioTextBox, "Ingrese valores numéricos");
            }
            else
            {
                e.Handled = false;
                errorProvider1.Clear();
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void ProductosForm_Load(object sender, EventArgs e)
        {
            TraerProductos();
        }

        private void TraerProductos()
        {
            ProductosDataGridView.DataSource = productoDB.DevolverProductos();
        }

        private void AdjuntarImagenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                ImagenPictureBox.Image = Image.FromFile(dialog.FileName);
            }
        }

        private void EliminarButton_Click(object sender, EventArgs e)
        {
            if (ProductosDataGridView.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("¿Seguro que desea eliminar el registro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    bool elimino = productoDB.Eliminar(ProductosDataGridView.CurrentRow.Cells["Codigo"].Value.ToString());

                    if (elimino)
                    {
                        TraerProductos();
                        MessageBox.Show("Registro eliminado");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el registro");
                    }
                }

            }
        }
    }
}
