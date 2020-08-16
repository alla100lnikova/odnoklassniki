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
    public partial class AuthForm : Form
    {
        MainForm m_FrmMain;

        public AuthForm(MainForm FrmMain)
        {
            InitializeComponent();
            m_FrmMain = FrmMain;
        }

        private void wbOkApplication_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (wbOkApplication.Url.ToString().IndexOf("https://localhost/auth") != -1)
            {
                char[] Symbols = { '=', '&' };
                string[] URL = wbOkApplication.Url.ToString().Split(Symbols);
                m_FrmMain.m_AccessToken = URL[1];
                this.Close();
            }
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            wbOkApplication.Navigate("https://connect.ok.ru/oauth/authorize?client_id=1255627776&scope=VALUABLE_ACCESS&response_type=token&redirect_uri=https://localhost/auth&layout=w");
        }
    }
}
