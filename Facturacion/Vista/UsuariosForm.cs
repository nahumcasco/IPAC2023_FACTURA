using Datos;
using Entidades;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Vista
{
    public partial class UsuariosForm : Form
    {
        public UsuariosForm()
        {
            InitializeComponent();
        }
        string tipoOperacion = string.Empty;
        UsuarioDB usuarioDB = new UsuarioDB();
        Usuario user = new Usuario();

        private void NuevoButton_Click(object sender, System.EventArgs e)
        {
            HabilitarControles();
            GuardarButton.Enabled = true;
            tipoOperacion = "Nuevo";
        }

        private void HabilitarControles()
        {
            CodigoTextBox.Enabled = true;
            NombreTextBox.Enabled = true;
            ContraseñaTextBox.Enabled = true;
            CorreoTextBox.Enabled = true;
            RolComboBox.Enabled = true;
            EstaActivoCheckBox.Enabled = true;
            AdjuntarImagenButton.Enabled = true;
            CancelarButton.Enabled = true;
            GuardarButton.Enabled = true;
        }

        private void DesabilitarControles()
        {
            CodigoTextBox.Enabled = false;
            NombreTextBox.Enabled = false;
            ContraseñaTextBox.Enabled = false;
            CorreoTextBox.Enabled = false;
            RolComboBox.Enabled = false;
            EstaActivoCheckBox.Enabled = false;
            AdjuntarImagenButton.Enabled = false;
            GuardarButton.Enabled = false;
        }
        private void LimpiarControles()
        {
            CodigoTextBox.Clear();
            NombreTextBox.Clear();
            ContraseñaTextBox.Clear();
            CorreoTextBox.Clear();
            RolComboBox.Text = "";
            EstaActivoCheckBox.Checked = false;
            FotoPictureBox.Image = null;
        }
        private void CancelarButton_Click(object sender, System.EventArgs e)
        {
            DesabilitarControles();
            LimpiarControles();
            //ModificarButton.Enabled = false;
            GuardarButton.Enabled = false;
            EliminarButton.Enabled = false;
            CancelarButton.Enabled = false;
        }

        private void AdjuntarImagenButton_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                FotoPictureBox.Image = Image.FromFile(dialog.FileName);
            }
        }

        private void GuardarButton_Click(object sender, System.EventArgs e)
        {
            if (tipoOperacion == "Nuevo")
            {
                if (string.IsNullOrEmpty(CodigoTextBox.Text))
                {
                    errorProvider1.SetError(CodigoTextBox, "Ingrese el código");
                    CodigoTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();
                if (string.IsNullOrEmpty(NombreTextBox.Text))
                {
                    errorProvider1.SetError(NombreTextBox, "Ingrese un nombre");
                    NombreTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();
                if (string.IsNullOrEmpty(ContraseñaTextBox.Text))
                {
                    errorProvider1.SetError(ContraseñaTextBox, "Ingrese una contraseña");
                    ContraseñaTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();
                if (string.IsNullOrEmpty(RolComboBox.Text))
                {
                    errorProvider1.SetError(RolComboBox, "Seleccione un rol");
                    RolComboBox.Focus();
                    return;
                }
                errorProvider1.Clear();

                user.CodigoUsuario = CodigoTextBox.Text;
                user.Nombre = NombreTextBox.Text;
                user.Contraseña = ContraseñaTextBox.Text;
                user.Correo = CorreoTextBox.Text;
                user.Rol = RolComboBox.Text;
                user.EstaActivo = EstaActivoCheckBox.Checked;

                if (FotoPictureBox.Image != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    FotoPictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    user.Foto = ms.GetBuffer();
                }

                //Insertar en la base de datos

                bool inserto = usuarioDB.Insertar(user);
                if (inserto)
                {
                    LimpiarControles();
                    DesabilitarControles();
                    TraerUsuarios();
                    MessageBox.Show("Registro guardado");
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el registro");
                }
            }
            else if (tipoOperacion == "Modificar")
            {
                user.CodigoUsuario = CodigoTextBox.Text;
                user.Nombre = NombreTextBox.Text;
                user.Contraseña = ContraseñaTextBox.Text;
                user.Correo = CorreoTextBox.Text;
                user.Rol = RolComboBox.Text;
                user.EstaActivo = EstaActivoCheckBox.Checked;

                if (FotoPictureBox.Image != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    FotoPictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    user.Foto = ms.GetBuffer();
                }

                bool edito = usuarioDB.Editar(user);
                if (edito)
                {
                    LimpiarControles();
                    DesabilitarControles();
                    TraerUsuarios();
                    MessageBox.Show("Registro Actualizado");
                }
                else
                {
                    MessageBox.Show("No se pudo Actualizar el registro");
                }
            }
        }

        private void ModificarButton_Click(object sender, System.EventArgs e)
        {
            tipoOperacion = "Modificar";

            if (UsuariosDataGridView.SelectedRows.Count > 0)
            {
                CodigoTextBox.Text = UsuariosDataGridView.CurrentRow.Cells["CodigoUsuario"].Value.ToString();
                NombreTextBox.Text = UsuariosDataGridView.CurrentRow.Cells["Nombre"].Value.ToString();
                ContraseñaTextBox.Text = UsuariosDataGridView.CurrentRow.Cells["Contrasena"].Value.ToString();
                CorreoTextBox.Text = UsuariosDataGridView.CurrentRow.Cells["Correo"].Value.ToString();
                RolComboBox.Text = UsuariosDataGridView.CurrentRow.Cells["Rol"].Value.ToString();
                EstaActivoCheckBox.Checked = Convert.ToBoolean(UsuariosDataGridView.CurrentRow.Cells["EstaActivo"].Value);

                byte[] img = usuarioDB.DevolverImagen(UsuariosDataGridView.CurrentRow.Cells["CodigoUsuario"].Value.ToString());

                if (img.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(img);
                    FotoPictureBox.Image = Bitmap.FromStream(ms);
                }

                HabilitarControles();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un registro");
            }
        }

        private void UsuariosForm_Load(object sender, System.EventArgs e)
        {
            TraerUsuarios();
        }

        private void TraerUsuarios()
        {
            DataTable dt = new DataTable();
            dt = usuarioDB.DevolverUsuarios();
            UsuariosDataGridView.DataSource = dt;
        }

        private void EliminarButton_Click(object sender, EventArgs e)
        {
            if (UsuariosDataGridView.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("¿Seguro que desea eliminar el registro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    bool elimino = usuarioDB.Eliminar(UsuariosDataGridView.CurrentRow.Cells["CodigoUsuario"].Value.ToString());
                    if (elimino)
                    {
                        MessageBox.Show("Registro eliminado correctamente");
                        TraerUsuarios();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el registro");
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un registro");
            }
        }

    }
}
