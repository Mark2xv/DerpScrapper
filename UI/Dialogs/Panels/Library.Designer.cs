namespace DerpScrapper.UI.DialogPanels
{
    partial class Library
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.libraryName = new System.Windows.Forms.TextBox();
            this.libraryPrimaryLanguage = new System.Windows.Forms.ComboBox();
            this.librarySecondaryLanguage = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name: ";
            // 
            // libraryName
            // 
            this.libraryName.Location = new System.Drawing.Point(137, 9);
            this.libraryName.Name = "libraryName";
            this.libraryName.Size = new System.Drawing.Size(142, 20);
            this.libraryName.TabIndex = 1;
            // 
            // libraryPrimaryLanguage
            // 
            this.libraryPrimaryLanguage.FormattingEnabled = true;
            this.libraryPrimaryLanguage.Location = new System.Drawing.Point(137, 37);
            this.libraryPrimaryLanguage.Name = "libraryPrimaryLanguage";
            this.libraryPrimaryLanguage.Size = new System.Drawing.Size(142, 21);
            this.libraryPrimaryLanguage.TabIndex = 2;
            // 
            // librarySecondaryLanguage
            // 
            this.librarySecondaryLanguage.FormattingEnabled = true;
            this.librarySecondaryLanguage.Location = new System.Drawing.Point(137, 68);
            this.librarySecondaryLanguage.Name = "librarySecondaryLanguage";
            this.librarySecondaryLanguage.Size = new System.Drawing.Size(142, 21);
            this.librarySecondaryLanguage.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Primary language: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Secondary language: ";
            // 
            // Library
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.librarySecondaryLanguage);
            this.Controls.Add(this.libraryPrimaryLanguage);
            this.Controls.Add(this.libraryName);
            this.Controls.Add(this.label1);
            this.Name = "Library";
            this.Size = new System.Drawing.Size(288, 96);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox libraryName;
        private System.Windows.Forms.ComboBox libraryPrimaryLanguage;
        private System.Windows.Forms.ComboBox librarySecondaryLanguage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
