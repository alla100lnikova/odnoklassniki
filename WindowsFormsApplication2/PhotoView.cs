using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class PhotoView : Form
    {
        string m_Url;
        public PhotoView(string URL)
        {
            InitializeComponent();
            m_Url = URL;
        }

        private void PhotoView_Load(object sender, EventArgs e)
        {
            pbPhoto.ImageLocation = m_Url;
            pbPhoto.Load();
            pbPhoto.Size = pbPhoto.Image.Size;
            this.Height = pbPhoto.Height + 60;
            this.Width = pbPhoto.Width + 40;
        }
    }
}
