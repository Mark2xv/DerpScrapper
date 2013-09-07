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
            this.LibraryTabs = new System.Windows.Forms.TabControl();
            this.tpg_addNew = new System.Windows.Forms.TabPage();
            this.LibraryTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // LibraryTabs
            // 
            this.LibraryTabs.Controls.Add(this.tpg_addNew);
            this.LibraryTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LibraryTabs.Location = new System.Drawing.Point(0, 0);
            this.LibraryTabs.Name = "LibraryTabs";
            this.LibraryTabs.SelectedIndex = 0;
            this.LibraryTabs.Size = new System.Drawing.Size(1039, 494);
            this.LibraryTabs.TabIndex = 0;
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
            this.Controls.Add(this.LibraryTabs);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.LibraryTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl LibraryTabs;
        private System.Windows.Forms.TabPage tpg_addNew;
    }
}