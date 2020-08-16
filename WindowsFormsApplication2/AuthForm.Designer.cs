namespace WindowsFormsApplication2
{
    partial class AuthForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthForm));
            this.wbOkApplication = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbOkApplication
            // 
            this.wbOkApplication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbOkApplication.Location = new System.Drawing.Point(0, 0);
            this.wbOkApplication.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbOkApplication.Name = "wbOkApplication";
            this.wbOkApplication.Size = new System.Drawing.Size(630, 367);
            this.wbOkApplication.TabIndex = 1;
            this.wbOkApplication.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbOkApplication_Navigated);
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 367);
            this.Controls.Add(this.wbOkApplication);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AuthForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AuthForm";
            this.Load += new System.EventHandler(this.AuthForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbOkApplication;
    }
}