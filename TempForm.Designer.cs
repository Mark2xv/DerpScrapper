namespace DerpScrapper
{
    partial class TempForm
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
            this.tbc_main = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tbc_main
            // 
            this.tbc_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbc_main.Location = new System.Drawing.Point(0, 0);
            this.tbc_main.Name = "tbc_main";
            this.tbc_main.SelectedIndex = 0;
            this.tbc_main.Size = new System.Drawing.Size(923, 472);
            this.tbc_main.TabIndex = 0;
            this.tbc_main.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tbc_main_Selecting);
            // 
            // TempForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 472);
            this.Controls.Add(this.tbc_main);
            this.Name = "TempForm";
            this.Text = "TempForm";
            this.Shown += new System.EventHandler(this.TempForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbc_main;


    }
}