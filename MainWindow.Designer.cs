namespace DerpScrapper
{
    partial class MainWindow
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tpg_addNew = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tpg_addNew);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1039, 494);
            this.tabs.TabIndex = 0;
            // 
            // tpg_addNew
            // 
            this.tpg_addNew.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tpg_addNew.Location = new System.Drawing.Point(4, 22);
            this.tpg_addNew.Name = "tpg_addNew";
            this.tpg_addNew.Size = new System.Drawing.Size(1031, 468);
            this.tpg_addNew.TabIndex = 0;
            this.tpg_addNew.Text = "Add new collection";
            this.tpg_addNew.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 494);
            this.Controls.Add(this.tabs);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tpg_addNew;
    }
}