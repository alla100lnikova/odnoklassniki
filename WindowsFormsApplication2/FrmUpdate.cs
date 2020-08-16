using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using xNet;

namespace WindowsFormsApplication2
{
    public partial class FrmUpdate : Form
    {

        public enum FormType
        {
            Insert,
            Update
        }

        public string Post
        {
            get { return tbPost.Text; }
            set { tbPost.Text = value; }
        }

        public string ImageUrl
        {
            get { return pbPhoto.ImageLocation; }
            set { pbPhoto.ImageLocation = value; }
        }

        public FrmUpdate(FormType frmType)
        {
            InitializeComponent();
            btnOK.Enabled = true;
            switch (frmType)
            {
                case FormType.Insert:
                    {
                        btnOK.Text = @"Добавить";
                        btnLoad.Visible = true;
                    }
                    break;
                case FormType.Update:
                    {
                        btnOK.Text = @"Изменить";
                        btnLoad.Visible = false;
                    }
                    break;
            }
        }

        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            if (pbPhoto.ImageLocation == null)
            {
                pbPhoto.ImageLocation = "NoPhoto.jpg";
            }
            pbPhoto.Load();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.PNG;)|*.BMP;*.JPG;*.JPEG;*.PNG;|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageUrl = openFileDialog.FileName;
                try
                {
                    pbPhoto.Load();
                }
                catch
                {
                    MessageBox.Show("Ну разве это картинка?!");
                }
            }
        }
    }
}
